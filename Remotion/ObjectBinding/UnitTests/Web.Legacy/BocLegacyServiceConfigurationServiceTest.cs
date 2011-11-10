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
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Legacy;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.Web.Legacy
{
  [TestFixture]
  public class BocLegacyServiceConfigurationServiceTest
  {
    [Test]
    public void GetConfiguration ()
    {
      var allServiceTypes = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (new[] { typeof (IBocList).Assembly })
          .Select (e => e.ServiceType).ToList();
      var nonLegacyServices = new[] { typeof (BocListCssClassDefinition) };
      var legacyServiceTypes = allServiceTypes
          .Except (nonLegacyServices)
          .Concat (new[] { typeof (BocListQuirksModeCssClassDefinition) });

      Assert.That (
          legacyServiceTypes.ToArray(),
          Is.EquivalentTo (BocLegacyServiceConfigurationService.GetConfiguration().Select (e => e.ServiceType).ToArray()));

      Assert.That (
          BocLegacyServiceConfigurationService.GetConfiguration ().Where (e => !nonLegacyServices.Contains (e.ServiceType)).Select (e => e.ImplementationType.Assembly).ToArray (),
          Is.All.EqualTo (typeof (BocLegacyServiceConfigurationService).Assembly));
    }

    [Test]
    public void RegisterLegacyTypesToNewDefaultServiceLocator ()
    {
      var legacyServiceTypes = BocLegacyServiceConfigurationService.GetConfiguration();

      var locator = new DefaultServiceLocator();
      foreach (var serviceConfigurationEntry in legacyServiceTypes)
        locator.Register (serviceConfigurationEntry);

      foreach (var legacyServiceType in legacyServiceTypes)
        Assert.That (locator.GetInstance (legacyServiceType.ServiceType), Is.TypeOf (legacyServiceType.ImplementationType));
    }
  }
}