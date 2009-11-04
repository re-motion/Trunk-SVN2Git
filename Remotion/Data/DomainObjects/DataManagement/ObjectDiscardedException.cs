// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
/// <summary>
/// The exception that is thrown when properties or methods of a discarded object are accessed.
/// </summary>
/// <remarks>
/// A <see cref="DomainObject"/> is discarded in one of the following situations:
/// <list type="buttons">
///   <description>
///     A new <see cref="DomainObject"/> is created and the <see cref="ClientTransaction"/> has been rolled back.
///   </description>
///   <description>
///     A new <see cref="DomainObject"/> is created and deleted.
///   </description>
///   <description>
///     An existing <see cref="DomainObject"/> is deleted and the <see cref="ClientTransaction"/> has been committed.
///   </description>
/// </list>
/// All objects that are associated with the discarded <see cref="DomainObject"/> are discarded too. These objects are:
/// <list type="buttons">
///   <description>
///     The <see cref="DataContainer"/> associated with the <see cref="DomainObject"/>.
///   </description>
///   <description>
///     The <see cref="PropertyValueCollection"/> holding the properties of of the <see cref="DomainObject"/>.
///   </description>
///   <description>
///     Each <see cref="PropertyValue"/> of the <see cref="DomainObject"/>.
///   </description>
/// </list>
/// </remarks>
[Serializable]
public class ObjectDiscardedException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private ObjectID _id;

  // construction and disposing

  public ObjectDiscardedException () : this ("Object is already discarded.") 
  {
  }

  public ObjectDiscardedException (string message) : base (message) 
  {
  }
  
  public ObjectDiscardedException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected ObjectDiscardedException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _id = (ObjectID) info.GetValue ("ID", typeof (ObjectID));
  }

  public ObjectDiscardedException (ObjectID id) : this (string.Format ("Object '{0}' is already discarded.", id), id)
  {
  }

  public ObjectDiscardedException (string message, ObjectID id) : base (message) 
  {
    ArgumentUtility.CheckNotNull ("id", id);

    _id = id;
  }

  // methods and properties

  /// <summary>
  /// The <see cref="ObjectID"/> of the object that caused the exception.
  /// </summary>
  public ObjectID ID
  {
    get { return _id; }
  }

  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("ID", _id);
  }
}
}
