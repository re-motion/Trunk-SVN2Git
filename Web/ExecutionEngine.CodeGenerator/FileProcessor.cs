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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Remotion.Text;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class FileProcessor
  {
    private readonly LanguageProvider _languageProvider;
    private readonly string _functionBaseType;

    public FileProcessor (LanguageProvider languageProvider, string functionBaseType)
    {
      ArgumentUtility.CheckNotNull ("languageProvider", languageProvider);
      _languageProvider = languageProvider;
      _functionBaseType = functionBaseType;
    }

    private class XmlFragmentContext
    {
      public readonly StringBuilder XmlFragment;
      /// <summary> line number of the first XML segment line </summary>
      public readonly int FirstLineNumber;
      /// <summary>  beginning line position for each line </summary>
      public readonly List<int> Indents;

      public XmlFragmentContext (StringBuilder xmlFragment, int firstLineNumber, List<int> indents)
      {
        ArgumentUtility.CheckNotNull ("xmlFragment", xmlFragment);
        ArgumentUtility.CheckNotNull ("indents", indents);

        XmlFragment = xmlFragment;
        FirstLineNumber = firstLineNumber;
        Indents = indents;
      }
    }

    private class CommentLineContext
    {
      public readonly XmlFragmentContext XmlFragmentContext;
      public readonly FunctionDeclaration FunctionDeclaration;
      public readonly bool IsXmlFragmentComplete;

      public CommentLineContext (XmlFragmentContext xmlFragmentContext, FunctionDeclaration functionDeclaration, bool isXmlFragmentComplete)
      {
        XmlFragmentContext = xmlFragmentContext;
        FunctionDeclaration = functionDeclaration;
        IsXmlFragmentComplete = isXmlFragmentComplete;
      }
    }

    private class SchemaValidationObject
    {
      private readonly FileInfo _file;
      private readonly int _firstLineNumber;
      private readonly List<int> _indents;
      private bool _hasFailed;

      public SchemaValidationObject (FileInfo file, int firstLineNumber, List<int> indents)
      {
        ArgumentUtility.CheckNotNull ("file", file);
        ArgumentUtility.CheckNotNull ("indents", indents);

        _file = file;
        _firstLineNumber = firstLineNumber;
        _indents = indents;
      }

      public bool HasFailed
      {
        get { return _hasFailed; }
      }

      public ValidationEventHandler CreateValidationHandler ()
      {
        return delegate (object sender, ValidationEventArgs e)
        {
          XmlSchemaException schemaError = e.Exception;

          int lineNumber = schemaError.LineNumber + _firstLineNumber - 1;
          int linePosition = schemaError.LinePosition + _indents[schemaError.LineNumber - 1];
          string errorMessage = string.Format (
              "{0}({1},{2}): {3} WG{4:0000}: {5}",
              _file.FullName,
              lineNumber,
              linePosition,
              e.Severity.ToString ().ToLower (),
              (int) InputError.InvalidSchema,
              schemaError.Message);
          Console.Error.WriteLine (errorMessage);

          if (e.Severity == XmlSeverityType.Error)
            _hasFailed = true;
        };
      }
    }

    public void ProcessFile (FileInfo file, char[] whitespace, CodeCompileUnit unit)
    {
      CommentLineContext commentLineContext = new CommentLineContext (null, null, false);
      List<string> importNamespaces = new List<string>(); // namespaces to import
      SeparatedStringBuilder currentNamespace = new SeparatedStringBuilder (".");

      StreamReader reader = new StreamReader (file.FullName, true);
      int lineNumber = 1;

      for (string line = reader.ReadLine (); line != null; line = reader.ReadLine(), lineNumber++)
      {
        string lineArgument;
        CodeLineType lineType = _languageProvider.ParseLine (line, out lineArgument);

        if (lineType == CodeLineType.NamespaceImport)
        {
          ProcessNamespaceImportLine (importNamespaces, lineArgument);
        }
        else if (lineType == CodeLineType.NamespaceDeclaration)
        {
          ProcessNamespaceDeclarationLine (lineArgument, currentNamespace);
        }
        else if (lineType == CodeLineType.LineComment)
        {
          commentLineContext = ProcessCommentLine (file, line, lineArgument, commentLineContext, whitespace, lineNumber);
          if (commentLineContext.IsXmlFragmentComplete)
            continue;
        }
        else if (lineType == CodeLineType.ClassDeclaration)
        {
          if (!commentLineContext.IsXmlFragmentComplete && commentLineContext.XmlFragmentContext != null)
            ProcessXmlFragment (commentLineContext.XmlFragmentContext, file);

          ProcessClassDeclarationLine (lineArgument, currentNamespace, commentLineContext.FunctionDeclaration);
          break;
        }
      }

      if (commentLineContext.IsXmlFragmentComplete && commentLineContext.FunctionDeclaration != null)
        GenerateClass (file, importNamespaces, commentLineContext.FunctionDeclaration, unit, commentLineContext.XmlFragmentContext.FirstLineNumber);
    }

    private void ProcessNamespaceImportLine (List<string> importNamespaces, string lineArgument)
    {
      if (!importNamespaces.Contains (lineArgument))
        importNamespaces.Add (lineArgument);
    }

    private void ProcessNamespaceDeclarationLine (string lineArgument, SeparatedStringBuilder currentNamespace)
    {
      currentNamespace.Append (lineArgument);
    }

    private CommentLineContext ProcessCommentLine (FileInfo file, string line, string lineArgument, CommentLineContext commentLineContext, char[] whitespace, int lineNumber)
    {
      if (commentLineContext.XmlFragmentContext == null)
      {
        if (lineArgument.TrimStart (whitespace).StartsWith ("<" + FunctionDeclaration.ElementName))
        {
          return new CommentLineContext (ProcessXmlFragmentOpeningTagLine (line, lineArgument, whitespace, lineNumber), null, false);
        }

        return commentLineContext;
      }
      else
      {
        commentLineContext.XmlFragmentContext.XmlFragment.AppendLine ();
        commentLineContext.XmlFragmentContext.XmlFragment.Append (lineArgument);
        commentLineContext.XmlFragmentContext.Indents.Add (line.IndexOf (lineArgument));
        if (lineArgument.TrimEnd (whitespace).EndsWith ("</" + FunctionDeclaration.ElementName + ">"))
          return ProcessXmlFragmentClosingTagLine (commentLineContext.XmlFragmentContext, file);
        else
          return commentLineContext;
      }
    }

    private XmlFragmentContext ProcessXmlFragmentOpeningTagLine (string line, string lineArgument, char[] whitespace, int lineNumber)
    {
      StringBuilder xmlFragment = new StringBuilder (1000);
      xmlFragment.AppendFormat ("<{0} xmlns=\"{1}\"", FunctionDeclaration.ElementName, FunctionDeclaration.SchemaUri);
      lineArgument = lineArgument.TrimStart (whitespace).Substring (FunctionDeclaration.ElementName.Length + 1);
      xmlFragment.Append (lineArgument);
      var indents = new List<int> ();
      indents.Add (line.IndexOf (lineArgument));

      return new XmlFragmentContext (xmlFragment, lineNumber, indents);
    }

    private CommentLineContext ProcessXmlFragmentClosingTagLine (XmlFragmentContext xmlFragmentContext, FileInfo file)
    {
      FunctionDeclaration declaration = ProcessXmlFragment(xmlFragmentContext, file);
      if (declaration == null)
        return new CommentLineContext (xmlFragmentContext, null, true);

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
        var.TypeName = _languageProvider.ConvertTypeName (var.TypeName);

      if (string.IsNullOrEmpty (declaration.FunctionBaseType))
        declaration.FunctionBaseType = _functionBaseType;
      return new CommentLineContext (xmlFragmentContext, declaration, true);
    }

    private FunctionDeclaration ProcessXmlFragment (XmlFragmentContext xmlfFragmentContext, FileInfo file)
    {
      // fragment complete, process it
      StringReader stringReader = new StringReader (xmlfFragmentContext.XmlFragment.ToString());

      XmlSchemaSet schemas = new XmlSchemaSet();
      schemas.Add (FunctionDeclaration.SchemaUri, FunctionDeclaration.GetSchemaReader());

      XmlReaderSettings settings = new XmlReaderSettings();
      settings.Schemas = schemas;
      settings.ValidationType = ValidationType.Schema;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

      var schemaValidationObject = new SchemaValidationObject (file, xmlfFragmentContext.FirstLineNumber, xmlfFragmentContext.Indents);
      settings.ValidationEventHandler += schemaValidationObject.CreateValidationHandler();

      XmlReader xmlReader = XmlReader.Create (stringReader, settings, file.FullName);
      XmlSerializer serializer = new XmlSerializer (typeof (FunctionDeclaration), FunctionDeclaration.SchemaUri);

      try
      {
        FunctionDeclaration declaration = (FunctionDeclaration) serializer.Deserialize (xmlReader);

        if (schemaValidationObject.HasFailed)
          return null;
        else
          return declaration;
      }
      catch (InvalidOperationException e)
      {
        XmlException xmlException = e.InnerException as XmlException;
        if (xmlException != null)
        {
          throw new InputException (
              InputError.XmlError,
              file.FullName,
              xmlException.LineNumber + xmlfFragmentContext.FirstLineNumber - 1,
              xmlException.LinePosition + xmlfFragmentContext.Indents[xmlException.LineNumber - 1],
              xmlException);
        }
        else
        {
          throw;
        }
      }
    }

    private void ProcessClassDeclarationLine (string lineArgument, SeparatedStringBuilder currentNamespace, FunctionDeclaration declaration)
    {
      if (declaration != null && string.IsNullOrEmpty (declaration.PageType))
      {
        string type = lineArgument;
        if (currentNamespace.Length > 0)
          type = currentNamespace + "." + type;
        declaration.PageType = type;
      }
    }

    private void GenerateClass (FileInfo file, List<string> importNamespaces, FunctionDeclaration declaration, CodeCompileUnit unit, int firstLineNumber)
    {
      var generator = new PageFunctionGenerator (unit, declaration, importNamespaces, file, firstLineNumber);
      generator.GenerateClass ();
    }
  }
}