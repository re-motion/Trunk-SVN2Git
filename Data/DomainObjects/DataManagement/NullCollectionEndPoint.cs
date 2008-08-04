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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using FlattenedSerializationInfo=Remotion.Data.DomainObjects.Infrastructure.Serialization.FlattenedSerializationInfo;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class NullCollectionEndPoint : CollectionEndPoint
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public NullCollectionEndPoint (IRelationEndPointDefinition definition)
        : base (definition)
    {
    }

    // methods and properties

    public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      return new NullEndPointModification (this, oldEndPoint, newEndPoint);
    }

    public override RelationEndPointModification CreateInsertModification (IEndPoint oldEndPoint, IEndPoint newEndPoint, int index)
    {
      return new NullEndPointModification (this, oldEndPoint, newEndPoint);
    }

    public override RelationEndPointModification CreateReplaceModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      return new NullEndPointModification (this, oldEndPoint, newEndPoint);
    }

    public override void NotifyClientTransactionOfBeginRelationChange (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);
    }

    public override void NotifyClientTransactionOfEndRelationChange ()
    {
    }

    public override void PerformDelete ()
    {
      throw new InvalidOperationException ("PerformDelete cannot be called on a NullCollectionEndPoint.");
    }

    public override void CheckMandatory ()
    {
      throw new InvalidOperationException ("CheckMandatory cannot be called on a NullCollectionEndPoint.");
    }

    public override void Commit ()
    {
      throw new InvalidOperationException ("Commit cannot be called on a NullCollectionEndPoint.");
    }

    public override void Rollback ()
    {
      throw new InvalidOperationException ("Rollback cannot be called on a NullCollectionEndPoint.");
    }

    public override bool HasChanged
    {
      get { return false; }
    }

    public override DataContainer GetDataContainer ()
    {
      return null;
    }

    public override DomainObject GetDomainObject ()
    {
      return null;
    }

    public override ObjectID ObjectID
    {
      get { return null; }
    }

    public override RelationEndPointID ID
    {
      get { return null; }
    }

    public override bool IsNull
    {
      get { return true; }
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      throw new InvalidOperationException ("SerializeIntoFlatStructure cannot be called on a NullCollectionEndPoint.");
    }
  }
}
