using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints
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

    private readonly ILazyLoader _lazyLoader;
    private readonly IVirtualEndPointDataKeeperFactory<TDataKeeper> _dataKeeperFactory;
    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _originalOppositeEndPoints;

    protected IncompleteVirtualEndPointLoadStateBase (
        ILazyLoader lazyLoader,
        IVirtualEndPointDataKeeperFactory<TDataKeeper> dataKeeperFactory)
    {
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("dataKeeperFactory", dataKeeperFactory);

      _lazyLoader = lazyLoader;
      _dataKeeperFactory = dataKeeperFactory;
      _originalOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
    }

    public abstract void EnsureDataComplete (TEndPoint endPoint);

    public bool CanEndPointBeCollected (TEndPoint endPoint)
    {
      return _originalOppositeEndPoints.Count == 0;
    }

    public ICollection<IRealObjectEndPoint> OriginalOppositeEndPoints
    {
      get { return _originalOppositeEndPoints.Values; }
    }

    public ILazyLoader LazyLoader
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

    public bool CanDataBeMarkedIncomplete (TEndPoint endPoint)
    {
      return true;
    }

    public void MarkDataIncomplete (TEndPoint endPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      // Do nothing - data is already incomplete
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
 
    public virtual void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _originalOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
      oppositeEndPoint.ResetSyncState ();
    }

    public virtual void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
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

    public void SynchronizeOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      throw new InvalidOperationException ("Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.");
    }

    public void SetDataFromSubTransaction (TEndPoint endPoint, IVirtualEndPointLoadState<TEndPoint, TData, TDataKeeper> sourceLoadState)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("sourceLoadState", sourceLoadState);

      throw new InvalidOperationException ("Cannot comit data from a sub-transaction into a virtual end-point in incomplete state.");
    }

    public bool HasChanged ()
    {
      return false; 
    }

    public void Commit (TEndPoint endPoint)
    {
      Assertion.IsTrue (!HasChanged());
    }

    public void Rollback (TEndPoint endPoint)
    {
      Assertion.IsTrue (!HasChanged ());
    }

    protected void MarkDataComplete (TEndPoint endPoint, IEnumerable<DomainObject> items, Action<TDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      if (s_log.IsInfoEnabled)
        s_log.InfoFormat ("Virtual end-point '{0}' is transitioned to complete state.", endPoint.ID);

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
            s_log.WarnFormat ("Virtual end-point '{0}' contains an item without an opposite end-point: '{1}'. The virtual end-point is out-of-sync.",
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
      _lazyLoader = info.GetValueForHandle<ILazyLoader> ();

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