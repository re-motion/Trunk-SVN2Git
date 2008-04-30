using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class NullablePropertyAttributeTest
  {
    private class StubNullablePropertyAttribute: NullablePropertyAttribute
    {
      public StubNullablePropertyAttribute()
      {
      }
    }

    private StubNullablePropertyAttribute _attribute;
    private INullablePropertyAttribute _nullable;

    [SetUp]
    public void SetUp()
    {
      _attribute = new StubNullablePropertyAttribute();
      _nullable = _attribute;
    }

    [Test]
    public void GetNullable_FromDefault()
    {
      Assert.IsTrue (_attribute.IsNullable);
      Assert.IsTrue (_nullable.IsNullable);
    }

    [Test]
    public void GetNullable_FromExplicitValue()
    {
      _attribute.IsNullable = false;
      Assert.IsFalse (_attribute.IsNullable);
      Assert.IsFalse (_nullable.IsNullable);
    }
  }
}