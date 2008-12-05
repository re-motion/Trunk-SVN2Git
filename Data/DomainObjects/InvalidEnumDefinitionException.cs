// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// The exception that is thrown when an enum definition is invalid.
/// </summary>
[Serializable]
public class InvalidEnumDefinitionException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private Type _enumType;

  // construction and disposing

  public InvalidEnumDefinitionException () : this ("Enumeration does not contain any values.") 
  {
  }

  public InvalidEnumDefinitionException (string message) : base (message) 
  {
  }
  
  public InvalidEnumDefinitionException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected InvalidEnumDefinitionException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _enumType = (Type) info.GetValue ("EnumType", typeof (Type));
  }

  public InvalidEnumDefinitionException (Type enumType) : this (
      string.Format ("Enumeration '{0}' does not contain any values.", enumType.FullName), enumType)
  {
  }

  public InvalidEnumDefinitionException (string message, Type enumType) : base (message) 
  {
    ArgumentUtility.CheckNotNull ("enumType", enumType);

    _enumType = enumType;
  }

  // methods and properties

  /// <summary>
  /// The type of the enum that is invalid.
  /// </summary>
  public Type EnumType
  {
    get { return _enumType; }
  }

  /// <summary>
  /// Sets the SerializationInfo object with the parameter name and additional exception information.
  /// </summary>
  /// <param name="info">The object that holds the serialized object data.</param>
  /// <param name="context">The contextual information about the source or destination.</param>
  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("EnumType", _enumType);
  }
}
}
