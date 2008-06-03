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
using System.Configuration;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class StorageGroupElement: ConfigurationElement, INamedConfigurationElement
  {
    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _storageProviderNameProperty;

    private readonly ConfigurationProperty _storageGroupTypeProperty;
    private readonly DoubleCheckedLockingContainer<StorageGroupAttribute> _storageGroup;

    public StorageGroupElement()
    {
      _storageGroup = new DoubleCheckedLockingContainer<StorageGroupAttribute> (
          delegate { return (StorageGroupAttribute) Activator.CreateInstance (StorageGroupType); });
      _storageGroupTypeProperty = TypeElement<StorageGroupAttribute>.CreateTypeProperty (null);

      _storageProviderNameProperty = new ConfigurationProperty (
          "provider",
          typeof (string),
          null,
          ConfigurationPropertyOptions.IsRequired);


      _properties.Add (_storageGroupTypeProperty);
      _properties.Add (_storageProviderNameProperty);
    }

    //TODO: test
    public StorageGroupElement (StorageGroupAttribute storageGroup, string storageProviderName)
        : this()
    {
      ArgumentUtility.CheckNotNull ("storageGroup", storageGroup);
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderName", storageProviderName);

      StorageGroup = storageGroup;
      StorageGroupType = storageGroup.GetType();
      StorageProviderName = storageProviderName;
    }

    public StorageGroupAttribute StorageGroup
    {
      get { return _storageGroup.Value; }
      protected set { _storageGroup.Value = value; }
    }

    protected Type StorageGroupType
    {
      get { return (Type) this[_storageGroupTypeProperty]; }
      set { this[_storageGroupTypeProperty] = value; }
    }

    public string StorageProviderName
    {
      get { return (string) this[_storageProviderNameProperty]; }
      protected set { this[_storageProviderNameProperty] = value; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    string INamedConfigurationElement.Name
    {
      get { return TypeUtility.GetPartialAssemblyQualifiedName (StorageGroupType); }
    }
  }
}
