using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.Security;

namespace Remotion.Web.UnitTests.Security
{
  [TestFixture]
  public class HttpContextUserProviderTest
  {
    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      HttpContextUserProvider provider = new HttpContextUserProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
    
    [Test]
    public void GetIsNull()
    {
      IUserProvider _userProvider = new HttpContextUserProvider();
      Assert.IsFalse (_userProvider.IsNull);
    }
  }
}