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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using log4net.Config;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Remotion.Text;
using Remotion.Text.CommandLine;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  internal class Program
  {
    private readonly Arguments _arguments;
    private LanguageProvider _inputProvider;

    private static int Main (string[] args)
    {
      // parse arguments / show usage info
      CommandLineClassParser<Arguments> parser = new CommandLineClassParser<Arguments>();
      Arguments arguments = null;
      try
      {
        arguments = parser.Parse (args);
        new Program (arguments).Process();
      }
      catch (CommandLineArgumentException e)
      {
        string appName = Path.GetFileName (Environment.GetCommandLineArgs()[0]);
        Console.Error.WriteLine ("re:call function generator");
        Console.Error.Write (e.Message);
        Console.Error.WriteLine ("Usage: " + parser.GetAsciiSynopsis (appName, 79));
        return 1;
      }
      catch (InputException e)
      {
        Console.Error.WriteLine (
            "{0}({1},{2}): error WG{3:0000}: {4}",
            e.Path,
            e.Line,
            e.Position,
            e.ErrorCode,
            e.Message.Replace ("\r", "\\r"));
        return 1;
      }
      catch (Exception e)
      {
        // write error info accpording to /verbose option
        Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
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

    private Program (Arguments args)
    {
      _arguments = args;
    }

    private void Process ()
    {
      if (_arguments.Verbose)
        BasicConfigurator.Configure();

      // select correct CodeDOM provider and configure syntax
      CodeDomProvider codeDomProvider;
      switch (_arguments.Language)
      {
        case Language.CSharp:
          codeDomProvider = new CSharpCodeProvider();
          _inputProvider = new CSharpProvider();

          break;
        case Language.VB:
          codeDomProvider = new VBCodeProvider();
          _inputProvider = new VBProvider();
          break;

        default:
          throw new Exception ("Unknown language " + _arguments.Language);
      }

      // generate classes for each [WxePageFunction] class
      CodeCompileUnit unit = new CodeCompileUnit();
      FunctionDeclaration declaration = null;

      string fileMask = Path.Combine (Directory.GetCurrentDirectory(), _arguments.FileMask);
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

      char[] whitespace = new char[] { ' ', '\t' };
      FileInfo file = null;
      try
      {
        for (int idxFile = 0; idxFile < files.Length; ++idxFile)
        {
          file = files[idxFile];

          if (outputUpToDate && file.LastWriteTimeUtc > outputFile.LastWriteTimeUtc)
            outputUpToDate = false;

          StringBuilder xmlFragment = null;
          List<int> indents = null; // beginning line position for each line
          bool validationFailed = false;
          List<string> importNamespaces = new List<string>(); // namespaces to import
          SeparatedStringBuilder currentNamespace = new SeparatedStringBuilder (".");

          StreamReader reader = new StreamReader (file.FullName, true);
          int lineNumber = 1;
          int firstLineNumber = -1; // line number of the first XML segment line
          string line = reader.ReadLine();
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
              currentNamespace.Append (lineArgument);
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
                    var generator = new PageFunctionGenerator (unit, declaration, importNamespaces, file, firstLineNumber);
                    generator.GenerateClass();
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
                xmlFragment.AppendLine();
                xmlFragment.Append (lineArgument);
                indents.Add (line.IndexOf (lineArgument));
                if (lineArgument.TrimEnd (whitespace).EndsWith ("</" + FunctionDeclaration.ElementName + ">"))
                {
                  // fragment complete, process it
                  StringReader stringReader = new StringReader (xmlFragment.ToString());

                  XmlSchemaSet schemas = new XmlSchemaSet();
                  schemas.Add (FunctionDeclaration.SchemaUri, FunctionDeclaration.GetSchemaReader());

                  XmlReaderSettings settings = new XmlReaderSettings();
                  settings.Schemas = schemas;
                  settings.ValidationType = ValidationType.Schema;
                  settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                  settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                  {
                    XmlSchemaException schemaError = e.Exception;
                    Console.Error.WriteLine (
                        "{0}({1},{2}): {3} WG{4:0000}: {5}",
                        file.FullName,
                        schemaError.LineNumber + firstLineNumber - 1,
                        schemaError.LinePosition + indents[schemaError.LineNumber - 1],
                        e.Severity.ToString().ToLower(),
                        (int) InputError.InvalidSchema,
                        schemaError.Message);
                    if (e.Severity == XmlSeverityType.Error)
                      validationFailed = true;
                  };

                  XmlReader xmlReader = XmlReader.Create (stringReader, settings, file.FullName);
                  XmlSerializer serializer = new XmlSerializer (typeof (FunctionDeclaration), FunctionDeclaration.SchemaUri);

                  try
                  {
                    declaration = (FunctionDeclaration) serializer.Deserialize (xmlReader);
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
                      throw;
                  }

                  if (validationFailed)
                    declaration = null;
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
            var generator = new PageFunctionGenerator (unit, declaration, importNamespaces, file, firstLineNumber);
            generator.GenerateClass ();
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
  }
}