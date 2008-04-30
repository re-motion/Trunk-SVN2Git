using System;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence.Rdbms
{
  public class SqlProviderBaseTest : ClientTransactionBaseTest
  {
    private RdbmsProviderDefinition _providerDefinition;
    private SqlProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _providerDefinition = new RdbmsProviderDefinition (c_testDomainProviderID, typeof (SqlProvider), TestDomainConnectionString);

      _provider = new SqlProvider (_providerDefinition);
    }

    public override void TearDown ()
    {
      _provider.Dispose ();
      base.TearDown ();
    }

    protected RdbmsProviderDefinition ProviderDefinition
    {
      get { return _providerDefinition; }
    }

    protected SqlProvider Provider
    {
      get { return _provider; }
    }
  }
}
