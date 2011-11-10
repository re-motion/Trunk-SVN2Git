// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// The exception that is thrown when a domain object property is set with a value that is exceeds the <see cref="PropertyDefinition.MaxLength"/> of 
/// the property.
/// </summary>
[Serializable]
public class ValueTooLongException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private readonly string _propertyName;
  private readonly int _maxLength;

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
