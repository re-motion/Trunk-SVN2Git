using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class GetValueInfoByValue : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;


    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();

      _mockRepository = new MockRepository ();
      _mockBusinessObject = _mockRepository.CreateMock<IBusinessObject> ();
    }

    [Test]
    public void ValidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
          property.GetValueInfoByValue (TestEnum.Value1, null));
    }

    [Test]
    public void Null ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.GetValueInfoByValue (null, null), Is.Null);
    }

    [Test]
    public void UndefinedEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.GetValueInfoByValue (EnumWithUndefinedValue.UndefinedValue, null), Is.Null);
    }

    [Test]
    public void InvalidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo (
          new EnumerationValueInfo ((TestEnum) (-1), "-1", "-1", false),
          property.GetValueInfoByValue ((TestEnum) (-1), null));
    }

    [Test]
    public void DisabledEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithDisabledEnumValue), "DisabledFromProperty");
      _mockRepository.ReplayAll ();

      IEnumerationValueInfo actual = property.GetValueInfoByValue (TestEnum.Value1, _mockBusinessObject);

      _mockRepository.VerifyAll ();
      CheckEnumerationValueInfo (new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", false), actual);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Object must be the same type as the enum. The type passed in was 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.EnumWithUndefinedValue'; "
        + "the enum type was 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.TestEnum'.")]
    public void EnumValueFromOtherType ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      property.GetValueInfoByValue (EnumWithUndefinedValue.Value1, null);
    }

    [Test]
    public void GetDisplayNameFromGlobalizationSerivce ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");
      IBindableObjectGlobalizationService mockGlobalizationService = _mockRepository.CreateMock<IBindableObjectGlobalizationService> ();
      _businessObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value1)).Return ("MockValue1");
      _mockRepository.ReplayAll ();

      IEnumerationValueInfo actual = property.GetValueInfoByValue (TestEnum.Value1, null);

      _mockRepository.VerifyAll ();
      CheckEnumerationValueInfo (new EnumerationValueInfo (TestEnum.Value1, "Value1", "MockValue1", true), actual);
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (
        GetPropertyParameters (GetPropertyInfo (type, propertyName), _businessObjectProvider));
    }
  }
}