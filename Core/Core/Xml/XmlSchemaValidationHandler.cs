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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using log4net;

namespace Remotion.Xml
{
  public class XmlSchemaValidationHandler
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (XmlSchemaValidationHandler));

    private bool _failOnError;

    private List<XmlSchemaValidationErrorInfo> _messages = new List<XmlSchemaValidationErrorInfo>();

    private int _warnings = 0;
    private int _errors = 0;

    private XmlSchemaValidationErrorInfo _firstError = null;
    private XmlSchemaException _firstException = null;

    public XmlSchemaValidationHandler (bool failOnError)
    {
      _failOnError = failOnError;
    }

    public int Warnings
    {
      get { return _warnings; }
    }

    public int Errors
    {
      get { return _errors; }
    }

    public ValidationEventHandler Handler
    {
      get { return new ValidationEventHandler (HandleValidation); }
    }

    private void HandleValidation (object sender, ValidationEventArgs args)
    {
      XmlReader reader = (XmlReader) sender;

      // WORKAROUND: known bug in .NET framework 1.x
      // TODO: verify for 2.0
      if (args.Message.IndexOf ("http://www.w3.org/XML/1998/namespace:base") >= 0)
      {
        s_log.DebugFormat (
            "Ignoring the following schema validation error in {0}, because it is considered a known .NET framework bug: {1}",
            reader.BaseURI,
            args.Message);
        return;
      }

      IXmlLineInfo lineInfo = sender as IXmlLineInfo;
      XmlSchemaValidationErrorInfo errorInfo = new XmlSchemaValidationErrorInfo (args.Message, reader.BaseURI, lineInfo, args.Severity);
      _messages.Add (errorInfo);

      if (args.Severity == XmlSeverityType.Error)
      {
        s_log.Error (errorInfo);
        ++ _errors;
        if (_failOnError)
          throw args.Exception;

        if (_errors == 0)
        {
          _firstError = errorInfo;
          _firstException = args.Exception;
        }
      }
      else
      {
        s_log.Warn (errorInfo);
        ++ _warnings;
      }
    }

    public XmlSchemaException FirstException
    {
      get { return _firstException; }
    }

    public void EnsureNoErrors()
    {
      if (_errors > 0)
      {
        string lineInfoMessage = string.Empty;
        if (_firstError.HasLineInfo())
          lineInfoMessage = string.Format (" Line {0}, position {1}.", _firstError.LineNumber, _firstError.LinePosition);

        throw _firstException;
        //throw new RemotionXmlSchemaValidationException (
        //    string.Format (
        //        "Schema verification failed with {0} errors and {1} warnings in '{2}'. First error: {3}{4}",
        //        _errors,
        //        _warnings,
        //        _context,
        //        _firstError.ErrorMessage,
        //        lineInfoMessage),
        //    _context,
        //    _firstError.LineNumber,
        //    _firstError.LinePosition,
        //    null);
      }
    }
  }
}
