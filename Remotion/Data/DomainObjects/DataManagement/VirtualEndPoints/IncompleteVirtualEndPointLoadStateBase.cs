﻿using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints
{
  /// <summary>
  /// Defines common logic for <see cref="IVirtualEndPoint"/> implementations in incomplete state, ie., before lazy loading has completed.
  /// </summary>
  /// <typeparam name="TEndPoint">The type of the end point whose state is managed by this class.</typeparam>
  /// <typeparam name="TData">The type of data held by the <typeparamref name="TDataKeeper"/>.</typeparam>
  /// <typeparam name="TDataKeeper">The type of data keeper holding the data for the end-point.</typeparam>
  public abstract class IncompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataKeeper> 
      : IVirtualEndPointLoadState<TEndPoint, TData, TDataKeeper>
      where TEndPoint : IVirtualEndPoint<TData>
      where TDataKeeper : IVirtualEndPointDataKeeper
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (IncompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataKeeper>));

    private readonly TDataKeeper _dataKeeper;
    private readonly IRelationEndPointLazyLoader _lazyLoader;
    private readonly IVirtualEndPointDataKeeperFactory<TDataKeeper> _dataKeeperFactory;

    protected IncompleteVirtualEndPointLoadStateBase (
        TDataKeeper dataKeeper,
        IRelationEndPointLazyLoader lazyLoader,
        IVirtualEndPointDataKeeperFactory<TDataKeeper> dataKeeperFactory)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("dataKeeperFactory", dataKeeperFactory);

      // TODO 3818: Throw if dataKeeper.HasDataChanged => incomplete state with changed data is currently not supported

      // TODO 3818: Remove _dataKeeper field, replace with Dictionary<ObjectID, IRealObjectEndPoint> _originalOppositeEndPoints
      _dataKeeper = dataKeeper;
      _lazyLoader = lazyLoader;
      _dataKeeperFactory = dataKeeperFactory;
    }

    // TODO 3818: Remove, use _originalOppositeEndPoints instead
    protected abstract IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ();

    public static ILog Log
    {
      get { return s_log; }
    }

    public TDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IVirtualEndPointDataKeeperFactory<TDataKeeper> DataKeeperFactory
    {
      get { return _dataKeeperFactory; }
    }

    public bool IsDataComplete ()
    {
      return false;
    }

    public void EnsureDataComplete (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      _lazyLoader.LoadLazyVirtualEndPoint (endPoint);
    }

    public void MarkDataIncomplete (TEndPoint endPoint, Action<TDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      throw new InvalidOperationException ("The data is already incomplete.");
    }

    public virtual TData GetData (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      endPoint.EnsureDataComplete ();
      return endPoint.GetData();
    }

    public virtual TData GetOriginalData (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      endPoint.EnsureDataComplete ();
      return endPoint.GetOriginalData();
    }
 
    public void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      // TODO 3818: add to _originalOppositeEndPoints
      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.ResetSyncState ();
    }

    public void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      // TODO 3818: remove from _originalOppositeEndPoints; throw if not found
      _dataKeeper.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public void RegisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      endPoint.EnsureDataComplete ();
      endPoint.RegisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      endPoint.EnsureDataComplete ();
      endPoint.UnregisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public bool IsSynchronized (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      endPoint.EnsureDataComplete ();
      return endPoint.IsSynchronized;
    }

    public void Synchronize (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      endPoint.EnsureDataComplete ();
      endPoint.Synchronize ();
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      throw new InvalidOperationException ("Cannot synchronize an opposite end-point with a collection end-point in incomplete state.");
    }

    public void SetValueFrom (TEndPoint endPoint, TEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      endPoint.EnsureDataComplete ();
      endPoint.SetValueFrom (sourceEndPoint);
    }

    public bool HasChanged ()
    {
      // TODO 3818: Return false
      return _dataKeeper.HasDataChanged ();
    }

    public void Commit ()
    {
      // TODO 3818: nop
      _dataKeeper.Commit ();
    }

    public void Rollback ()
    {
      // TODO 3818: nop
      _dataKeeper.Rollback ();
    }

    protected void MarkDataComplete (TEndPoint endPoint, IEnumerable<DomainObject> items, Action<TDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      Assertion.IsFalse (
          _dataKeeper.HasDataChanged (),
          "When it is allowed to have a changed collection in incomplete state, this algorithm must be rewritten.");

      if (s_log.IsInfoEnabled)
        s_log.InfoFormat ("CollectionEndPoint '{0}' is transitioned to complete state.", endPoint.ID);

      var newDataKeeper = _dataKeeperFactory.Create (endPoint.ID);
      var originalOppositeEndPoints = GetOriginalOppositeEndPoints ().ToDictionary (ep => ep.ObjectID);

      foreach (var item in items)
      {
        IRealObjectEndPoint oppositeEndPoint;
        if (originalOppositeEndPoints.TryGetValue (item.ID, out oppositeEndPoint))
        {
          newDataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
          oppositeEndPoint.MarkSynchronized ();
          originalOppositeEndPoints.Remove (item.ID);
        }
        else
        {
          newDataKeeper.RegisterOriginalItemWithoutEndPoint (item);

          if (s_log.IsWarnEnabled)
          {
            s_log.WarnFormat ("CollectionEndPoint '{0}' contains an item without an opposite end-point: '{1}'. The CollectionEndPoint is out-of-sync.",
                              endPoint.ID,
                              item.ID);
          }
        }
      }

      stateSetter (newDataKeeper);

      foreach (var oppositeEndPointWithoutItem in originalOppositeEndPoints.Values)
        endPoint.RegisterOriginalOppositeEndPoint (oppositeEndPointWithoutItem);
    }

    #region Serialization

    protected IncompleteVirtualEndPointLoadStateBase (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader> ();
      // TODO 3818: serialize _originalOppositeEndPoints instead of _dataKeeper
      _dataKeeper = info.GetValueForHandle<TDataKeeper> ();
      _dataKeeperFactory = info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<TDataKeeper>> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_lazyLoader);
      // TODO 3818: serialize _originalOppositeEndPoints instead of _dataKeeper
      info.AddHandle (_dataKeeper);
      info.AddHandle (_dataKeeperFactory);
    }

    #endregion
  }
}