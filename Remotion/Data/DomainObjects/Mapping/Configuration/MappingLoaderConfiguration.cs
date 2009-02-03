// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Configuration;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Mapping.Configuration
{
  public class MappingLoaderConfiguration: ExtendedConfigurationSection
  {
    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _mappingLoaderProperty;

    private readonly ConfigurationProperty _domainObjectFactoryProperty;

    public MappingLoaderConfiguration()
    {
      _mappingLoaderProperty = new ConfigurationProperty (
          "loader",
          typeof (TypeElement<IMappingLoader, MappingReflector>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_mappingLoaderProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public IMappingLoader CreateMappingLoader()
    {
      return MappingLoaderElement.CreateInstance();
    }

    public Type MappingLoaderType
    {
      get { return MappingLoaderElement.Type; }
      set { MappingLoaderElement.Type = value; }
    }

    protected TypeElement<IMappingLoader> MappingLoaderElement
    {
      get { return (TypeElement<IMappingLoader>) this[_mappingLoaderProperty]; }
    }
  }
}
