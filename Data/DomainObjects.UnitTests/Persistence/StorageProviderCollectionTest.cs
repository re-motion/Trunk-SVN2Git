using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class StorageProviderCollectionTest : StandardMappingTest
  {
    private StorageProviderCollection _collection;
    private StorageProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _provider = new SqlProvider (new RdbmsProviderDefinition ("TestDomain", typeof (SqlProvider), "ConnectionString"));
      _collection = new StorageProviderCollection ();
    }

    [Test]
    public void ContainsProviderTrue ()
    {
      _collection.Add (_provider);
      Assert.IsTrue (_collection.Contains (_provider));
    }

    [Test]
    public void ContainsProviderFalse ()
    {
      _collection.Add (_provider);

      StorageProvider copy = new SqlProvider ((RdbmsProviderDefinition) _provider.Definition);
      Assert.IsFalse (_collection.Contains (copy));
    }

  }
}
