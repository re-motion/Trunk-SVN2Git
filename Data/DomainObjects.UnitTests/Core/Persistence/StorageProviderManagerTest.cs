using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence
{
  [TestFixture]
  public class StorageProviderManagerTest : StandardMappingTest
  {
    private StorageProviderManager _storageProviderManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _storageProviderManager.Dispose ();
    }

    [Test]
    public void LookUp ()
    {
      StorageProvider provider = _storageProviderManager[c_testDomainProviderID];

      Assert.IsNotNull (provider);
      Assert.AreEqual (typeof (SqlProvider), provider.GetType ());
      Assert.AreEqual (c_testDomainProviderID, provider.ID);
    }

    [Test]
    public void Reference ()
    {
      StorageProvider provider1 = _storageProviderManager[c_testDomainProviderID];
      StorageProvider provider2 = _storageProviderManager[c_testDomainProviderID];

      Assert.AreSame (provider1, provider2);
    }

    [Test]
    public void Disposing ()
    {
      RdbmsProvider provider = null;

      using (_storageProviderManager)
      {
        provider = (RdbmsProvider) _storageProviderManager[c_testDomainProviderID];
        provider.LoadDataContainer (DomainObjectIDs.Order1);

        Assert.IsTrue (provider.IsConnected);
      }

      Assert.IsFalse (provider.IsConnected);
    }
  }
}
