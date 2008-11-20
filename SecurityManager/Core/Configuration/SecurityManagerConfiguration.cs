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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Configuration
{
  public class SecurityManagerConfiguration : ConfigurationSection
  {
    private static readonly DoubleCheckedLockingContainer<SecurityManagerConfiguration> s_current;

    static SecurityManagerConfiguration()
    {
      s_current = new DoubleCheckedLockingContainer<SecurityManagerConfiguration> (
          delegate { return (SecurityManagerConfiguration) ConfigurationManager.GetSection ("remotion.securityManager") ?? new SecurityManagerConfiguration (); });
    }

    public static SecurityManagerConfiguration Current
    {
      get { return s_current.Value; }
    }

    protected static void SetCurrent (SecurityManagerConfiguration current)
    {
      s_current.Value = current;
    }

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty _xmlnsProperty;

    private readonly ConfigurationProperty _organizationalStructureFactoryProperty;
    private readonly DoubleCheckedLockingContainer<IOrganizationalStructureFactory> _organizationalStructureFactory;
   
    private readonly ConfigurationProperty _accessControlProperty;

    public SecurityManagerConfiguration()
    {
      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);

      _organizationalStructureFactory = new DoubleCheckedLockingContainer<IOrganizationalStructureFactory> (
          delegate { return OrganizationalStructureFactoryElement.CreateInstance(); });
      _organizationalStructureFactoryProperty = new ConfigurationProperty (
          "organizationalStructureFactory",
          typeof (TypeElement<IOrganizationalStructureFactory, OrganizationalStructureFactory>),
          null,
          ConfigurationPropertyOptions.None);
      _accessControlProperty = new ConfigurationProperty (
          "accessControl",
          typeof (AccessControlElement),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_xmlnsProperty);
      _properties.Add (_organizationalStructureFactoryProperty);
      _properties.Add (_accessControlProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public IOrganizationalStructureFactory OrganizationalStructureFactory
    {
      get { return _organizationalStructureFactory.Value; }
      set { _organizationalStructureFactory.Value = value; }
    }

    protected TypeElement<IOrganizationalStructureFactory> OrganizationalStructureFactoryElement
    {
      get { return (TypeElement<IOrganizationalStructureFactory>) this[_organizationalStructureFactoryProperty]; }
    }

    public AccessControlElement AccessControl
    {
      get { return (AccessControlElement) this[_accessControlProperty]; }
    }
  }
}
