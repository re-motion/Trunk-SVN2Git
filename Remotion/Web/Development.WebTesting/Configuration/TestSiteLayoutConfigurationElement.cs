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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Configures the path to the test site and its required resources.
  /// </summary>
  public class TestSiteLayoutConfigurationElement : ConfigurationElement
  {
    private readonly ConfigurationProperty _rootPathProperty;
    private readonly ConfigurationProperty _resourcesProperty;

    public TestSiteLayoutConfigurationElement ()
    {
      _rootPathProperty = new ConfigurationProperty ("rootPath", typeof (string), "", ConfigurationPropertyOptions.IsRequired);
      _resourcesProperty = new ConfigurationProperty ("resources", typeof (TestSiteResourceConfigurationElementCollection));
    }

    /// <summary>
    /// Returns the absolute path to the test site used in the integration test project.
    /// </summary>
    public string RootPath => GetRootedRootPath ((string) this[_rootPathProperty]);

    /// <summary>
    /// Returns the resources needed by the test site.
    /// </summary>
    public IReadOnlyList<string> Resources => ((ConfigurationElementCollection) this[_resourcesProperty])
        .Cast<TestSiteResourceConfigurationElement>()
        .Select (resource => EnsureRootedPath (RootPath, resource.Path))
        .ToArray();

    /// <inheritdoc />
    protected override ConfigurationPropertyCollection Properties => new ConfigurationPropertyCollection { _rootPathProperty, _resourcesProperty };

    private string GetRootedRootPath ([NotNull] string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);

      return EnsureRootedPath (AppDomain.CurrentDomain.BaseDirectory, path);
    }

    private static string EnsureRootedPath (string rootPath, string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("rootPath", rootPath);
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);

      if (Path.IsPathRooted (path))
      {
        return Path.GetFullPath (path);
      }

      var combinedPath = Path.Combine (rootPath, path);

      return Path.GetFullPath (combinedPath);
    }
  }
}