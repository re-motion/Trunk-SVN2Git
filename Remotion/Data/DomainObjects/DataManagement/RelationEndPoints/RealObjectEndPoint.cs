// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that holds the foreign key in a relation. The foreign key is actually held by a 
  /// <see cref="PropertyValue"/> object, this end point implementation just delegates to the <see cref="PropertyValue"/>.
  /// </summary>
  public class RealObjectEndPoint : ObjectEndPoint, IRealObjectEndPoint
  {
    private static PropertyValue GetForeignKeyProperty (DataContainer foreignKeyDataContainer, string propertyName)
    {
      PropertyValue foreignKeyProperty;
      try
      {
        foreignKeyProperty = foreignKeyDataContainer.PropertyValues[propertyName];
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("The foreign key data container must be compatible with the end point definition.", "foreignKeyDataContainer", ex);
      }

      Assertion.IsTrue (foreignKeyProperty.Definition.IsObjectID, "The foreign key property must have a property type of ObjectID.");
      return foreignKeyProperty;
    }

    private readonly DataContainer _foreignKeyDataContainer;
    private readonly PropertyValue _foreignKeyProperty;

    private IRealObjectEndPointSyncState _syncState; // keeps track of whether this end-point is synchronised with the opposite end point

    public RealObjectEndPoint (
        ClientTransaction clientTransaction, 
        RelationEndPointID id,
        DataContainer foreignKeyDataContainer,
        IRelationEndPointProvider endPointProvider)
      : base (
          ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction),
          ArgumentUtility.CheckNotNull ("id", id),
          ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider))
    {
      ArgumentUtility.CheckNotNull ("foreignKeyDataContainer", foreignKeyDataContainer);

      if (ID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a non-virtual end point.", "id");

      _foreignKeyDataContainer = foreignKeyDataContainer;
      _foreignKeyProperty = GetForeignKeyProperty (_foreignKeyDataContainer, PropertyName);
      _syncState = new UnknownRealObjectEndPointSyncState (EndPointProvider);
    }

    public DataContainer ForeignKeyDataContainer
    {
      get { return _foreignKeyDataContainer; }
    }

    public PropertyValue ForeignKeyProperty
    {
      get { return _foreignKeyProperty; }
    }

    public override ObjectID OppositeObjectID
    {
      get { return (ObjectID) ForeignKeyProperty.GetValueWithoutEvents (ValueAccess.Current); }
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return (ObjectID) ForeignKeyProperty.GetValueWithoutEvents (ValueAccess.Original); }
    }

    public override bool HasChanged
    {
      get { return ForeignKeyProperty.HasChanged; }
    }

    public override bool HasBeenTouched
    {
      get { return ForeignKeyProperty.HasBeenTouched; }
    }

    public override bool IsDataComplete
    {
      get { return true; }
    }

    public override bool? IsSynchronized
    {
      get { return _syncState.IsSynchronized (this); }
    }

    public override DomainObject GetOppositeObject (bool includeDeleted)
    {
      if (OppositeObjectID == null)
        return null;
      else if (includeDeleted && ClientTransaction.IsInvalid (OppositeObjectID))
        return ClientTransaction.GetInvalidObjectReference (OppositeObjectID);
      else
        return ClientTransaction.GetObject (OppositeObjectID, includeDeleted);
    }

    public override DomainObject GetOriginalOppositeObject ()
    {
      if (OriginalOppositeObjectID == null)
        return null;

      return ClientTransaction.GetObject (OriginalOppositeObjectID, true);
    }

    public override void EnsureDataComplete ()
    {
      Assertion.IsTrue (IsDataComplete);
    }

    public override void Synchronize ()
    {
      var oppositeID = RelationEndPointID.CreateOpposite (Definition, OppositeObjectID);
      var oppositeEndPoint = (IVirtualEndPoint) EndPointProvider.GetRelationEndPointWithLazyLoad (oppositeID);
      _syncState.Synchronize (this, oppositeEndPoint);
    }

    public void MarkSynchronized ()
    {
      _syncState = new SynchronizedRealObjectEndPointSyncState (EndPointProvider);
    }

    public void MarkUnsynchronized ()
    {
      _syncState = new UnsynchronizedRealObjectEndPointSyncState ();
    }

    public void ResetSyncState ()
    {
      _syncState = new UnknownRealObjectEndPointSyncState (EndPointProvider);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return _syncState.CreateDeleteCommand (this, () => SetOppositeObjectID (null));
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      return _syncState.CreateSetCommand (this, newRelatedObject, domainObject => SetOppositeObjectID (domainObject.GetSafeID()));
    }

    public override void Touch ()
    {
      ForeignKeyProperty.Touch ();
      Assertion.IsTrue (HasBeenTouched);
    }

    public override void Commit ()
    {
      ForeignKeyProperty.CommitState ();
      Assertion.IsFalse (HasBeenTouched);
      Assertion.IsFalse (HasChanged);
    }

    public override void Rollback ()
    {
      ForeignKeyProperty.RollbackState ();
      Assertion.IsFalse (HasBeenTouched);
      Assertion.IsFalse (HasChanged);
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      ArgumentUtility.CheckNotNull ("sourceObjectEndPoint", sourceObjectEndPoint);

      SetOppositeObjectID (sourceObjectEndPoint.OppositeObjectID);
    }

    private void SetOppositeObjectID (ObjectID value)
    {
      _foreignKeyProperty.Value = value; // TODO 1925: This is with events, which is a little inconsistent to OppositeObjectID
    }


    #region Serialization
    protected RealObjectEndPoint (FlattenedDeserializationInfo info)
      : base (info)
    {
      _foreignKeyDataContainer = info.GetValueForHandle<DataContainer> ();
      _foreignKeyProperty = GetForeignKeyProperty (_foreignKeyDataContainer, PropertyName);
      _syncState = info.GetValueForHandle<IRealObjectEndPointSyncState> ();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      base.SerializeIntoFlatStructure (info);
      info.AddHandle (_foreignKeyDataContainer);
      info.AddHandle (_syncState);
    }
    #endregion

  }
}