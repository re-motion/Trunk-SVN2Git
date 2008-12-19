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
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Design;
using Remotion.Development.UnitTesting.IO;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Design
{
  [TestFixture]
  public class DomainObjectsDesignModeHelperTest
  {
    [Test]
    public void InitializeConfiguration()
    {
      using (TempFile configFile = new TempFile ())
      {
        System.Configuration.Configuration configuration = ConfigurationFactory.LoadConfigurationFromFile (
            configFile, ResourceManager.GetDomainObjectsConfigurationWithFakeMappingLoader());

        MockRepository mockRepository = new MockRepository();
        IDesignModeHelper mockDesignModeHelper = mockRepository.StrictMock<IDesignModeHelper>();
        IDesignerHost stubDesignerHost = mockRepository.StrictMock<IDesignerHost> ();
        Expect.Call (mockDesignModeHelper.GetConfiguration ()).Return (configuration);
        SetupResult.For (mockDesignModeHelper.DesignerHost).Return (stubDesignerHost);

        mockRepository.ReplayAll();

        ConfigurationWrapper oldConfigurationWrapper = ConfigurationWrapper.Current;
        IDomainObjectsConfiguration oldDomainObjectsConfiguration = DomainObjectsConfiguration.Current;
        MappingConfiguration oldMappingConfiguration = MappingConfiguration.Current;

        Assert.That (oldConfigurationWrapper, Is.Not.Null);
        Assert.That (oldDomainObjectsConfiguration, Is.Not.Null);
        Assert.That (oldMappingConfiguration, Is.Not.Null);

        DomainObjectsDesignModeHelper domainObjectsDesignModeHelper = new DomainObjectsDesignModeHelper (mockDesignModeHelper);
        domainObjectsDesignModeHelper.InitializeConfiguration();

        mockRepository.VerifyAll();

        Assert.That (oldConfigurationWrapper, Is.Not.SameAs (ConfigurationWrapper.Current));
        Assert.That (oldDomainObjectsConfiguration, Is.Not.SameAs (DomainObjectsConfiguration.Current));
        Assert.That (oldMappingConfiguration, Is.Not.SameAs (MappingConfiguration.Current));
        Assert.That (MappingConfiguration.Current.ClassDefinitions.Contains ("Fake"), Is.True);
      }
    }

    [Test]
    public void InitializeConfiguration_WithNoConfiguration ()
    {
        MockRepository mockRepository = new MockRepository ();
        IDesignModeHelper mockDesignModeHelper = mockRepository.StrictMock<IDesignModeHelper> ();
        Expect.Call (mockDesignModeHelper.GetConfiguration ()).Return (null);

        mockRepository.ReplayAll ();

        ConfigurationWrapper oldConfigurationWrapper = ConfigurationWrapper.Current;
        IDomainObjectsConfiguration oldDomainObjectsConfiguration = DomainObjectsConfiguration.Current;
        MappingConfiguration oldMappingConfiguration = MappingConfiguration.Current;

        Assert.That (oldConfigurationWrapper, Is.Not.Null);
        Assert.That (oldDomainObjectsConfiguration, Is.Not.Null);
        Assert.That (oldMappingConfiguration, Is.Not.Null);

        DomainObjectsDesignModeHelper domainObjectsDesignModeHelper = new DomainObjectsDesignModeHelper (mockDesignModeHelper);
        domainObjectsDesignModeHelper.InitializeConfiguration ();

        mockRepository.VerifyAll ();

        Assert.That (oldConfigurationWrapper, Is.SameAs (ConfigurationWrapper.Current));
        Assert.That (oldDomainObjectsConfiguration, Is.SameAs (DomainObjectsConfiguration.Current));
        Assert.That (oldMappingConfiguration, Is.SameAs (MappingConfiguration.Current));
    }
  }
}
