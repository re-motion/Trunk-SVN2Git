using System;
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

    public static ILog Log
    {
      get { return s_log; }
    }

    private readonly IRelationEndPointLazyLoader _lazyLoader;
    private readonly IVirtualEndPointDataKeeperFactory<TDataKeeper> _dataKeeperFactory;
    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _originalOppositeEndPoints;

    protected IncompleteVirtualEndPointLoadStateBase (
        IEnumerable<IRealObjectEndPoint> originalOppositeEndPoints,
        IRelationEndPointLazyLoader lazyLoader,
        IVirtualEndPointDataKeeperFactory<TDataKeeper> dataKeeperFactory)
    {
      ArgumentUtility.CheckNotNull ("originalOppositeEndPoints", originalOppositeEndPoints);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("dataKeeperFactory", dataKeeperFactory);

      _originalOppositeEndPoints = originalOppositeEndPoints.ToDictionary(ep=>ep.ObjectID);
      _lazyLoader = lazyLoader;
      _dataKeeperFactory = dataKeeperFactory;
    }

    public abstract void EnsureDataComplete (TEndPoint endPoint);

    public ICollection<IRealObjectEndPoint> OriginalOppositeEndPoints
    {
      get { return _originalOppositeEndPoints.Values; }
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

      _originalOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);

      oppositeEndPoint.ResetSyncState ();
    }

    public void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (!_originalOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
        throw new InvalidOperationException ("The opposite end-point has not been registered.");

      _originalOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
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
      return false; 
    }

    public void Commit ()
    {
      Assertion.IsTrue (!HasChanged());
    }

    public void Rollback ()
    {
      Assertion.IsTrue (!HasChanged ());
    }

    protected void MarkDataComplete (TEndPoint endPoint, IEnumerable<DomainObject> items, Action<TDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      if (s_log.IsInfoEnabled)
        s_log.InfoFormat ("CollectionEndPoint '{0}' is transitioned to complete state.", endPoint.ID);

      var newDataKeeper = _dataKeeperFactory.Create (endPoint.ID);
      
      foreach (var item in items)
      {
        IRealObjectEndPoint oppositeEndPoint;
        if (_originalOppositeEndPoints.TryGetValue (item.ID, out oppositeEndPoint))
        {
          newDataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
          oppositeEndPoint.MarkSynchronized ();
          _originalOppositeEndPoints.Remove (item.ID);
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

      foreach (var oppositeEndPointWithoutItem in _originalOppositeEndPoints.Values)
        endPoint.RegisterOriginalOppositeEndPoint (oppositeEndPointWithoutItem);
    }

    #region Serialization

    protected IncompleteVirtualEndPointLoadStateBase (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader> ();

      var realObjectEndPoints = new List<IRealObjectEndPoint>();
      info.FillCollection (realObjectEndPoints);
      _originalOppositeEndPoints = realObjectEndPoints.ToDictionary (ep => ep.ObjectID);
      
      _dataKeeperFactory = info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<TDataKeeper>> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_lazyLoader);
      info.AddCollection(_originalOppositeEndPoints.Values);
      info.AddHandle (_dataKeeperFactory);
    }

    #endregion
  }
}