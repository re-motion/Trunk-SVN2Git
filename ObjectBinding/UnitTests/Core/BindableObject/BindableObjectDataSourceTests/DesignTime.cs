using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class DesignTime : TestBase
  {
    private BindableObjectDataSource _dataSource;
    private BindableObjectProvider _provider;
    private MockRepository _mockRepository;
    private ISite _stubSite;
    private IDesignerHost _mockDesignerHost;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _mockRepository = new MockRepository();
      _stubSite = _mockRepository.Stub<ISite>();
      SetupResult.For (_stubSite.DesignMode).Return (true);
      _dataSource.Site = _stubSite;

      _mockDesignerHost = _mockRepository.CreateMock<IDesignerHost>();
      SetupResult.For (_stubSite.GetService (typeof (IDesignerHost))).Return (_mockDesignerHost);

      _provider = BindableObjectProvider.Current;
      BindableObjectProvider.SetCurrent (_provider);
    }

    [Test]
    public void GetAndSetType ()
    {
      Expect.Call (
          _mockDesignerHost.GetType (
              "Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll();

      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.Type = typeof (SimpleBusinessObjectClass);
      Assert.That (_dataSource.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetType_WithNull ()
    {
      _mockRepository.ReplayAll();

      _dataSource.Type = null;
      Assert.That (_dataSource.Type, Is.Null);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectClass ()
    {
      Expect.Call (
          _mockDesignerHost.GetType (
              "Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleBusinessObjectClass))
          .Repeat.AtLeastOnce();
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.Current));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectClass_SameTwice ()
    {
      SetupResult.For (
          _mockDesignerHost.GetType (
              "Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual, Is.SameAs (_dataSource.BusinessObjectClass));

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
        + "'Remotion.ObjectBinding.IBusinessObject' interface via the 'Remotion.ObjectBinding.BindableObject.BindableObjectMixinBase`1'.\r\n"
        + "Parameter name: concreteType")]
    public void GetBusinessObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      Expect.Call (
          _mockDesignerHost.GetType ("Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleReferenceType, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleReferenceType))
          .Repeat.AtLeastOnce();
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleReferenceType);
      Dev.Null = _dataSource.BusinessObjectClass;

      _mockRepository.VerifyAll();
    }
  }
}