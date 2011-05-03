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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the properties and relations of a class.</summary>
  public abstract class MemberReflectorBase
  {
    public const StorageClass DefaultStorageClass = StorageClass.Persistent;

    private readonly IPropertyInformation _propertyInfo;
    private readonly IMappingNameResolver _nameResolver;
    private readonly StorageClassAttribute _storageClassAttribute;

    protected MemberReflectorBase (PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);
      _propertyInfo = new PropertyInfoAdapter (propertyInfo);
      _nameResolver = nameResolver;
      _storageClassAttribute = PropertyInfo.GetCustomAttribute<StorageClassAttribute> (true);
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public StorageClassAttribute StorageClassAttribute
    {
      get { return _storageClassAttribute; }
    }

    public StorageClass StorageClass
    {
      get { return StorageClassAttribute != null ? StorageClassAttribute.StorageClass : DefaultStorageClass; }
    }

    protected virtual string GetPropertyName ()
    {
      return _nameResolver.GetPropertyName (PropertyInfo);
    }

    protected bool IsNullableFromAttribute ()
    {
      var attribute = PropertyInfo.GetCustomAttribute<INullablePropertyAttribute> (true);
      if (attribute != null)
        return attribute.IsNullable;
      return true;
    }
  }
}
