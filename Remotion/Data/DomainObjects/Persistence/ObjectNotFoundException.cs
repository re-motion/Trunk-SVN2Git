// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Data.DomainObjects.Persistence
{
[Serializable]
public class ObjectNotFoundException : StorageProviderException
{
  // types

  // static members and constants

  // member fields

  private ObjectID _id;

  // construction and disposing

  public ObjectNotFoundException () : this ("Object could not be found.") 
  {
  }

  public ObjectNotFoundException (string message) : base (message) 
  {
  }

  public ObjectNotFoundException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected ObjectNotFoundException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _id = (ObjectID) info.GetValue ("ID", typeof (ObjectID));
  }

  public ObjectNotFoundException (ObjectID id, Exception inner) : this (string.Format ("Object '{0}' could not be found.", id), id, inner)
  {
  }

  public ObjectNotFoundException (ObjectID id) : this (id, null)
  {
  }

  public ObjectNotFoundException (string message, ObjectID id) : this (message, id, null) 
  {
  }

  public ObjectNotFoundException (string message, ObjectID id, Exception inner)
    : base (message, inner)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    _id = id;
  }

  // methods and properties

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
