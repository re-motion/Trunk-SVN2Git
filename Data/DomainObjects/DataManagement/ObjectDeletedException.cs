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

namespace Remotion.Data.DomainObjects.DataManagement
{
[Serializable]
public class ObjectDeletedException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private ObjectID _id;

  // construction and disposing

  public ObjectDeletedException () : this ("Object is already deleted.") 
  {
  }

  public ObjectDeletedException (string message) : base (message) 
  {
  }
  
  public ObjectDeletedException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected ObjectDeletedException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _id = (ObjectID) info.GetValue ("ID", typeof (ObjectID));
  }

  public ObjectDeletedException (ObjectID id) : this (string.Format ("Object '{0}' is already deleted.", id), id)
  {
  }

  public ObjectDeletedException (string message, ObjectID id) : base (message) 
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
