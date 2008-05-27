using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core
{
  [TestFixture]
  public class BusinessObjectProviderAttributeTest
  {
    private class StubBusinessObjectProviderAttribute : BusinessObjectProviderAttribute
    {
      public StubBusinessObjectProviderAttribute (Type businessObjectProviderType)
          : base (businessObjectProviderType)
      {
      }
    }

    [Test]
    public void Initialize_WithValidType ()
    {
      BusinessObjectProviderAttribute attribute = new StubBusinessObjectProviderAttribute (typeof (BindableObjectProvider));

      Assert.That (attribute.BusinessObjectProviderType, Is.EqualTo (typeof (BindableObjectProvider)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithInvalidType ()
    {
      new StubBusinessObjectProviderAttribute (typeof (object));
    }
  }
}