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
  /// <summary>
  /// Defines the configuration for a specific <see cref="StorageProvider"/>. Subclasses of <see cref="StorageProviderDefinition"/> can be 
  /// instantiated from a config file entry.
  /// </summary>
  public abstract class StorageProviderDefinition: ExtendedProviderBase
  {
    // types

    // static members and constants

    // member fields

    private readonly IStorageObjectFactory _factory;

    // construction and disposing

    protected StorageProviderDefinition (string name, NameValueCollection config)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("config", config);

      var factoryTypeName = GetAndRemoveNonEmptyStringAttribute (config, "factoryType", name, true);
      var factoryType = TypeUtility.GetType (factoryTypeName, true);
      _factory = CreateStorageObjectFactory (factoryType);
    }

    protected StorageProviderDefinition (string name, Type factoryType)
        : base (name, new NameValueCollection())
    {
      ArgumentUtility.CheckNotNull ("factoryType", factoryType);

      _factory = CreateStorageObjectFactory (factoryType);
    }

    // abstract methods and properties

    public abstract bool IsIdentityTypeSupported (Type identityType);

    // methods and properties

    public void CheckIdentityType (Type identityType)
    {
      if (!IsIdentityTypeSupported (identityType))
        throw new IdentityTypeNotSupportedException (GetType(), identityType);
    }

    public IStorageObjectFactory Factory
    {
      get { return _factory; }
    }

    private IStorageObjectFactory CreateStorageObjectFactory (Type factoryType)
    {
      // When creating the storage object factory, use CreateDynamic to create a ParamList from the dynamic type of this StorageProviderDefinition. 
      // That way, concrete factories such as SqlStorageObjectFactory can have ctors taking concrete definitions such as RdbmsProviderDefinition.
      return (IStorageObjectFactory) ObjectFactory.Create (factoryType, ParamList.CreateDynamic (this));
    }
  }
}
