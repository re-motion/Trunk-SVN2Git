using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class RunTime:TestBase
  {
    private BindableObjectDataSource _dataSource;
    private BindableObjectProvider _provider;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _provider = new BindableObjectProvider();
      BindableObjectProvider.SetCurrent (_provider);
    }

    [Test]
    public void GetAndSetBusinessObject ()
    {
      IBusinessObject businessObject = MockRepository.GenerateStub<IBusinessObject>();
      ((IBusinessObjectDataSource) _dataSource).BusinessObject = businessObject;
      Assert.That (((IBusinessObjectDataSource) _dataSource).BusinessObject, Is.SameAs (businessObject));
    }

    [Test]
    public void GetAndSetType ()
    {
      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.Type = typeof (SimpleBusinessObjectClass);
      Assert.That (_dataSource.Type, Is.EqualTo (typeof (SimpleBusinessObjectClass)));
    }

    [Test]
    public void GetAndSetType_WithNull ()
    {
      _dataSource.Type = null;
      Assert.That (_dataSource.Type, Is.Null);
    }

    [Test]
    public void GetBusinessObjectClass_WithoutType ()
    {
      Assert.That (_dataSource.Type, Is.Null);
      Assert.That (_dataSource.BusinessObjectClass, Is.Null);
    }

    [Test]
    public void GetBusinessObjectClass_WithValidType ()
    {
      _dataSource.Type = typeof (SimpleBusinessObjectClass);
      Assert.That (_dataSource.BusinessObjectClass, Is.SameAs (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
        + "'Remotion.ObjectBinding.IBusinessObject' interface via the 'Remotion.ObjectBinding.BindableObject.BindableObjectMixinBase`1'.\r\n"
        + "Parameter name: concreteType")]
    public void GetBusinessObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      _dataSource.Type = typeof (SimpleReferenceType);
      Dev.Null = _dataSource.BusinessObjectClass;
    }
  }
}