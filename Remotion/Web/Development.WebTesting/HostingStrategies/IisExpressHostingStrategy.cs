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
using System.Collections.Specialized;
using System.IO;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Hosts the web application using IIS Express.
  /// </summary>
  public class IisExpressHostingStrategy : IHostingStrategy
  {
    private readonly TestSiteConfiguration _testSiteConfiguration;
    private readonly int _port;
    private IisExpressProcessWrapper _iisExpressInstance;

    /// <param name="testSiteConfiguration">The configuration of the used test site.</param>
    /// <param name="port">Port to be used.</param>
    public IisExpressHostingStrategy (TestSiteConfiguration testSiteConfiguration, int port)
    {
      ArgumentUtility.CheckNotNull ("testSiteConfiguration", testSiteConfiguration);

      _testSiteConfiguration = testSiteConfiguration;
      _port = port;
    }

    /// <summary>
    /// Constructor required for direct usage in <see cref="WebTestConfigurationSection"/>.
    /// </summary>
    /// <param name="testSiteConfiguration">The configuration of the used test site.</param>
    /// <param name="properties">The configuration properties.</param>
    [UsedImplicitly]
    public IisExpressHostingStrategy (TestSiteConfiguration testSiteConfiguration, [NotNull] NameValueCollection properties)
        : this (testSiteConfiguration, int.Parse (ArgumentUtility.CheckNotNull ("properties", properties)["port"]))
    {
    }

    /// <inheritdoc/>
    public void DeployAndStartWebApplication ()
    {
      if (_iisExpressInstance != null)
        throw new InvalidOperationException ("WebApplication is already running.");

      var absoluteWebApplicationPath = Path.GetFullPath (_testSiteConfiguration.Path);

      _iisExpressInstance = new IisExpressProcessWrapper (absoluteWebApplicationPath, _port);
      _iisExpressInstance.Run();
    }

    /// <inheritdoc/>
    public void StopAndUndeployWebApplication ()
    {
      if (_iisExpressInstance != null)
        _iisExpressInstance.Dispose();
    }
  }
}