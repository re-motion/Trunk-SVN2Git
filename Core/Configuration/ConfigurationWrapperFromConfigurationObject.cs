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
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>
  /// Concrete implementation of <see cref="ConfigurationWrapper"/> that uses an instance of the <see cref="System.Configuration.Configuration"/>
  /// type. Create the instance by invoking <see cref="ConfigurationWrapper.CreateFromConfigurationObject"/>.
  /// </summary>
  internal sealed class ConfigurationWrapperFromConfigurationObject : ConfigurationWrapper
  {
    private System.Configuration.Configuration _configuration;
    private NameValueCollection _appSettings;

    public ConfigurationWrapperFromConfigurationObject (System.Configuration.Configuration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _configuration = configuration;
      
      MethodInfo getRuntimeObject = configuration.AppSettings.GetType ().GetMethod (
        "GetRuntimeObject",
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.ExactBinding);
      Assertion.IsNotNull (getRuntimeObject, "System.Configuration.AppSettingsSection.GetRuntimeObject() does not exist.");
      _appSettings = (NameValueCollection) getRuntimeObject.Invoke (configuration.AppSettings, new object[0]) ?? new NameValueCollection();
    }

    public override object GetSection (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      return _configuration.GetSection (sectionName);
    }

    public override ConnectionStringSettings GetConnectionString (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return _configuration.ConnectionStrings.ConnectionStrings[name];
    }

    public override string GetAppSetting (string name)
    {
       ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      
      return _appSettings[name];
    }
  }
}
