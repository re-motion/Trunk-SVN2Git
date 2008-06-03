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
using System.ComponentModel;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Design;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Design
{
  /// <summary>
  /// The <see cref="DomainObjectsDesignModeHelper"/> is inteded to encapsulate design mode specific initialization code for <see cref="IComponent"/>
  /// implementations in the <see cref="N:Remotion.Data.DomainObjects"/> namespace, such as data sources.
  /// </summary>
  public class DomainObjectsDesignModeHelper
  {
    private readonly IDesignModeHelper _designModeHelper;

    public DomainObjectsDesignModeHelper (IDesignModeHelper designModeHelper)
    {
      ArgumentUtility.CheckNotNull ("designModeHelper", designModeHelper);

      _designModeHelper = designModeHelper;
    }

    public void InitializeConfiguration ()
    {
      System.Configuration.Configuration configuration = _designModeHelper.GetConfiguration();
      if (configuration != null)
      {
        ConfigurationWrapper.SetCurrent (ConfigurationWrapper.CreateFromConfigurationObject (configuration));
        DomainObjectsConfiguration.SetCurrent (new DomainObjectsConfiguration());
        MappingConfiguration.SetCurrent (new MappingConfiguration (GetMappingLoader()));       
      }
    }

    private IMappingLoader GetMappingLoader()
    {
      Type mappingLoaderType = GetMappingLoaderType();
      DesignModeMappingLoaderAttribute designModeMappingLoaderAttribute = 
          AttributeUtility.GetCustomAttribute<DesignModeMappingLoaderAttribute> (mappingLoaderType, true);
      Assertion.IsNotNull(
          designModeMappingLoaderAttribute, 
          "'{0}' does not have the '{1}' applied.", mappingLoaderType.FullName, typeof (DesignModeMappingLoaderAttribute).FullName);

      return designModeMappingLoaderAttribute.CreateInstance (_designModeHelper.DesignerHost);
    }

    private Type GetMappingLoaderType()
    {
      IDomainObjectsConfiguration domainObjectsConfiguration = DomainObjectsConfiguration.Current;
      Assertion.IsNotNull (domainObjectsConfiguration, "DomainObjectsConfiguration.Current evaluated and returned null.");

      MappingLoaderConfiguration mappingLoaderConfiguration = domainObjectsConfiguration.MappingLoader;
      Assertion.IsNotNull (mappingLoaderConfiguration, "DomainObjectsConfiguration.Current.MappingLoader evaluated and returned null.");
      
      Type mappingLoaderType = mappingLoaderConfiguration.MappingLoaderType;
      Assertion.IsNotNull (mappingLoaderType, "DomainObjectsConfiguration.Current.MappingLoader.MappingLoaderType evaluated and returned null.");

      return mappingLoaderType;
    }
  }
}
