using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
public class StorageProviderCollection : CommonCollection, IDisposable
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderCollection ()
  {
  }

  // standard constructor for collections
  public StorageProviderCollection (StorageProviderCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (StorageProvider provider in collection)
    {
      Add (provider);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  #region IDisposable Members

  public virtual void Dispose ()
  {
    for (int i = Count - 1; i>= 0; i--)
      this[i].Dispose ();      

    BaseClear ();
    GC.SuppressFinalize (this);
  }

  #endregion

  // methods and properties

  #region Standard implementation for "add-only" collections

  public bool Contains (StorageProvider provider)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);

    return BaseContains (provider.ID, provider);
  }

  public bool Contains (string storageProviderID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    return BaseContainsKey (storageProviderID);
  }

  public StorageProvider this [int index]  
  {
    get { return (StorageProvider) BaseGetObject (index); }
  }

  public StorageProvider this [string storageProviderID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
      return (StorageProvider) BaseGetObject (storageProviderID); 
    }
  }

  public int Add (StorageProvider value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    
    return BaseAdd (value.ID, value);
  }

  #endregion
}
}