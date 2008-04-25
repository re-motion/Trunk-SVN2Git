using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class ConvertToNativePropertyType : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();

      _mockRepository = new MockRepository();
      _mockRepository.CreateMock<IBusinessObject>();
    }


    [Test]
    public void ValidValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertFromNativePropertyType (TestEnum.Value1), Is.EqualTo (TestEnum.Value1));
    }

    [Test]
    public void Null ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (null), Is.Null);
    }

    [Test]
    public void UndefinedValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (null), Is.EqualTo (EnumWithUndefinedValue.UndefinedValue));
    }

    [Test]
    public void InvalidEnumValue ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType ((TestEnum) (-1)), Is.EqualTo ((TestEnum) (-1)));
    }

    [Test]
    public void EnumValueFromOtherType ()
    {
      PropertyBase property = CreateProperty (typeof (ClassWithValueType<TestEnum>), "Scalar");

      Assert.That (property.ConvertToNativePropertyType (EnumWithUndefinedValue.Value1), Is.EqualTo (EnumWithUndefinedValue.Value1));
    }


    protected EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (
        GetPropertyParameters (GetPropertyInfo (type, propertyName), _businessObjectProvider));
    }
  }
}