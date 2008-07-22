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
using System.Runtime.Serialization;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  /// <summary>
  /// Thrown when there is an error in the mixin configuration which is detected during validation of the configuration. The problem prevents
  /// code being generated from the configuration. See also <see cref="ConfigurationException"/>.
  /// </summary>
  [Serializable]
  public class ValidationException : Exception
  {
    private static string BuildExceptionString (IValidationLog log)
    {
      ArgumentUtility.CheckNotNull ("log", log);

      StringBuilder sb = new StringBuilder ("Some parts of the mixin configuration could not be validated.");
      foreach (ValidationResult item in log.GetResults ())
      {
        if (item.TotalRulesExecuted != item.Successes.Count)
        {
          sb.Append (Environment.NewLine).Append (item.Definition.FullName);
          string parentString = item.GetParentDefinitionString ();
          if (parentString.Length > 0)
            sb.Append (" (").Append (parentString).Append ("):").Append (Environment.NewLine);

          foreach (ValidationExceptionResultItem exception in item.Exceptions)
            sb.Append ("Internal exception: ").Append (exception.Message).Append (Environment.NewLine);

          foreach (ValidationResultItem failure in item.Failures)
            sb.Append ("Error: ").Append (failure.Message).Append (Environment.NewLine);

          foreach (ValidationResultItem warning in item.Warnings)
            sb.Append ("Warning: ").Append (warning.Message).Append (Environment.NewLine);
        }
      }
      return sb.ToString ();
    }

    private readonly IValidationLog _validationLog;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="log">The validation log.</param>
    public ValidationException (string message, IValidationLog log)
        : base (message)
    {
      _validationLog = log;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class and creates a descriptive message from the validation log.
    /// </summary>
    /// <param name="log">The validation log log.</param>
    /// <exception cref="ArgumentNullException">The log is empty.</exception>
    public ValidationException (IValidationLog log)
      : this (BuildExceptionString (log), log)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class during deserialization.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
    protected ValidationException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _validationLog = (IValidationLog) info.GetValue ("ValidationLog", typeof (IValidationLog));
    }

    public IValidationLog ValidationLog
    {
      get { return _validationLog; }
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);
      info.AddValue ("ValidationLog", _validationLog);
    }
  }
}
