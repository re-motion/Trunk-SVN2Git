using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure;
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
      where TEndPoint : IVirtualEndPoint
      where TDataKeeper : IVirtualEndPointDataKeeper
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (LoggingClientTransactionListener));

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

      _dataKeeper = dataKeeper;
      _lazyLoader = lazyLoader;
      _dataKeeperFactory = dataKeeperFactory;

      // TODO 3816: Consider moving to CompleteVirtualEndPointStateBase.MarkDataIncomplete
      ResetSyncStateForAllOriginalOppositeEndPoints();
    }

    protected abstract TData GetDataFromEndPoint (TEndPoint endPoint);
    protected abstract TData GetOriginalDataFromEndPoint (TEndPoint endPoint);

    protected abstract IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ();
    protected abstract void ResetSyncStateForAllOriginalOppositeEndPoints ();

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

    public void MarkDataComplete (TEndPoint endPoint, IEnumerable<DomainObject> data, Action<TDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("data", data);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      Assertion.IsFalse (
          _dataKeeper.HasDataChanged (),
          "When it is allowed to have a changed collection in incomplete state, this algorithm must be rewritten.");

      if (s_log.IsInfoEnabled)
        s_log.InfoFormat ("CollectionEndPoint '{0}' is transitioned to complete state.", endPoint.ID);

      var newDataKeeper = _dataKeeperFactory.Create (endPoint.ID);
      var originalOppositeEndPoints = GetOriginalOppositeEndPoints().ToDictionary (ep => ep.ObjectID);

      foreach (var item in data)
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
      return GetDataFromEndPoint (endPoint);
    }

    public virtual TData GetOriginalData (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      endPoint.EnsureDataComplete ();
      return GetOriginalDataFromEndPoint (endPoint);
    }
 
    public void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.ResetSyncState ();
    }

    public void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

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
      return _dataKeeper.HasDataChanged ();
    }

    public void Commit ()
    {
      _dataKeeper.Commit ();
    }

    public void Rollback ()
    {
      _dataKeeper.Rollback ();
    }

    #region Serialization

    protected IncompleteVirtualEndPointLoadStateBase (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader> ();
      _dataKeeper = info.GetValueForHandle<TDataKeeper> ();
      _dataKeeperFactory = info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<TDataKeeper>> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_dataKeeper);
      info.AddHandle (_dataKeeperFactory);
    }

    #endregion
  }
}