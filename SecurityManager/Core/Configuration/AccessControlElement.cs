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

namespace Remotion.SecurityManager.Configuration
{
  public class AccessControlElement : ConfigurationElement
  {
     private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _disableSpecificUserProperty;

    public AccessControlElement ()
    {
      _disableSpecificUserProperty = new ConfigurationProperty (
          "disableSpecificUser",
          typeof (bool),
          false,
          ConfigurationPropertyOptions.None);

      _properties.Add (_disableSpecificUserProperty);
    }

    public bool DisableSpecificUser
    {
      get { return (bool) this[_disableSpecificUserProperty]; }
      set { this[_disableSpecificUserProperty] = value; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }
 }
}