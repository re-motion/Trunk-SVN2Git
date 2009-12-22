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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents an <see cref="IObjectEndPoint"/> (with a specific <see cref="RelationEndPointDefinition"/>) for a <see langword="null"/> object.
  /// This is used by the different end point modification commands - when a bidirectional relation modification extends to a <see langword="null"/> 
  /// object, this end point (or <see cref="NullCollectionEndPoint"/>) is used to represent the object's part in the relation, and a 
  /// <see cref="NullEndPointModification"/> is used to represent the modification. The end point is created by 
  /// <see cref="RelationEndPoint.CreateNullRelationEndPoint"/> (e.g. via 
  /// <see cref="EndPointExtensions.GetEndPointWithOppositeDefinition{T}(Remotion.Data.DomainObjects.DataManagement.IEndPoint,Remotion.Data.DomainObjects.DomainObject)"/>)
  /// and is usually discarded after executing the modification.
  /// </summary>
  public class NullObjectEndPoint : IObjectEndPoint
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointDefinition _definition;

    public NullObjectEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("definition", definition);

      _clientTransaction = clientTransaction;
      _definition = definition;
    }

    public IRelationEndPointDefinition Definition
    {
      get { return _definition; }
    }

    public ObjectID ObjectID
    {
      get { return null; }
    }

    public RelationEndPointID ID
    {
      get { return new RelationEndPointID (null, Definition); }
    }

    public RelationDefinition RelationDefinition
    {
      get { return Definition.RelationDefinition; }
    }

    public ObjectID OppositeObjectID
    {
      get { return null; }
      set { throw new InvalidOperationException ("It is not possible to set the OppositeObjectID of a NullObjectEndPoint."); }
    }

    public ObjectID OriginalOppositeObjectID
    {
      get { throw new InvalidOperationException ("It is not possible to get the OriginalOppositeObjectID from a NullObjectEndPoint."); }
    }

    public bool HasChanged
    {
      get { return false; }
    }

    public bool HasBeenTouched
    {
      get { return false; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public bool IsNull
    {
      get { return true; }
    }

    public void SetOppositeObjectAndNotify (DomainObject newRelatedObject)
    {
      throw new InvalidOperationException ("It is not possible to set the OppositeObjectID of a NullObjectEndPoint.");
    }

    public void Touch ()
    {
      // do nothing
    }

    public void Commit ()
    {
      throw new InvalidOperationException ("Commit cannot be called on a NullObjectEndPoint.");
    }

    public void Rollback ()
    {
      throw new InvalidOperationException ("Rollback cannot be called on a NullObjectEndPoint.");
    }
 
    public IRelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject)
    {
      return new NullEndPointModification (this, removedRelatedObject, null);
    }

    public IRelationEndPointModification CreateSetModification (DomainObject newRelatedObject)
    {
      return new NullEndPointModification (this, this.GetOppositeObject (true), newRelatedObject);
    }

    public void NotifyClientTransactionOfBeginRelationChange (DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      // do nothing
    }

    public void NotifyClientTransactionOfEndRelationChange ()
    {
      // do nothing
    }
  }
}