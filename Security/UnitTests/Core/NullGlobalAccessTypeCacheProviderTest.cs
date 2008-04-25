using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullGlobalAccessTypeCacheProviderTest
  {
    private IGlobalAccessTypeCacheProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new NullGlobalAccessTypeCacheProvider();
    }

    [Test]
    public void Initialize()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new NullGlobalAccessTypeCacheProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
    
    [Test]
    public void GetAccessTypeCache ()
    {
      Assert.IsInstanceOfType (typeof (NullCache<Tuple<SecurityContext, string>, AccessType[]>), _provider.GetCache());
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsTrue (_provider.IsNull);
    }

    [Test]
    public void SerializeInstanceNotInConfiguration ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      NullGlobalAccessTypeCacheProvider provider = new NullGlobalAccessTypeCacheProvider ("MyProvider", config);
      NullGlobalAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);

      Assert.AreEqual ("MyProvider", deserializedProvider.Name);
      Assert.AreEqual ("The Description", deserializedProvider.Description);
      Assert.IsInstanceOfType (typeof (NullCache<Tuple<SecurityContext, string>, AccessType[]>), deserializedProvider.GetCache ());
      Assert.IsTrue (((IGlobalAccessTypeCacheProvider) deserializedProvider).IsNull);
    }

    [Test]
    public void SerializeInstanceFromConfiguration ()
    {
      NullGlobalAccessTypeCacheProvider provider =
          (NullGlobalAccessTypeCacheProvider) SecurityConfiguration.Current.GlobalAccessTypeCacheProviders["None"];

      NullGlobalAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);
      Assert.AreSame (provider, deserializedProvider);
    }
  }
}