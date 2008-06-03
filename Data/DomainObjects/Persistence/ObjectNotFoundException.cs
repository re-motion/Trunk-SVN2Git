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
