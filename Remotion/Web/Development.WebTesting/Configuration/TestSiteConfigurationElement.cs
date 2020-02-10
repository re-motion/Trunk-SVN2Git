// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Configures the path to the test site and its required resources.
  /// </summary>
  public class TestSiteConfigurationElement : ConfigurationElement
  {
    private readonly ConfigurationProperty _pathProperty;
    private readonly ConfigurationProperty _resourcesProperty;

    public TestSiteConfigurationElement ()
    {
      _pathProperty = new ConfigurationProperty ("path", typeof (string));
      _resourcesProperty = new ConfigurationProperty ("resources", typeof (TestSiteResourceConfigurationElementCollection));
    }

    /// <summary>
    /// Returns the relative path to the test site used in the integration test project.
    /// </summary>
    public string Path => (string) this[_pathProperty];

    /// <summary>
    /// Returns the resources needed by the test site.
    /// </summary>
    public IReadOnlyList<TestSiteResourceConfigurationElement> Resources => ((ConfigurationElementCollection) this[_resourcesProperty])
        .Cast<TestSiteResourceConfigurationElement>()
        .ToArray();

    protected override ConfigurationPropertyCollection Properties => new ConfigurationPropertyCollection { _pathProperty, _resourcesProperty };
  }
}