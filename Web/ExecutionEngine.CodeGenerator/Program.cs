/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using Remotion.Text.CommandLine;
using Remotion.Web.ExecutionEngine;
using Remotion.Utilities;
using Remotion.Text;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  class Program
  {
    private Arguments _arguments;
    private LanguageProvider _inputProvider;

    static int Main (string[] args)
    {
      // parse arguments / show usage info
      CommandLineClassParser<Arguments> parser = new CommandLineClassParser<Arguments> ();
      Arguments arguments = null;
      try
      {
        arguments = parser.Parse (args);
        new Program (arguments).Process ();
      }
      catch (CommandLineArgumentException e)
      {
        string appName = System.IO.Path.GetFileName (System.Environment.GetCommandLineArgs ()[0]);
        Console.Error.WriteLine ("re:call function generator");
        Console.Error.Write (e.Message);
        Console.Error.WriteLine ("Usage: " + parser.GetAsciiSynopsis (appName, 79));
        return 1;
      }
      catch (InputException e)
      {
        Console.Error.WriteLine ("{0}({1},{2}): error WG{3:0000}: {4}",
            e.Path, e.Line, e.Position,
            e.ErrorCode,
            e.Message.Replace ("\r", "\\r"));
        return 1;
      }
      catch (Exception e)
      {
        // write error info accpording to /verbose option
        Console.Error.WriteLine ("Execution aborted: {0}",  e.Message);
        if (arguments != null && arguments.Verbose)
        {
          Console.Error.WriteLine ("Detailed exception information:");
          for (Exception current = e; current != null; current = current.InnerException)
          {
            Console.Error.WriteLine ("{0}: {1}", e.GetType().FullName, e.Message);
            Console.Error.WriteLine (e.StackTrace);
          }
        }
        return 1;
      }

      return 0;
    }

    Program (Arguments args)
    {
      _arguments = args;
    }

    void Process ()
    {
			if (_arguments.Verbose)
				log4net.Config.BasicConfigurator.Configure ();

			// select correct CodeDOM provider and configure syntax
      CodeDomProvider codeDomProvider;
      switch (_arguments.Language)
      {
        case Language.CSharp:
          codeDomProvider = new Microsoft.CSharp.CSharpCodeProvider ();
          _inputProvider = new CSharpProvider();

          break;
        case Language.VB:
          codeDomProvider = new Microsoft.VisualBasic.VBCodeProvider();
          _inputProvider = new VBProvider();
          break;

        default:
          throw new Exception ("Unknown language " + _arguments.Language);
      }

      // generate classes for each [WxePageFunction] class
      CodeCompileUnit unit = new CodeCompileUnit ();
      FunctionDeclaration declaration = null;

			string fileMask = Path.Combine (Directory.GetCurrentDirectory (), _arguments.FileMask);
			DirectoryInfo directory = new DirectoryInfo (Path.GetDirectoryName (fileMask));
			FileInfo[] files = directory.GetFiles (
					Path.GetFileName (fileMask), 
					_arguments.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

      bool outputUpToDate = false; 
      FileInfo outputFile = new FileInfo (_arguments.OutputFile);
      if (_arguments.ProjectFile != null)
      {                
        if (outputFile.Exists)
          outputUpToDate = true;

        FileInfo projectFile = new FileInfo (_arguments.ProjectFile);
        if (! projectFile.Exists)
          throw new ApplicationException ("Project file " + _arguments.ProjectFile + " not found.");

        if (outputUpToDate && projectFile.LastWriteTimeUtc > outputFile.LastWriteTimeUtc)
          outputUpToDate = false;
      }

      char[] whitespace = new char[] {' ', '\t'};
      FileInfo file = null;
      try
      {
			  for (int idxFile = 0; idxFile < files.Length; ++idxFile)
			  {
			    file = files[idxFile];

          if (outputUpToDate && file.LastWriteTimeUtc > outputFile.LastWriteTimeUtc)
            outputUpToDate = false;

          StringBuilder xmlFragment = null;
          List<int> indents = null;         // beginning line position for each line
          bool validationFailed = false;
			    List<string> importNamespaces = new List<string>(); // namespaces to import
			    SeparatedStringBuilder currentNamespace = new SeparatedStringBuilder (".");

          StreamReader reader = new StreamReader (file.FullName, true);
          int lineNumber = 1;
          int firstLineNumber = -1; // line number of the first XML segment line
          string line = reader.ReadLine ();
          while (line != null)
				  {
            string lineArgument;
			      CodeLineType lineType = _inputProvider.ParseLine (line, out lineArgument);

            if (lineType == CodeLineType.NamespaceImport)
            {
              if (!importNamespaces.Contains (lineArgument))
                importNamespaces.Add (lineArgument);
            }
            else if (lineType == CodeLineType.NamespaceDeclaration)
            {
              currentNamespace.Append (lineArgument);
            }
            else if (lineType == CodeLineType.ClassDeclaration)
            {
              if (declaration != null && string.IsNullOrEmpty (declaration.PageType))
              {
                string type = lineArgument;
                if (currentNamespace.Length > 0)
                  type = currentNamespace + "." + type;
                declaration.PageType = type;
              }
            }
            else if (lineType == CodeLineType.LineComment)
            {
              //string originalLine = line;
              //line = line.TrimStart (whitespace);
              //if (line.StartsWith (linePrefix))
              //{
              //  line = line.Substring (linePrefix.Length);
              // line = lineArgument;
						  if (xmlFragment == null)
						  {
							  if (lineArgument.TrimStart (whitespace).StartsWith ("<" + FunctionDeclaration.ElementName))
							  {
                  if (declaration != null)
                  {
                    // generate previous wxe function
                    GenerateClass (unit, declaration, importNamespaces, file, firstLineNumber);
                    declaration = null;
                  }

							    xmlFragment = new StringBuilder (1000);
								  xmlFragment.AppendFormat ("<{0} xmlns=\"{1}\"", FunctionDeclaration.ElementName, FunctionDeclaration.SchemaUri);
                  lineArgument = lineArgument.TrimStart (whitespace).Substring (FunctionDeclaration.ElementName.Length + 1);
                  xmlFragment.Append (lineArgument);
							    indents = new List<int>();
                  indents.Add (line.IndexOf (lineArgument));

								  firstLineNumber = lineNumber;
							  }
						  }
						  else
						  {
							  xmlFragment.AppendLine ();
                xmlFragment.Append (lineArgument);
                indents.Add (line.IndexOf (lineArgument));
                if (lineArgument.TrimEnd (whitespace).EndsWith ("</" + FunctionDeclaration.ElementName + ">"))
							  {
								  // fragment complete, process it
								  StringReader stringReader = new StringReader (xmlFragment.ToString());

								  XmlSchemaSet schemas = new XmlSchemaSet ();
								  schemas.Add (FunctionDeclaration.SchemaUri, FunctionDeclaration.GetSchemaReader ());

								  XmlReaderSettings settings = new XmlReaderSettings ();
								  settings.Schemas = schemas;
								  settings.ValidationType = ValidationType.Schema;
                  settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                  settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                      {
                        XmlSchemaException schemaError = e.Exception;
                        Console.Error.WriteLine ("{0}({1},{2}): {3} WG{4:0000}: {5}",
                            file.FullName,
                            schemaError.LineNumber + firstLineNumber - 1, 
                            schemaError.LinePosition + indents[schemaError.LineNumber - 1],
                            e.Severity.ToString ().ToLower (),
                            (int)InputError.InvalidSchema,
                            schemaError.Message); 
                        if (e.Severity == XmlSeverityType.Error)
                          validationFailed = true;
                      };

							    XmlReader xmlReader = XmlReader.Create (stringReader, settings, file.FullName);
								  XmlSerializer serializer = new XmlSerializer (typeof (FunctionDeclaration), FunctionDeclaration.SchemaUri);

                  try
                  {
									  declaration = (FunctionDeclaration) serializer.Deserialize(xmlReader);
                  }
                  catch (InvalidOperationException e)
                  {
                    XmlException xmlException = e.InnerException as XmlException;
                    if (xmlException != null)
                    {
                      throw new InputException (
                          InputError.XmlError, 
                          file.FullName, 
                          xmlException.LineNumber + firstLineNumber - 1,  
                          xmlException.LinePosition + indents[xmlException.LineNumber - 1], 
                          xmlException);
                    }
                    else
                    {
                      throw;
                    }
                  }

							    if (validationFailed)
                  {
                    declaration = null;
                  }
                  else
                  {
                    if (string.IsNullOrEmpty (declaration.AspxFile))
                    {
                      string cd = Environment.CurrentDirectory;
                      string path = file.FullName;

                      // TODO: geht nicht wenn parameter filemask ein ..\ o.ä. enthält
                      Assertion.IsTrue (path.StartsWith (cd));
                      path = path.Substring (cd.Length + 1);
                      string ext = file.Extension;
                      if (!string.IsNullOrEmpty (ext))
                        path = path.Substring (0, path.Length - ext.Length);
                      declaration.AspxFile = path;
                    }

                    // replace built-in types
                    foreach (VariableDeclaration var in declaration.ParametersAndVariables)
                      var.TypeName = _inputProvider.ConvertTypeName (var.TypeName);

                    if (string.IsNullOrEmpty (declaration.FunctionBaseType))
                      declaration.FunctionBaseType = _arguments.FunctionBaseType;
                  }

							  }
						  }
					  }

					  line = reader.ReadLine();
					  ++ lineNumber;
				  }
        
          if (declaration != null)
          {
            GenerateClass (unit, declaration, importNamespaces, file, firstLineNumber);
            declaration = null;
          }
        }
      }
      catch (Exception e)
      {
        if (e is InputException || file == null)
          throw;
        else
          throw new InputException (InputError.Unknown, file.FullName, 1, 1, e);
      }

      // write generated code
      if (! outputUpToDate)
      {
        using (TextWriter writer = new StreamWriter (_arguments.OutputFile, false, Encoding.Unicode))
        {
          Console.WriteLine ("Writing classes to " + _arguments.OutputFile);
          CodeGeneratorOptions options = new CodeGeneratorOptions();
          ICodeGenerator generator = codeDomProvider.CreateGenerator (_arguments.OutputFile);
          generator.GenerateCodeFromCompileUnit (unit, writer, options);
        }
      }
    } 

		private static void SpliTypeName (string fullTypeName, out string nameSpace, out string typeName)
		{
			int pos = fullTypeName.LastIndexOf ('.');
			if (pos < 0)
			{
				nameSpace = null;
				typeName = fullTypeName;
			}
			else
			{
				nameSpace = fullTypeName.Substring (0, pos);
				typeName = fullTypeName.Substring (pos + 1);
			}
		}

    // generate output classes for [WxePageFunction] page class
    static void GenerateClass (CodeCompileUnit unit, FunctionDeclaration functionDeclaration, List<string> importNamespaces, FileInfo file, int line)
    {
      if (string.IsNullOrEmpty (functionDeclaration.PageType))
      {
        throw new InputException (InputError.ClassNotFound, file.FullName, line, 1);
      }

      string nameSpace;
			string typeName;
			SpliTypeName (functionDeclaration.PageType, out nameSpace, out typeName);

			string functionName = functionDeclaration.FunctionName;
			if (functionName == null)
				functionName = typeName + "Function";

      CodeNamespace ns = new CodeNamespace (nameSpace);
      unit.Namespaces.Add (ns);

      foreach (string importNamespace in importNamespaces)
        ns.Imports.Add (new CodeNamespaceImport (importNamespace));
      //ns.Imports.Add (new CodeNamespaceImport ("System"));
      //ns.Imports.Add (new CodeNamespaceImport ("Remotion.Web.ExecutionEngine"));

      // generate a partial class for the page that allows access to parameters and
      // local variables from page code
      CodeTypeDeclaration partialPageClass = new CodeTypeDeclaration (typeName);
      ns.Types.Add (partialPageClass);
      partialPageClass.IsPartial = true;
      partialPageClass.Attributes = MemberAttributes.Public;

      // add Return() method as alias for ExecuteNextStep()
      CodeMemberMethod returnMethod = new CodeMemberMethod ();
      partialPageClass.Members.Add (returnMethod);
      returnMethod.Name = "Return";
      returnMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
      CodeExpression executeNextStep = new CodeMethodInvokeExpression (
          new CodeThisReferenceExpression (),
          "ExecuteNextStep",
          new CodeExpression[0]);
      returnMethod.Statements.Add (executeNextStep);

      //// add Return (outPar1, outPar2, ...) method 
      //// -- removed (unneccessary, possibly confusing)
      //CodeMemberMethod returnParametersMethod = new CodeMemberMethod ();
      //foreach (WxePageParameterAttribute parameterDeclaration in GetPageParameterAttributesOrdered (type))
      //{
      //  if (parameterDeclaration.Direction != WxeParameterDirection.In)
      //  {
      //    returnParametersMethod.Parameters.Add (new CodeParameterDeclarationExpression (
      //        new CodeTypeReference (parameterDeclaration.Type),
      //        parameterDeclaration.Name));
      //    returnParametersMethod.Statements.Add (new CodeAssignStatement (
      //        new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), parameterDeclaration.Name),
      //        new CodeArgumentReferenceExpression (parameterDeclaration.Name)));
      //  }
      //}
      //if (returnParametersMethod.Parameters.Count > 0)
      //{
      //  partialPageClass.Members.Add (returnParametersMethod);
      //  returnParametersMethod.Name = "Return";
      //  returnParametersMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
      //  returnParametersMethod.Statements.Add (executeNextStep);
      //}

      // generate a WxeFunction class
      CodeTypeDeclaration functionClass = new CodeTypeDeclaration (functionName);
      ns.Types.Add (functionClass);
      functionClass.Attributes = MemberAttributes.Public;
      functionClass.BaseTypes.Add (new CodeTypeReference (functionDeclaration.FunctionBaseType));
      functionClass.CustomAttributes.Add (new CodeAttributeDeclaration (new CodeTypeReference (typeof (SerializableAttribute))));

      // generate a strongly typed CurrentFunction property for the page class
      CodeMemberProperty currentFunctionProperty = new CodeMemberProperty ();
      currentFunctionProperty.Name = "CurrentFunction";
      currentFunctionProperty.Type = new CodeTypeReference (functionClass.Name);
      currentFunctionProperty.Attributes = MemberAttributes.New | MemberAttributes.Family | MemberAttributes.Final;
      currentFunctionProperty.GetStatements.Add (new CodeMethodReturnStatement (
          new CodeCastExpression (
            new CodeTypeReference (functionClass.Name), 
            new CodePropertyReferenceExpression (
                new CodeBaseReferenceExpression (),
                "CurrentFunction"))));
      partialPageClass.Members.Add (currentFunctionProperty);

      int parameterIndex = 0;

      // generate local variables in partial/variables class, and
      // generate function parameters in partial/variables class and function class
			foreach (VariableDeclaration variableDeclaration in functionDeclaration.ParametersAndVariables)
      {
        CodeMemberProperty localProperty = new CodeMemberProperty ();
        localProperty.Name = variableDeclaration.Name;
        localProperty.Type = new CodeTypeReference (variableDeclaration.TypeName);
        // localProperty.Type = new LiteralTypeReference (variableDeclaration.TypeName);

        partialPageClass.Members.Add (localProperty);
        localProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        // TODO: can get-accessor alone be set to protected via CodeDOM?

        ParameterDeclaration parameterDeclaration = variableDeclaration as ParameterDeclaration;
        CodeMemberProperty functionProperty = null;
        if (parameterDeclaration != null)
        {
          functionProperty = new CodeMemberProperty ();
          functionClass.Members.Add (functionProperty);
          functionProperty.Name = parameterDeclaration.Name;
          functionProperty.Type = new CodeTypeReference (parameterDeclaration.TypeName);
          functionProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        }

        // <variable> := Variables["<parameterName>"]
        CodeExpression variable = new CodeIndexerExpression (
            new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), "Variables"),
            new CodePrimitiveExpression (variableDeclaration.Name));

        // <getStatement> := get { return (<type>) <variable>; }
        CodeStatement getStatement = new CodeMethodReturnStatement (
            new CodeCastExpression (
                new CodeTypeReference (variableDeclaration.TypeName),
                variable));

        // <setStatement> := set { <variable> = value; }
        CodeStatement setStatement = new CodeAssignStatement (
            variable,
            new CodePropertySetValueReferenceExpression ());

        if (parameterDeclaration != null)
        {
          // add get/set accessors according to parameter direction
          if (parameterDeclaration.Direction != WxeParameterDirection.Out)
          {
            // In or InOut: get from page, set from function
            localProperty.GetStatements.Add (getStatement);
            functionProperty.SetStatements.Add (setStatement);
          }

          if (parameterDeclaration.Direction != WxeParameterDirection.In)
          {
            // Out or InOut: get from function
            functionProperty.GetStatements.Add (getStatement);
          }

          // all directions: set from page
          localProperty.SetStatements.Add (setStatement);
        }
        else
        {
          // variables always have get and set, and are only added to the local variable collection
          localProperty.GetStatements.Add (getStatement);
          localProperty.SetStatements.Add (setStatement);
        }

        if (functionProperty != null)
        {
          // add attribute [WxeParameter (parameterIndex, [Required,] Direction)
          CodeAttributeDeclaration wxeParameterAttribute = new CodeAttributeDeclaration (
              new CodeTypeReference (typeof (WxeParameterAttribute)));
          functionProperty.CustomAttributes.Add (wxeParameterAttribute);
          wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (
              new CodePrimitiveExpression (parameterIndex)));
          if (parameterDeclaration.IsRequired.HasValue)
          {
            wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (
                new CodePrimitiveExpression (parameterDeclaration.IsRequired.Value)));
          }
          wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (
              new CodeFieldReferenceExpression (
                  new CodeTypeReferenceExpression (typeof (WxeParameterDirection)),
                  parameterDeclaration.Direction.ToString ())));
        }

        if (parameterDeclaration != null)
          ++ parameterIndex;
      }

      // add PageStep to WXE function
      CodeMemberField step1 = new CodeMemberField (typeof (WxePageStep), "Step1");
      functionClass.Members.Add (step1);
      step1.InitExpression = new CodeObjectCreateExpression (
          new CodeTypeReference (typeof (WxePageStep)),
          new CodePrimitiveExpression (functionDeclaration.AspxFile));

      // add constructors to WXE function

      // ctor () {}
      CodeConstructor defaultCtor = new CodeConstructor ();
      functionClass.Members.Add (defaultCtor);
      defaultCtor.Attributes = MemberAttributes.Public;

      // ctor (params object[] args): base (args) {}
      // replace by (VarRef<type1> arg1, VarRef<type2> arg2, ...)
      //CodeConstructor untypedCtor = new CodeConstructor ();
      //functionClass.Members.Add (untypedCtor);
      //untypedCtor.Attributes = MemberAttributes.Public;
      //CodeParameterDeclarationExpression untypedParameters = new CodeParameterDeclarationExpression (
      //    new CodeTypeReference (typeof (object[])),
      //    "args");
      //untypedParameters.CustomAttributes.Add (new CodeAttributeDeclaration ("System.ParamArrayAttribute"));
      //untypedCtor.Parameters.Add (untypedParameters);
      //untypedCtor.BaseConstructorArgs.Add (new CodeArgumentReferenceExpression ("args"));

      // ctor (<type1> inarg1, <type2> inarg2, ...): base (inarg1, inarg2, ...) {}
      CodeConstructor typedCtor = new CodeConstructor ();
      typedCtor.Attributes = MemberAttributes.Public;
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction == WxeParameterDirection.Out)
          break;

        typedCtor.Parameters.Add (new CodeParameterDeclarationExpression (
            new CodeTypeReference (parameterDeclaration.TypeName),
            parameterDeclaration.Name));

        typedCtor.BaseConstructorArgs.Add (new CodeArgumentReferenceExpression (parameterDeclaration.Name));
      }
      if (typedCtor.Parameters.Count > 0)
        functionClass.Members.Add (typedCtor);

      // <returnType> Call (IWxePage page, WxeExecuteFunctionOptions options, <type> [ref|out] param1, <type> [ref|out] param2, ...)
      CodeMemberMethod callMethod = new CodeMemberMethod ();
      partialPageClass.Members.Add (callMethod);
      callMethod.Name = "Call";
      callMethod.Attributes = MemberAttributes.Static | MemberAttributes.Public;
      callMethod.Parameters.Add (new CodeParameterDeclarationExpression (
          new CodeTypeReference (typeof (IWxePage)), "currentPage"));
      callMethod.Parameters.Add (new CodeParameterDeclarationExpression (
          new CodeTypeReference (typeof (IWxeCallArguments)), "arguments"));
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.IsReturnValue)
        {
          callMethod.ReturnType = new CodeTypeReference (parameterDeclaration.TypeName);
        }
        else
        {
          CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression (
              new CodeTypeReference (parameterDeclaration.TypeName),
              parameterDeclaration.Name);
          callMethod.Parameters.Add (parameter);
          if (parameterDeclaration.Direction == WxeParameterDirection.InOut)
            parameter.Direction = FieldDirection.Ref;
          else if (parameterDeclaration.Direction == WxeParameterDirection.Out)
            parameter.Direction = FieldDirection.Out;
        }
      }
      // <class>Function function;
      CodeVariableDeclarationStatement functionVariable = new CodeVariableDeclarationStatement (
          new CodeTypeReference (functionClass.Name), "function");
      callMethod.Statements.Add (functionVariable);
      // common variables
      CodeArgumentReferenceExpression currentPage = new CodeArgumentReferenceExpression ("currentPage");
      CodeVariableReferenceExpression function = new CodeVariableReferenceExpression ("function");
      // if (! currentPage.IsReturningPostBack)
      CodeConditionStatement ifNotIsReturningPostBack = new CodeConditionStatement ();
      callMethod.Statements.Add (ifNotIsReturningPostBack);
      ifNotIsReturningPostBack.Condition = new CodeBinaryOperatorExpression (
          new CodePropertyReferenceExpression (currentPage, "IsReturningPostBack"),
          CodeBinaryOperatorType.ValueEquality,
          new CodePrimitiveExpression (false));
      // { 
      //   function = new <class>Function();
      ifNotIsReturningPostBack.TrueStatements.Add (new CodeAssignStatement (
          function, 
          new CodeObjectCreateExpression (new CodeTypeReference (functionClass.Name))));
      //   function.ParamN = ParamN;
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction != WxeParameterDirection.Out)
        {
          ifNotIsReturningPostBack.TrueStatements.Add (new CodeAssignStatement (
              new CodePropertyReferenceExpression (function, parameterDeclaration.Name),
              new CodeArgumentReferenceExpression (parameterDeclaration.Name)));
        }
      }
      //   function.SetCatchExceptionTypes (typeof (Exception));
      ifNotIsReturningPostBack.TrueStatements.Add (new CodeMethodInvokeExpression (
          function,
          "SetCatchExceptionTypes", 
          new CodeTypeOfExpression (typeof (Exception))));
      //  currentPage.ExecuteFunction (function, arguments);
      ifNotIsReturningPostBack.TrueStatements.Add (new CodeMethodInvokeExpression (
          currentPage,
          "ExecuteFunction",
          function,
          new CodeVariableReferenceExpression ("arguments")));
              // //   currentPage.ExecuteFunction (function);
              // ifNotIsReturningPostBack.TrueStatements.Add (new CodeMethodInvokeExpression (
              //     currentPage, "ExecuteFunction",
              //     new CodeExpression[] { new CodeVariableReferenceExpression ("function") }));
              //    throw new Exception ("(Unreachable code)"); 
      ifNotIsReturningPostBack.TrueStatements.Add (new CodeThrowExceptionStatement (
          new CodeObjectCreateExpression (new CodeTypeReference (typeof (Exception)),
          new CodeExpression[] {
            new CodePrimitiveExpression ("(Unreachable code)") })));
      // } else {
      //   function = (<class>Function) currentPage.ReturningFunction;
      ifNotIsReturningPostBack.FalseStatements.Add (new CodeAssignStatement (
          function,
          new CodeCastExpression (
              new CodeTypeReference (functionClass.Name),
              new CodePropertyReferenceExpression (currentPage, "ReturningFunction"))));
      //    if (function.Exception != null)
      CodeConditionStatement ifException = new CodeConditionStatement ();
      ifNotIsReturningPostBack.FalseStatements.Add (ifException);
      ifException.Condition = new CodeBinaryOperatorExpression (
          new CodePropertyReferenceExpression (
              function,
              "Exception"),
          CodeBinaryOperatorType.IdentityInequality,
          new CodePrimitiveExpression (null));
      //      throw function.Exception;
      ifException.TrueStatements.Add (new CodeThrowExceptionStatement (
          new CodePropertyReferenceExpression (
              function,
              "Exception")
          ));
      //   ParamN = function.ParamN;
			foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction != WxeParameterDirection.In && !parameterDeclaration.IsReturnValue)
        {
          ifNotIsReturningPostBack.FalseStatements.Add (new CodeAssignStatement (
              new CodeArgumentReferenceExpression (parameterDeclaration.Name),
              new CodePropertyReferenceExpression (function, parameterDeclaration.Name)));
        }
        else if (parameterDeclaration.IsReturnValue)
        {
          // TODO: Throw Exception if any but last parameter has return value flag!
          ifNotIsReturningPostBack.FalseStatements.Add (new CodeMethodReturnStatement (
            new CodePropertyReferenceExpression (function, parameterDeclaration.Name)));
        }
      }

      // <returnType> Call (IWxePage page, <type> [ref|out] param1, <type> [ref|out] param2, ...)
      CodeMemberMethod callMethodOverload = new CodeMemberMethod ();
      callMethodOverload.Name = callMethod.Name;
      callMethodOverload.ReturnType = callMethod.ReturnType;
      callMethodOverload.Attributes = callMethod.Attributes;
      CodeExpression[] callOverloadParameters = new CodeExpression[callMethod.Parameters.Count];
      for (int i = 0; i < callMethod.Parameters.Count; ++i)
      {
        if (i == 1) // options parameter
        {
          callOverloadParameters[i] = new CodePropertyReferenceExpression (new CodeTypeReferenceExpression(typeof (WxeCallArguments)), "Default");
        }
        else
        {
          callMethodOverload.Parameters.Add (callMethod.Parameters[i]);
          callOverloadParameters[i] = new CodeDirectionExpression (
              callMethod.Parameters[i].Direction,
              new CodeArgumentReferenceExpression (callMethod.Parameters[i].Name));
        }
      }
      partialPageClass.Members.Add (callMethodOverload);
      // [return] Call (page, WxeCallArguments.Default, param1, param2, ...);
      CodeMethodInvokeExpression callOverloadStatement = new CodeMethodInvokeExpression (
          new CodeTypeReferenceExpression ( /*nameSpace + "." + */partialPageClass.Name), callMethod.Name, callOverloadParameters);
      if (callMethod.ReturnType.BaseType != typeof(void).FullName)
        callMethodOverload.Statements.Add (new CodeMethodReturnStatement (callOverloadStatement));
      else
        callMethodOverload.Statements.Add (callOverloadStatement);

    }
  }
}
