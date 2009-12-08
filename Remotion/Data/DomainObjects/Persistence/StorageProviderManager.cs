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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Tracing;
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
  private readonly IPersistenceTracer _persistenceTracer;

  // construction and disposing

  public StorageProviderManager ()
    :this (Guid.Empty)
  {
  }

  public StorageProviderManager (Guid clientTransactionID)
  {
    _storageProviders = new StorageProviderCollection ();
    _persistenceTracer = new PersistenceTracer (clientTransactionID);
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
      provider.PersistenceTracer = _persistenceTracer;

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
