// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that does not hold the foreign key in a relation. The <see cref="VirtualObjectEndPoint"/> is
  /// constructed by the <see cref="RelationEndPointManager"/> as an in-memory representation of the opposite of the <see cref="RealObjectEndPoint"/> 
  /// holding the foreign key.
  /// </summary>
  public class VirtualObjectEndPoint : ObjectEndPoint, IVirtualObjectEndPoint
  {
    [Serializable]
    public class EndPointLoader : IncompleteVirtualObjectEndPointLoadState.IEndPointLoader
    {
      private readonly ILazyLoader _lazyLoader;

      public EndPointLoader (ILazyLoader lazyLoader)
      {
        ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
        _lazyLoader = lazyLoader;
      }

      public ILazyLoader LazyLoader
      {
        get { return _lazyLoader; }
      }

      public IVirtualObjectEndPointLoadState LoadEndPointAndGetNewState (IVirtualObjectEndPoint endPoint)
      {
        var virtualObjectEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualObjectEndPoint> ("endPoint", endPoint);
        _lazyLoader.LoadLazyVirtualObjectEndPoint (virtualObjectEndPoint);
        return virtualObjectEndPoint._loadState;
      }

      #region Serialization
      public EndPointLoader (FlattenedDeserializationInfo info)
      {
        ArgumentUtility.CheckNotNull ("info", info);

        _lazyLoader = info.GetValueForHandle<ILazyLoader> ();
      }

      void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
      {
        ArgumentUtility.CheckNotNull ("info", info);

        info.AddHandle (_lazyLoader);
      }
      #endregion
    }

    private readonly ILazyLoader _lazyLoader;
    private readonly IVirtualEndPointDataManagerFactory<IVirtualObjectEndPointDataManager> _dataManagerFactory;
    private readonly IVirtualEndPointStateUpdateListener _stateUpdateListener;

    private IVirtualObjectEndPointLoadState _loadState;

    private bool _hasBeenTouched;
    
    public VirtualObjectEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ILazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IVirtualEndPointDataManagerFactory<IVirtualObjectEndPointDataManager> dataManagerFactory,
        IVirtualEndPointStateUpdateListener stateUpdateListener)
        : base (
            ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction),
            ArgumentUtility.CheckNotNull ("id", id),
            ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider))
    {
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("dataManagerFactory", dataManagerFactory);
      ArgumentUtility.CheckNotNull ("stateUpdateListener", stateUpdateListener);

      if (!ID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a virtual end point.", "id");

      _lazyLoader = lazyLoader;
      _dataManagerFactory = dataManagerFactory;
      _stateUpdateListener = stateUpdateListener;

      SetIncompleteState();

      _hasBeenTouched = false;
    }

    public ILazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }
    
    public IVirtualEndPointDataManagerFactory<IVirtualObjectEndPointDataManager> DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public IVirtualEndPointStateUpdateListener StateUpdateListener
    {
      get { return _stateUpdateListener; }
    }

    public override ObjectID OppositeObjectID
    {
      get { return GetOppositeObject (true).GetSafeID(); }
    }

    DomainObject IVirtualEndPoint<DomainObject>.GetData ()
    {
      return GetOppositeObject (true);
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return GetOriginalOppositeObject().GetSafeID(); }
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

    public override bool? IsSynchronized
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
      _stateUpdateListener.VirtualEndPointStateUpdated (ID, HasChanged);
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _loadState.SynchronizeOppositeEndPoint (this, oppositeEndPoint);
      _stateUpdateListener.VirtualEndPointStateUpdated (ID, HasChanged);
    }

    public void MarkDataComplete (DomainObject item)
    {
      _loadState.MarkDataComplete (this, item, SetCompleteState);
    }

    public bool CanBeCollected
    {
      get { return _loadState.CanEndPointBeCollected (this); }
    }

    public bool CanBeMarkedIncomplete
    {
      get { return _loadState.CanDataBeMarkedIncomplete (this); }
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
      var command = _loadState.CreateSetCommand (this, newRelatedObject);
      return CreateStateUpdateRaisingCommandDecorator (command);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      var command = _loadState.CreateDeleteCommand (this);
      return CreateStateUpdateRaisingCommandDecorator (command);
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _loadState.Commit (this);
        _stateUpdateListener.VirtualEndPointStateUpdated (ID, false);
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        _loadState.Rollback (this);
        _stateUpdateListener.VirtualEndPointStateUpdated (ID, false);
      }

      _hasBeenTouched = false;
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      var sourceVirtualObjectEndPoint = ArgumentUtility.CheckNotNullAndType<VirtualObjectEndPoint> ("sourceObjectEndPoint", sourceObjectEndPoint);
      _loadState.SetDataFromSubTransaction (this, sourceVirtualObjectEndPoint._loadState);
      _stateUpdateListener.VirtualEndPointStateUpdated (ID, HasChanged);
    }

    private void SetIncompleteState ()
    {
      var loader = new EndPointLoader (_lazyLoader);
      _loadState = new IncompleteVirtualObjectEndPointLoadState (loader, _dataManagerFactory);
    }

    private void SetCompleteState (IVirtualObjectEndPointDataManager dataManager)
    {
      _loadState = new CompleteVirtualObjectEndPointLoadState (dataManager, EndPointProvider, ClientTransaction);
    }

    private IDataManagementCommand CreateStateUpdateRaisingCommandDecorator (IDataManagementCommand command)
    {
      return new VirtualEndPointStateUpdatedRaisingCommandDecorator (command, ID, _stateUpdateListener, () => HasChanged);
    }

    #region Serialization

    protected VirtualObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _lazyLoader = info.GetValueForHandle<ILazyLoader> ();
      _dataManagerFactory = info.GetValueForHandle<IVirtualEndPointDataManagerFactory<IVirtualObjectEndPointDataManager>> ();
      _stateUpdateListener = info.GetValueForHandle<IVirtualEndPointStateUpdateListener>();
      
      _loadState = info.GetValue<IVirtualObjectEndPointLoadState> ();
      _hasBeenTouched = info.GetBoolValue ();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      base.SerializeIntoFlatStructure (info);

      info.AddHandle (_lazyLoader);
      info.AddHandle (_dataManagerFactory);
      info.AddHandle (_stateUpdateListener);

      info.AddValue (_loadState);
      info.AddBoolValue (_hasBeenTouched);
    }

    #endregion
  }
}