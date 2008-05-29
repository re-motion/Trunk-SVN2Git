using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class GetEnabledValues : EnumerationTestBase
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
    public void DisableFromProperty ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithDisabledEnumValue), "DisabledFromProperty");
      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true),
              new EnumerationValueInfo (TestEnum.Value4, "Value4", "Value4", true),
              new EnumerationValueInfo (TestEnum.Value5, "Value5", "Value5", true)
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (null));
    }

    [Test]
    public void DisableFromObject ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithDisabledEnumValue), "DisabledFromObject");
      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true),
              new EnumerationValueInfo (TestEnum.Value4, "Value4", "Value4", true)
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (null));
    }

    [Test]
    public void GetDisplayNameFromGlobalizationSerivce ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithDisabledEnumValue), "DisabledFromProperty");
      IBindableObjectGlobalizationService mockGlobalizationService = _mockRepository.CreateMock<IBindableObjectGlobalizationService>();
      _businessObjectProvider.AddService (typeof (IBindableObjectGlobalizationService), mockGlobalizationService);

      EnumerationValueInfo[] expected = new EnumerationValueInfo[]
          {
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "MockValue2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "MockValue3", true),
              new EnumerationValueInfo (TestEnum.Value4, "Value4", "MockValue4", true),
              new EnumerationValueInfo (TestEnum.Value5, "Value5", "MockValue5", true)
          };

      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value1)).Return ("MockValue1");
      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value2)).Return ("MockValue2");
      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value3)).Return ("MockValue3");
      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value4)).Return ("MockValue4");
      Expect.Call (mockGlobalizationService.GetEnumerationValueDisplayName (TestEnum.Value5)).Return ("MockValue5");
      _mockRepository.ReplayAll();

      IEnumerationValueInfo[] actual = property.GetEnabledValues (null);

      _mockRepository.VerifyAll();
      CheckEnumerationValueInfos (expected, actual);
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (
        GetPropertyParameters (GetPropertyInfo (type, propertyName), _businessObjectProvider));
    }
  }
}