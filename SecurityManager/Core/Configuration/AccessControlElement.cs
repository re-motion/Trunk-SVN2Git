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
