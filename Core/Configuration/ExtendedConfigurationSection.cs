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

namespace Remotion.Configuration
{
  /// <summary>Base class for all configuration sections using the <see cref="ProviderHelperBase"/> to manage their provider sections.</summary>
  public abstract class ExtendedConfigurationSection: ConfigurationSection
  {
    protected ExtendedConfigurationSection()
    {
    }

    protected internal new object this [ConfigurationProperty property]
    {
      get { return base[property]; }
      set { base[property] = value; }
    }
  }
}
