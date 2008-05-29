using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class DateTimePropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void Initialize_DateProperty ()
    {
      IBusinessObjectDateTimeProperty property = new DateProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Date"), _businessObjectProvider));

      Assert.That (property.Type, Is.EqualTo (DateTimeType.Date));
    }

    [Test]
    public void Initialize_DateTimeProperty ()
    {
      IBusinessObjectDateTimeProperty property = new DateTimeProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "DateTime"), _businessObjectProvider));

      Assert.That (property.Type, Is.EqualTo (DateTimeType.DateTime));
    }
  }
}