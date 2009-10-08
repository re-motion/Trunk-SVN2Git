// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Reflection;
using Remotion.UnitTests.Design;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class ContextAwareTypeDiscoveryUtilityTest
  {
    private MockRepository _mockRepository;
    private ITypeDiscoveryService _serviceMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _serviceMock = _mockRepository.StrictMock<ITypeDiscoveryService>();
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = null;
      DesignerUtility.ClearDesignMode ();
    }

    [TearDown]
    public void TearDown ()
    {
      ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService = null;
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    public void AutoInitDefaultService ()
    {
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService;
      Assert.That (defaultService, Is.Not.Null);
      Assert.That (ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService, Is.SameAs (defaultService));
      Assert.That (ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService, Is.SameAs (defaultService));
      Assert.That (defaultService, Is.InstanceOfType (typeof (AssemblyFinderTypeDiscoveryService)));
    }

    [Test]
    public void AutoInitDefaultService_RootAssemblyFinder ()
    {
      var assemblyFinder = (AssemblyFinder) ((AssemblyFinderTypeDiscoveryService) ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService).AssemblyFinder;
      Assert.That (assemblyFinder.RootAssemblyFinder, Is.InstanceOfType (typeof (SearchPathRootAssemblyFinder)));

      var searchPathRootAssemblyFinder = (SearchPathRootAssemblyFinder) assemblyFinder.RootAssemblyFinder;
      Assert.That (searchPathRootAssemblyFinder.BaseDirectory, Is.EqualTo (AppDomain.CurrentDomain.BaseDirectory));
    }

    [Test]
    public void AutoInitDefaultService_Loader ()
    {
      var assemblyFinder = (AssemblyFinder) ((AssemblyFinderTypeDiscoveryService) ContextAwareTypeDiscoveryUtility.DefaultNonDesignModeService).AssemblyFinder;
      var searchPathRootAssemblyFinder = (SearchPathRootAssemblyFinder) assemblyFinder.RootAssemblyFinder;

      Assert.That (assemblyFinder.ReferencedAssemblyLoader, Is.SameAs (searchPathRootAssemblyFinder.Loader));
      Assert.That (assemblyFinder.ReferencedAssemblyLoader, Is.InstanceOfType (typeof (AssemblyLoader)));

      var castLoader = (AssemblyLoader) assemblyFinder.ReferencedAssemblyLoader;
      Assert.That (castLoader.Filter, Is.SameAs (ApplicationAssemblyFinderFilter.Instance));
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
