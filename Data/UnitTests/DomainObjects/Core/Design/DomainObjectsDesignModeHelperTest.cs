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
        IDesignModeHelper mockDesignModeHelper = mockRepository.CreateMock<IDesignModeHelper>();
        IDesignerHost stubDesignerHost = mockRepository.CreateMock<IDesignerHost> ();
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
        IDesignModeHelper mockDesignModeHelper = mockRepository.CreateMock<IDesignModeHelper> ();
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
