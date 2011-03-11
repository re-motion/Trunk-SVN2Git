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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents an <see cref="ICollectionEndPoint"/> (with a specific <see cref="RelationEndPointDefinition"/>) for a <see langword="null"/> object.
  /// This is used by the different end point modification commands - when a bidirectional relation modification extends to a <see langword="null"/> 
  /// object, this end point (or <see cref="NullObjectEndPoint"/>) is used to represent the object's part in the relation, and a 
  /// <see cref="NullEndPointModificationCommand"/> is used to represent the modification. The end point is created on the fly by 
  /// <see cref="RelationEndPointMap.GetRelationEndPointWithLazyLoad"/> and is usually discarded after it's used.
  /// </summary>
  public class NullCollectionEndPoint : ICollectionEndPoint
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointDefinition _definition;

    public NullCollectionEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("definition", definition);

      _clientTransaction = clientTransaction;
      _definition = definition;
    }

    public RelationEndPointID ID
    {
      get { return RelationEndPointID.Create(null, Definition); }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public ObjectID ObjectID
    {
      get { return null; }
    }

    public IRelationEndPointDefinition Definition
    {
      get { return _definition; }
    }

    public RelationDefinition RelationDefinition
    {
      get { return Definition.RelationDefinition; }
    }

    public bool IsDataComplete
    {
      get { return true; }
    }

    public bool HasChanged
    {
      get { return false; }
    }

    public bool HasBeenTouched
    {
      get { return false; }
    }

    public DomainObject GetDomainObject ()
    {
      return null;
    }

    public DomainObject GetDomainObjectReference ()
    {
      return null;
    }

    public bool IsNull
    {
      get { return true; }
    }

    public void CheckMandatory ()
    {
      throw new InvalidOperationException ("CheckMandatory cannot be called on a NullCollectionEndPoint.");
    }

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      throw new InvalidOperationException ("GetOppositeRelationEndPointIDs cannot be called on a NullCollectionEndPoint.");
    }

    public void SetValueFrom (IRelationEndPoint source)
    {
      throw new InvalidOperationException ("SetValueFrom cannot be called on a NullCollectionEndPoint.");
    }

    public DomainObjectCollection Collection
    {
      get { return new DomainObjectCollection (); }
    }

    public DomainObjectCollection OriginalCollection
    {
      get { throw new InvalidOperationException ("It is not possible to get the OriginalCollection from a NullCollectionEndPoint."); }
    }

    public IDomainObjectCollectionData GetCollectionData ()
    {
      throw new InvalidOperationException ("It is not possible to call GetCollectionData on a NullCollectionEndPoint.");
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
      throw new InvalidOperationException ("It is not possible to call GetCollectionWithOriginalData on a NullCollectionEndPoint.");
    }

    public void MarkDataComplete (DomainObject[] items)
    {
      // ignore
    }

    public void MarkDataIncomplete ()
    {
      throw new InvalidOperationException ("MarkDataIncomplete cannot be called on a NullCollectionEndPoint.");
    }

    public IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection)
    {
      return new NullEndPointModificationCommand (this);
    }

    public IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      return new NullEndPointModificationCommand (this);
    }

    public IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      return new NullEndPointModificationCommand (this);
    }

    public IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    { 
      return new NullEndPointModificationCommand (this);
    }

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      return new NullEndPointModificationCommand (this);
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      return new NullEndPointModificationCommand (this);
    }

    public IDomainObjectCollectionData CreateDelegatingCollectionData ()
    {
      throw new InvalidOperationException ("CreateDelegatingCollectionData cannot be called on a NullCollectionEndPoint.");
    }

    public void RegisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      throw new InvalidOperationException ("RegisterOriginalOppositeEndPoint cannot be called on a NullCollectionEndPoint.");
    }

    public void UnregisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      throw new InvalidOperationException ("UnregisterOriginalOppositeEndPoint cannot be called on a NullCollectionEndPoint.");
    }

    public bool IsSynchronized
    {
      get { return true; }
    }

    public void Synchronize ()
    {
      throw new InvalidOperationException ("Synchronize cannot be called on a NullCollectionEndPoint.");
    }

    public void SynchronizeOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      throw new InvalidOperationException ("SynchronizeOppositeEndPoint cannot be called on a NullCollectionEndPoint.");
    }

    public void EnsureDataComplete ()
    {
      // do nothing
    }

    public void Touch ()
    {
      // do nothing
    }

    public void Commit ()
    {
      throw new InvalidOperationException ("Commit cannot be called on a NullCollectionEndPoint.");
    }

    public void Rollback ()
    {
      throw new InvalidOperationException ("Rollback cannot be called on a NullCollectionEndPoint.");
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      throw new InvalidOperationException ("SerializeIntoFlatStructure cannot be called on a NullCollectionEndPoint.");
    }
  }
}
