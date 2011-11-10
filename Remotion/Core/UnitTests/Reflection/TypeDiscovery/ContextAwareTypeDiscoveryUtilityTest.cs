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
using System.ComponentModel.Design;
using NUnit.Framework;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.UnitTests.Configuration.TypeDiscovery;
using Remotion.UnitTests.Design;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection.TypeDiscovery
{
  [TestFixture]
  public class ContextAwareTypeDiscoveryUtilityTest
  {
    private MockRepository _mockRepository;
    private ITypeDiscoveryService _serviceMock;

    private TypeDiscoveryMode _oldTypeDiscoveryMode;
    private Type _oldTypeDiscoveryServiceType;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _serviceMock = _mockRepository.StrictMock<ITypeDiscoveryService>();
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = null;
      DesignerUtility.ClearDesignMode ();

      _oldTypeDiscoveryMode = TypeDiscoveryConfiguration.Current.Mode;
      _oldTypeDiscoveryServiceType = TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type;
    }

    [TearDown]
    public void TearDown ()
    {
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = null;
      DesignerUtility.ClearDesignMode ();

      TypeDiscoveryConfiguration.Current.Mode = _oldTypeDiscoveryMode;
      TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type = _oldTypeDiscoveryServiceType;
    }

    [Test]
    public void DefaultService_ComesFromConfiguration ()
    {
      TypeDiscoveryConfiguration.Current.Mode = TypeDiscoveryMode.CustomTypeDiscoveryService;
      TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type = typeof (FakeTypeDiscoveryService);

      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService;
      Assert.That (defaultService, Is.InstanceOf (typeof (FakeTypeDiscoveryService)));
    }

    [Test]
    public void DefaultService_Cached ()
    {
      TypeDiscoveryConfiguration.Current.Mode = TypeDiscoveryMode.CustomTypeDiscoveryService;
      TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type = typeof (FakeTypeDiscoveryService);

      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService;
      ITypeDiscoveryService defaultService2 = ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService;

      Assert.That (defaultService, Is.SameAs (defaultService2));
    }

    [Test]
    public void SetDefaultCurrent ()
    {
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = _serviceMock;
      Assert.That (ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService, Is.SameAs (_serviceMock));
    }

    [Test]
    public void SetDefaultCurrent_Null ()
    {
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = _serviceMock;
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = null;
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService;
      Assert.That (defaultService, Is.Not.Null);
      Assert.That (defaultService, Is.Not.SameAs (_serviceMock));
    }

    [Test]
    public void GetTypeDiscoveryService_StandardContext ()
    {
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = _serviceMock;
      Assert.That (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService (), Is.SameAs (_serviceMock));
    }

    [Test]
    public void GetTypeDiscoveryService_DesignModeContext ()
    {
      var designerHostMock = _mockRepository.StrictMock<IDesignerHost>();
      Expect.Call (designerHostMock.GetService (typeof (ITypeDiscoveryService))).Return (_serviceMock);

      _mockRepository.ReplayAll();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.That (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService (), Is.SameAs (_serviceMock));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetType_NormalMode_Success ()
    {
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType (typeof (ContextAwareTypeDiscoveryUtilityTest).AssemblyQualifiedName, true),
                   Is.SameAs (typeof (ContextAwareTypeDiscoveryUtilityTest)));
    }

    [Test]
    public void GetType_NormalMode_Failure_Null ()
    {
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType ("dfgj", false), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException))]
    public void GetType_NormalMode_Failure_Exception ()
    {
      ContextAwareTypeDiscoveryUtility.GetType ("dfgj", true);
    }

    [Test]
    public void GetType_DesignMode_Success ()
    {
      var designerHostMock = _mockRepository.StrictMock<IDesignerHost> ();
      Expect.Call (designerHostMock.GetType("abc")).Return (typeof (int));

      _mockRepository.ReplayAll ();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType ("abc", true), Is.SameAs (typeof (int)));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetType_DesignMode_Failure_Null ()
    {
      var designerHostMock = _mockRepository.StrictMock<IDesignerHost> ();
      Expect.Call (designerHostMock.GetType ("abc")).Return (null);

      _mockRepository.ReplayAll ();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType ("abc", false), Is.Null);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "Type 'abc' could not be loaded by the designer host.")]
    public void GetType_DesignMode_Failure_Exception ()
    {
      var designerHostMock = _mockRepository.StrictMock<IDesignerHost> ();
      Expect.Call (designerHostMock.GetType ("abc")).Return (null);

      _mockRepository.ReplayAll ();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      ContextAwareTypeDiscoveryUtility.GetType ("abc", true);
    }
  }
}
