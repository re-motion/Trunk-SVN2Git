using System;
using System.ComponentModel.Design;
using NUnit.Framework;
using Remotion.UnitTests.Design;
using Remotion.Reflection;
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
      ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (null);
      DesignerUtility.ClearDesignMode ();
    }

    [TearDown]
    public void TearDown ()
    {
      ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (null);
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    public void AutoIntoDefaultService ()
    {
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryService.DefaultService.Current;
      Assert.IsNotNull (defaultService);
      Assert.AreSame (defaultService, ContextAwareTypeDiscoveryService.DefaultService.Current);
      Assert.AreSame (defaultService, ContextAwareTypeDiscoveryService.DefaultService.Current);
      Assert.IsInstanceOfType (typeof (AssemblyFinderTypeDiscoveryService), defaultService);
      Assert.AreSame (ApplicationAssemblyFinderFilter.Instance, ((AssemblyFinderTypeDiscoveryService) defaultService).AssemblyFinder.Filter);
    }

    [Test]
    public void SetDefaultCurrent ()
    {
      ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (_serviceMock);
      Assert.AreSame (_serviceMock, ContextAwareTypeDiscoveryService.DefaultService.Current);
    }

    [Test]
    public void SetDefaultCurrent_Null ()
    {
      ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (_serviceMock);
      ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (null);
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryService.DefaultService.Current;
      Assert.IsNotNull (defaultService);
      Assert.AreNotSame (_serviceMock, defaultService);
    }

    [Test]
    public void StandardContext ()
    {
      ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (_serviceMock);
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