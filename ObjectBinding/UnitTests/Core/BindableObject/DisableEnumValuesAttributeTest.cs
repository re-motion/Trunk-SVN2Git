using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class DisableEnumValuesAttributeTest
  {
    private class StubEnumerationValueFilter:IEnumerationValueFilter
    {
      public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void GetEnumerationValueFilter_FromFilterTypeCtor()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (typeof (StubEnumerationValueFilter));

      Assert.That (attribute.GetEnumerationValueFilter (), Is.TypeOf (typeof (StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithArray ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (new object[] {TestEnum.Value1, TestEnum.Value3});

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter(), TestEnum.Value1, TestEnum.Value3);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithOneParameter ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter(), TestEnum.Value1);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithTwoParameters ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter (), TestEnum.Value1, TestEnum.Value2);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithThreeParameters ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2, TestEnum.Value3);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter (), TestEnum.Value1, TestEnum.Value2, TestEnum.Value3);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithFourParameters ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter(), TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithFiveParameters ()
    {
      DisableEnumValuesAttribute attribute =
          new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4, TestEnum.Value5);

      CheckConstantEnumerationValueFilter (
          attribute.GetEnumerationValueFilter(), TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4, TestEnum.Value5);
    }

    private void CheckConstantEnumerationValueFilter (IEnumerationValueFilter filter, params Enum[] expectedDisabledEnumValues)
    {
      Assert.That (filter, Is.TypeOf (typeof (ConstantEnumerationValueFilter)));
      Assert.That (((ConstantEnumerationValueFilter) filter).DisabledEnumValues, Is.EqualTo (expectedDisabledEnumValues));
    }
  }
}