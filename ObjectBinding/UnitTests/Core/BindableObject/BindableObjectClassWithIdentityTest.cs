using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectClassWithIdentityTest : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void Initialize ()
    {
      BindableObjectClassWithIdentity bindableObjectClass =
          new BindableObjectClassWithIdentity (TypeUtility.GetConcreteMixedType (typeof (ClassWithIdentity)), _bindableObjectProvider);

      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (ClassWithIdentity)));
      Assert.That (
          bindableObjectClass.Identifier,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithIdentity, Remotion.ObjectBinding.UnitTests"));
      Assert.That (bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetObject_WithDefaultService ()
    {
      BindableObjectClassWithIdentity bindableObjectClass =
          new BindableObjectClassWithIdentity (TypeUtility.GetConcreteMixedType (typeof (ClassWithIdentity)), _bindableObjectProvider);
      IGetObjectService mockService = _mockRepository.CreateMock<IGetObjectService>();
      IBusinessObjectWithIdentity expected = _mockRepository.Stub<IBusinessObjectWithIdentity>();

      Expect.Call (mockService.GetObject (bindableObjectClass, "TheUniqueIdentifier")).Return (expected);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (typeof (IGetObjectService), mockService);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject ("TheUniqueIdentifier");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void GetObject_WithCustomService ()
    {
      BindableObjectClassWithIdentity bindableObjectClass = new BindableObjectClassWithIdentity (
          TypeUtility.GetConcreteMixedType (typeof (ClassWithIdentityAndGetObjectServiceAttribute)), _bindableObjectProvider);
      ICustomGetObjectService mockService = _mockRepository.CreateMock<ICustomGetObjectService>();
      IBusinessObjectWithIdentity expected = _mockRepository.Stub<IBusinessObjectWithIdentity>();

      Expect.Call (mockService.GetObject (bindableObjectClass, "TheUniqueIdentifier")).Return (expected);
      _mockRepository.ReplayAll();

      _bindableObjectProvider.AddService (typeof (ICustomGetObjectService), mockService);
      IBusinessObjectWithIdentity actual = bindableObjectClass.GetObject ("TheUniqueIdentifier");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The 'Remotion.ObjectBinding.BindableObject.IGetObjectService' required for loading objectes of type "
        + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithIdentity' is not registered with the "
        + "'Remotion.ObjectBinding.BusinessObjectProvider' associated with this type.")]
    public void GetObject_WithoutService ()
    {
      BindableObjectClassWithIdentity bindableObjectClass =
          new BindableObjectClassWithIdentity (TypeUtility.GetConcreteMixedType (typeof (ClassWithIdentity)), _bindableObjectProvider);

      bindableObjectClass.GetObject ("TheUniqueIdentifier");
    }
  }
}