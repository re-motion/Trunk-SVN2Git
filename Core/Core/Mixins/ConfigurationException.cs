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
using Remotion.Mixins.Validation;
using System.Runtime.Serialization;

namespace Remotion.Mixins
{
  /// <summary>
  /// Thrown when there is a severe error in the mixin configuration which is detected during configuration analysis. The problem prevents
  /// the configuration from being fully analyzed. See also <see cref="ValidationException"/>.
  /// </summary>
  [Serializable]
  public class ConfigurationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ConfigurationException (string message)
        : base (message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConfigurationException (string message, Exception innerException)
        : base (message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class during deserialization.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
    protected ConfigurationException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}
