/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
public class StorageProviderManager : IDisposable
{
  // types

  // static members and constants

  // member fields

  private bool _disposed = false;
  private StorageProviderCollection _storageProviders;

  // construction and disposing

  public StorageProviderManager ()
  {
    _storageProviders = new StorageProviderCollection ();
  }

  #region IDisposable Members

  public void Dispose ()
  {
    if (!_disposed)
    {
      if (_storageProviders != null)
        _storageProviders.Dispose ();

      _storageProviders = null;
      
      _disposed = true;
      GC.SuppressFinalize (this);
    }
  }

  #endregion

  // methods and properties

  public StorageProvider GetMandatory (string storageProviderID)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

    StorageProvider provider = this[storageProviderID];
    if (provider == null)
    {
      throw CreatePersistenceException (
        "Storage Provider with ID '{0}' could not be created.", storageProviderID);
    }

    return provider;
  }

  public StorageProvider this [string storageProviderID]
  {
    get 
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      if (_storageProviders.Contains (storageProviderID))
        return _storageProviders[storageProviderID];

      StorageProviderDefinition providerDefinition = 
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (storageProviderID);

      Type concreteStorageProviderType = TypeFactory.GetConcreteType (providerDefinition.StorageProviderType);
      StorageProvider provider = (StorageProvider) ReflectionUtility.CreateObject (
          concreteStorageProviderType, providerDefinition);

      _storageProviders.Add (provider);

      return provider;
    }
  }

  public StorageProviderCollection StorageProviders
  {
    get 
    { 
      CheckDisposed ();
      return _storageProviders; 
    }
  }

  private PersistenceException CreatePersistenceException (string message, params object[] args)
  {
    return new PersistenceException (string.Format (message, args));
  }

  private void CheckDisposed ()
  {
    if (_disposed)
      throw new ObjectDisposedException ("StorageProviderManager", "A disposed StorageProviderManager cannot be accessed.");
  }
}
}
