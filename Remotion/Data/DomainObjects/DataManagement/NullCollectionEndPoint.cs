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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a collection end point (with a specific <see cref="RelationEndPointDefinition"/>) for a <see langword="null"/> object.
  /// </summary>
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

    public override IRelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject)
    {
      return new NullEndPointModification (this, removedRelatedObject, null);
    }

    public override IRelationEndPointModification CreateInsertModification (DomainObject insertedRelatedObject, int index)
    {
      return new NullEndPointModification (this, null, insertedRelatedObject);
    }

    public override IRelationEndPointModification CreateAddModification (DomainObject addedRelatedObject)
    {
      return new NullEndPointModification (this, null, addedRelatedObject);
    }

    public override IRelationEndPointModification CreateReplaceModification (int index, DomainObject replacementObject)
    { 
      return new NullEndPointModification (this, null, replacementObject);
    }

    public override void NotifyClientTransactionOfBeginRelationChange (DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
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
