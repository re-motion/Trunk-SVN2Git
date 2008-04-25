using System;
using NUnit.Framework;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandCreatePermissionAttributeTest
  {
    [Test]
    public void Initialize ()
    {
      WxeDemandCreatePermissionAttribute attribute = new WxeDemandCreatePermissionAttribute (typeof (SecurableObject));

      Assert.AreEqual (MethodType.Constructor, attribute.MethodType);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }
  }
}
