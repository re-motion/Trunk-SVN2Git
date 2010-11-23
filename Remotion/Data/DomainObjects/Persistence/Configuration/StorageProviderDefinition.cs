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
using System.Collections.Specialized;
using Remotion.Configuration;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public abstract class StorageProviderDefinition: ExtendedProviderBase
  {
    // types

    // static members and constants

    // member fields

    private Type _storageProviderType;
    private TypeConversionProvider _typeConversionProvider;
    private TypeProvider _typeProvider;
    private IStorageObjectFactory _factory;

    // construction and disposing

    protected StorageProviderDefinition (string name, NameValueCollection config)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("config", config);

      var storageProviderTypeName = GetAndRemoveNonEmptyStringAttribute (config, "providerType", name, true);
      var storageFactoryTypeName = GetAndRemoveNonEmptyStringAttribute (config, "factoryType", name, true);
      Initialize (TypeUtility.GetType (storageProviderTypeName, true), TypeUtility.GetType (storageFactoryTypeName, true));
    }

    protected StorageProviderDefinition (string name, Type storageProviderType, Type factoryType)
        : base (name, new NameValueCollection())
    {
      ArgumentUtility.CheckNotNull ("storageProviderType", storageProviderType);
      ArgumentUtility.CheckNotNull ("factoryType", factoryType);

      Initialize (storageProviderType, factoryType);
    }

    private void Initialize(Type storageProviderType, Type factoryType)
    {
      _storageProviderType = storageProviderType;
      
      // When creating the storage object factory, use CreateDynamic to create a ParamList from the dynamic type of this StorageProviderDefinition. 
      // That way, concrete factories such as SqlStorageObjectFactory can have ctors taking concrete definitions such as RdbmsProviderDefinition.
      _factory = (IStorageObjectFactory) ObjectFactory.Create (factoryType, ParamList.CreateDynamic (this));
      _typeConversionProvider = TypeConversionProvider.Create ();
      _typeProvider = new TypeProvider();
    }

    // abstract methods and properties

    public abstract bool IsIdentityTypeSupported (Type identityType);

    // methods and properties

    public void CheckIdentityType (Type identityType)
    {
      if (!IsIdentityTypeSupported (identityType))
        throw new IdentityTypeNotSupportedException (_storageProviderType, identityType);
    }

    public Type StorageProviderType
    {
      get { return _storageProviderType; }
    }

    public IStorageObjectFactory Factory
    {
      get { return _factory; }
    }

    public TypeConversionProvider TypeConversionProvider
    {
      get { return _typeConversionProvider; }
    }

    public TypeProvider TypeProvider
    {
      get { return _typeProvider; }
    }
   
  }
}
