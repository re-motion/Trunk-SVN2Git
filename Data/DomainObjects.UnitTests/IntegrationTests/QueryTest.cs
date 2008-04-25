using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class QueryTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void GetCollectionWithNullValues ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      Query query = new Query ("QueryWithNullValues");
      queryManager.GetCollection (query);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A database query returned duplicates of the domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not supported.")]
    public void GetCollectionWithDuplicates ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      Query query = new Query ("QueryWithDuplicates");
      queryManager.GetCollection (query);
    }

  }
}