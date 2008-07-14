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
using Remotion.Reflection;
using Remotion.UnitTests.Design;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class ContextAwareTypeDiscoveryServiceTest
  {
    private MockRepository _mockRepository;
    private ITypeDiscoveryService _serviceMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _serviceMock = _mockRepository.CreateMock<ITypeDiscoveryService>();
      ContextAwareTypeDiscoveryService.SetDefaultService (null);
      DesignerUtility.ClearDesignMode ();
    }

    [TearDown]
    public void TearDown ()
    {
      ContextAwareTypeDiscoveryService.SetDefaultService (null);
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    public void AutoIntoDefaultService ()
    {
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryService.DefaultService;
      Assert.IsNotNull (defaultService);
      Assert.AreSame (defaultService, ContextAwareTypeDiscoveryService.DefaultService);
      Assert.AreSame (defaultService, ContextAwareTypeDiscoveryService.DefaultService);
      Assert.IsInstanceOfType (typeof (AssemblyFinderTypeDiscoveryService), defaultService);
      Assert.AreSame (ApplicationAssemblyFinderFilter.Instance, ((AssemblyFinderTypeDiscoveryService) defaultService).AssemblyFinder.Filter);
    }

    [Test]
    public void SetDefaultCurrent ()
    {
      ContextAwareTypeDiscoveryService.SetDefaultService (_serviceMock);
      Assert.AreSame (_serviceMock, ContextAwareTypeDiscoveryService.DefaultService);
    }

    [Test]
    public void SetDefaultCurrent_Null ()
    {
      ContextAwareTypeDiscoveryService.SetDefaultService (_serviceMock);
      ContextAwareTypeDiscoveryService.SetDefaultService (null);
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryService.DefaultService;
      Assert.IsNotNull (defaultService);
      Assert.AreNotSame (_serviceMock, defaultService);
    }

    [Test]
    public void StandardContext ()
    {
      ContextAwareTypeDiscoveryService.SetDefaultService (_serviceMock);
      Assert.AreSame (_serviceMock, ContextAwareTypeDiscoveryService.GetInstance ());
    }

    [Test]
    public void DesignModeContext ()
    {
      IDesignerHost designerHostMock = _mockRepository.CreateMock<IDesignerHost>();
      Expect.Call (designerHostMock.GetService (typeof (ITypeDiscoveryService))).Return (_serviceMock);

      _mockRepository.ReplayAll();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.AreSame (_serviceMock, ContextAwareTypeDiscoveryService.GetInstance ());

      _mockRepository.VerifyAll ();
    }
  }
}
