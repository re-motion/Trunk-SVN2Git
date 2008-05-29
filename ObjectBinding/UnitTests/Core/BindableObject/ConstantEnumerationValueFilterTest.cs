using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ConstantEnumerationValueFilterTest : EnumerationTestBase
  {
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _mockRepository.CreateMock<IBusinessObject> ();
    }
    
    [Test]
    public void Initialize ()
    {
      Enum[] expected = new Enum[] {TestEnum.Value1, TestEnum.Value2};
      ConstantEnumerationValueFilter filter = new ConstantEnumerationValueFilter (expected);

      Assert.That (filter.DisabledEnumValues, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException))]
    public void Initialize_WithMismatchedEnumValues ()
    {
      new ConstantEnumerationValueFilter (new Enum[] {TestEnum.Value1, EnumWithUndefinedValue.Value2});
    }

    [Test]
    public void IsEnabled_WithFalse ()
    {
      IBusinessObject mockBusinessObject  =_mockRepository.CreateMock<IBusinessObject>();
      IBusinessObjectEnumerationProperty mockProperty = _mockRepository.CreateMock<IBusinessObjectEnumerationProperty>();

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter (new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      _mockRepository.ReplayAll();

      bool actual = filter.IsEnabled (new EnumerationValueInfo (TestEnum.Value1, "Value1", null, true), mockBusinessObject, mockProperty);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.False);
    }

    [Test]
    public void IsEnabled_WithTrue ()
    {
      IBusinessObject mockBusinessObject = _mockRepository.CreateMock<IBusinessObject> ();
      IBusinessObjectEnumerationProperty mockProperty = _mockRepository.CreateMock<IBusinessObjectEnumerationProperty> ();

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter (new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      _mockRepository.ReplayAll ();

      bool actual = filter.IsEnabled (new EnumerationValueInfo (TestEnum.Value2, "Value2", null, true), mockBusinessObject, mockProperty);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.True);
    }
  }
}