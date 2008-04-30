using System;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  public class SqlProviderBaseTest : TableInheritanceMappingTest
  {
    private StorageProviderManager _storageProviderManager;
    private SqlProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager ();
      _provider = (SqlProvider) _storageProviderManager.GetMandatory (TableInheritanceTestDomainProviderID);
      _provider.Connect ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _storageProviderManager.Dispose ();
    }

    protected StorageProviderManager StorageProviderManager
    {
      get { return _storageProviderManager; }
    }

    protected SqlProvider Provider
    {
      get { return _provider; }
    }
  }
}
