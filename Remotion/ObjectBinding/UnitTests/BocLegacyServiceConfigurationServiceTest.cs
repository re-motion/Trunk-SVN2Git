// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.Web.Legacy;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BocLegacyServiceConfigurationServiceTest
  {
    [Test]
    public void GetConfiguration ()
    {
      var allServiceTypes = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (new[] { typeof (IBocList).Assembly })
          .Select (e => e.ServiceType).ToList();
      var legacyServiceTypes = allServiceTypes
          .Except (new[] { typeof (BocListCssClassDefinition) })
          .Concat (new[] { typeof (BocListQuirksModeCssClassDefinition) });

      Assert.That (
          legacyServiceTypes.ToArray(),
          Is.EquivalentTo (BocLegacyServiceConfigurationService.GetConfiguration().Select (e => e.ServiceType).ToArray()));
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