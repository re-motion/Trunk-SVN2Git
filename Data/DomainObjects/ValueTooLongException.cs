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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// The exception that is thrown when a <see cref="PropertyValue"/> is set with a value that is exceeds the <see cref="PropertyValue.MaxLength"/> of the property.
/// </summary>
[Serializable]
public class ValueTooLongException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private int _maxLength;

  // construction and disposing

  public ValueTooLongException () : this ("Property value too long.") 
  {
  }

  public ValueTooLongException (string message) : base (message) 
  {
  }
  
  public ValueTooLongException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected ValueTooLongException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _propertyName = info.GetString ("PropertyName");
    _maxLength = info.GetInt32 ("MaxLength");
  }

  public ValueTooLongException (string message, string propertyName, int maxLength) : base (message) 
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _propertyName = propertyName;
    _maxLength = maxLength;
  }

  // methods and properties

  /// <summary>
  /// Gets the PropertyName that was set with a value exceeding the maximum length.
  /// </summary>
  public string PropertyName
  {
    get { return _propertyName; }
  }

  /// <summary>
  /// Gets the maximum length of the property.
  /// </summary>
  public int MaxLength
  {
    get { return _maxLength; }
  }

  /// <summary>
  /// Sets the SerializationInfo object with the parameter name and additional exception information.
  /// </summary>
  /// <param name="info">The object that holds the serialized object data.</param>
  /// <param name="context">The contextual information about the source or destination.</param>
  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("PropertyName", _propertyName);
    info.AddValue ("MaxLength", _maxLength);
  }
}
}
