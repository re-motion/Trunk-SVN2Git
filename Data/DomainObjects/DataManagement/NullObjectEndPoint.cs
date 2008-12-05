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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using FlattenedSerializationInfo=Remotion.Data.DomainObjects.Infrastructure.Serialization.FlattenedSerializationInfo;

namespace Remotion.Data.DomainObjects.DataManagement
{
public class NullObjectEndPoint : ObjectEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullObjectEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
  }

  // methods and properties

  public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
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
    throw new InvalidOperationException ("PerformDelete cannot be called on a NullObjectEndPoint.");    
  }

  public override void CheckMandatory ()
  {
    throw new InvalidOperationException ("CheckMandatory cannot be called on a NullObjectEndPoint.");    
  }

  public override void Commit ()
  {
    throw new InvalidOperationException ("Commit cannot be called on a NullObjectEndPoint.");    
  }

  public override void Rollback ()
  {
    throw new InvalidOperationException ("Rollback cannot be called on a NullObjectEndPoint.");
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
    throw new InvalidOperationException ("Rollback cannot be called on a NullCollectionEndPoint.");
  }
}
}
