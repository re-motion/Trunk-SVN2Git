using System;
using System.Collections.Specialized;
using System.Threading;
using NUnit.Framework;
using Remotion.Configuration;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class ThreadUserProviderTest
  {
    // types

    // static members

    // member fields

    private IUserProvider _userProvider;

    // construction and disposing

    public ThreadUserProviderTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _userProvider = new ThreadUserProvider ();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new ThreadUserProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetUser ()
    {
      Assert.AreSame (Thread.CurrentPrincipal, _userProvider.GetUser ());
    }
    
    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (_userProvider.IsNull);
    }
  }
}