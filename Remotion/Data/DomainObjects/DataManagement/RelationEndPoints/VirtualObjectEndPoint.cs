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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that does not hold the foreign key in a relation. The <see cref="VirtualObjectEndPoint"/> is
  /// constructed by the <see cref="RelationEndPointMap"/> as an in-memory representation of the opposite of the <see cref="RealObjectEndPoint"/> 
  /// holding the foreign key.
  /// </summary>
  public class VirtualObjectEndPoint : ObjectEndPoint, IVirtualObjectEndPoint
  {
    private readonly IRelationEndPointLazyLoader _lazyLoader;
    private readonly IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _dataKeeperFactory;

    private IVirtualObjectEndPointLoadState _loadState;

    private bool _hasBeenTouched;
    
    public VirtualObjectEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        IRelationEndPointLazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> dataKeeperFactory)
        : base (
            ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction),
            ArgumentUtility.CheckNotNull ("id", id),
            ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider))
    {
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("dataKeeperFactory", dataKeeperFactory);

      if (!ID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a virtual end point.", "id");

      _lazyLoader = lazyLoader;
      _dataKeeperFactory = dataKeeperFactory;

      var dataKeeper = _dataKeeperFactory.Create (ID);
      SetIncompleteState(dataKeeper);

      _hasBeenTouched = false;
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }
    
    public IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> DataKeeperFactory
    {
      get { return _dataKeeperFactory; }
    }

    public override ObjectID OppositeObjectID
    {
      get { return DomainObject.GetIDOrNull (GetOppositeObject (true)); }
    }

    DomainObject IVirtualEndPoint<DomainObject>.GetData ()
    {
      return GetOppositeObject (true);
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return DomainObject.GetIDOrNull (GetOriginalOppositeObject()); }
    }

    DomainObject IVirtualEndPoint<DomainObject>.GetOriginalData ()
    {
      return GetOriginalOppositeObject();
    }

    public override bool HasChanged
    {
      get { return _loadState.HasChanged(); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override bool IsDataComplete
    {
      get { return _loadState.IsDataComplete(); }
    }

    public override bool IsSynchronized
    {
      get { return _loadState.IsSynchronized (this); }
    }

    public override DomainObject GetOppositeObject (bool includeDeleted)
    {
      return _loadState.GetData (this);
    }

    public override DomainObject GetOriginalOppositeObject ()
    {
      return _loadState.GetOriginalData (this);
    }

    public override void EnsureDataComplete ()
    {
      _loadState.EnsureDataComplete (this);
    }

    public override void Synchronize ()
    {
      _loadState.Synchronize (this);
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);
      _loadState.SynchronizeOppositeEndPoint (oppositeEndPoint);
    }

    public void MarkDataComplete (DomainObject item)
    {
      _loadState.MarkDataComplete (this, item, SetCompleteState);
    }

    public bool CanBeCollected
    {
      get { return _loadState.GetCanEndPointBeCollected (this); }
    }

    public void MarkDataIncomplete ()
    {
      _loadState.MarkDataIncomplete (this, SetIncompleteState);
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);
      _loadState.RegisterOriginalOppositeEndPoint (this, oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);
      _loadState.UnregisterOriginalOppositeEndPoint (this, oppositeEndPoint);
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);
      _loadState.RegisterCurrentOppositeEndPoint (this, oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);
      _loadState.UnregisterCurrentOppositeEndPoint (this, oppositeEndPoint);
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      return _loadState.CreateSetCommand (this, newRelatedObject);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return _loadState.CreateDeleteCommand (this);
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      if (HasChanged)
        _loadState.Commit();

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      _loadState.Rollback();
      _hasBeenTouched = false;
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      var sourceVirtualObjectEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualObjectEndPoint> ("sourceObjectEndPoint", sourceObjectEndPoint);
      _loadState.SetDataFromSubTransaction (this, sourceVirtualObjectEndPoint._loadState);
    }

    private void SetIncompleteState (IVirtualObjectEndPointDataKeeper dataKeeper)
    {
      _loadState = new IncompleteVirtualObjectEndPointLoadState (dataKeeper, LazyLoader, _dataKeeperFactory);
    }

    private void SetCompleteState (IVirtualObjectEndPointDataKeeper dataKeeper)
    {
      _loadState = new CompleteVirtualObjectEndPointLoadState (dataKeeper, EndPointProvider, ClientTransaction);
    }

    #region Serialization

    protected VirtualObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader> ();
      _dataKeeperFactory = info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>> ();
      _loadState = info.GetValue<IVirtualObjectEndPointLoadState> ();

      _hasBeenTouched = info.GetBoolValue ();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      base.SerializeIntoFlatStructure (info);

      info.AddHandle (_lazyLoader);
      info.AddHandle (_dataKeeperFactory);
      info.AddValue (_loadState);

      info.AddBoolValue (_hasBeenTouched);
    }

    #endregion
  }
}