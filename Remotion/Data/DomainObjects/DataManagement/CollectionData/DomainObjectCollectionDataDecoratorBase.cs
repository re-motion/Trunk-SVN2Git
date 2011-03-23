using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Provides a common base class for classes decorating a <see cref="IDomainObjectCollectionData"/> instance. If not overridden, all
  /// the methods directly delegate to an inner <see cref="WrappedData"/> object.
  /// </summary>
  [Serializable]
  public abstract class DomainObjectCollectionDataDecoratorBase : IDomainObjectCollectionData
  {
    private IDomainObjectCollectionData _wrappedData;

    protected DomainObjectCollectionDataDecoratorBase (IDomainObjectCollectionData wrappedData)
    {
      ArgumentUtility.CheckNotNull ("wrappedData", wrappedData);
      _wrappedData = wrappedData;
    }

    protected IDomainObjectCollectionData WrappedData
    {
      get { return _wrappedData; }
      set { _wrappedData = value; }
    }

    public virtual int Count
    {
      get { return _wrappedData.Count; }
    }

    public virtual Type RequiredItemType
    {
      get { return _wrappedData.RequiredItemType; }
    }

    public virtual bool IsReadOnly
    {
      get { return _wrappedData.IsReadOnly; }
    }

    public virtual ICollectionEndPoint AssociatedEndPoint
    {
      get { return _wrappedData.AssociatedEndPoint; }
    }

    public virtual bool IsDataComplete
    {
      get { return _wrappedData.IsDataComplete; }
    }

    public virtual void EnsureDataComplete ()
    {
      _wrappedData.EnsureDataComplete();
    }

    public virtual IDomainObjectCollectionData GetDataStore ()
    {
      return _wrappedData.GetDataStore ();
    }

    public virtual bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.ContainsObjectID(objectID);
    }

    public virtual DomainObject GetObject (int index)
    {
      return _wrappedData.GetObject(index);
    }

    public virtual DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.GetObject(objectID);
    }

    public virtual int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.IndexOf(objectID);
    }

    public virtual void Clear ()
    {
      _wrappedData.Clear ();
    }

    public virtual void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _wrappedData.Insert (index, domainObject);
    }

    public virtual bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      return _wrappedData.Remove (domainObject);
    }

    public virtual bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.Remove (objectID);
    }

    public virtual void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      _wrappedData.Replace (index, value);
    }

    public virtual IEnumerator<DomainObject> GetEnumerator ()
    {
      return _wrappedData.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}