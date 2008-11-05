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
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class SchemaValidationObject
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
}