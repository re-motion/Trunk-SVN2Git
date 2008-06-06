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
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>
  /// Concrete implementation of <see cref="ConfigurationWrapper"/> that uses the <see cref="ConfigurationManager"/>. Create the instance by
  /// invoking <see cref="ConfigurationWrapper.CreateFromConfigurationManager"/>.
  /// </summary>
  internal sealed class ConfigurationWrapperFromConfigurationManager: ConfigurationWrapper
  {
    public ConfigurationWrapperFromConfigurationManager()
    {
    }

    public override object GetSection (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      return ConfigurationManager.GetSection (sectionName);
    }

    public override ConnectionStringSettings GetConnectionString (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ConfigurationManager.ConnectionStrings[name];
    }

    public override string GetAppSetting (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ConfigurationManager.AppSettings[name];
    }
  }
}
