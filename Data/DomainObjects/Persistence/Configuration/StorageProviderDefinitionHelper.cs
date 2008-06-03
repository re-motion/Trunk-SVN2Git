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
using System.Configuration.Provider;
using Remotion.Configuration;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class StorageProviderDefinitionHelper: ProviderHelperBase<StorageProviderDefinition>
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing


    public StorageProviderDefinitionHelper (ExtendedConfigurationSection configurationSection)
        : base (configurationSection)
    {
    }

    // methods and properties

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultProviderDefinition", null);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("providerDefinitions");
    }
  }
}
