using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class Common : EnumerationTestBase
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
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The property 'NullableScalar' defined on type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.ClassWithValueType`1[Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.EnumWithUndefinedValue]'"
        + " must not be nullable since the property's type already defines a 'Remotion.ObjectBinding.UndefinedEnumValueAttribute'.")]
    public void Initialize_NullableWithUndefinedValue ()
    {
      CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValue>), "NullableScalar");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
        "The enum type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.EnumWithUndefinedValueFromOtherType' "
        + "defines a 'Remotion.ObjectBinding.UndefinedEnumValueAttribute' with an enum value that belongs to a different enum type.")]
    public void Initialize_WithUndefinedEnumValueFromOtherType ()
    {
      CreateProperty (typeof (ClassWithValueType<EnumWithUndefinedValueFromOtherType>), "Scalar");
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (
        GetPropertyParameters (GetPropertyInfo (type, propertyName), _businessObjectProvider));
    }
  }
}