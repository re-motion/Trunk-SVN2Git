using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Queries
{
  [TestFixture]
  public class QueryDefinitionCollectionTest : StandardMappingTest
  {
    private QueryDefinitionCollection _collection;
    private QueryDefinition _definition;

    public override void SetUp ()
    {
      base.SetUp ();

      _collection = new QueryDefinitionCollection ();

      _definition = new QueryDefinition (
          "OrderQuery",
          "TestDomain",
          "select Order.* from Order inner join Customer where Customer.ID = @customerID order by OrderNo asc;",
          QueryType.Collection,
          typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "QueryDefinition 'OrderQuery' already exists in collection.\r\nParameter name: queryDefinition")]
    public void DuplicateQueryIDs ()
    {
      _collection.Add (_definition);
      _collection.Add (_definition);
    }

    [Test]
    public void ContainsQueryDefinitionTrue ()
    {
      _collection.Add (_definition);

      Assert.IsTrue (_collection.Contains (_definition));
    }

    [Test]
    public void ContainsQueryDefinitionFalse ()
    {
      _collection.Add (_definition);

      QueryDefinition copy = new QueryDefinition (
          _definition.ID, _definition.StorageProviderID, _definition.Statement, _definition.QueryType, _definition.CollectionType);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullQueryDefinition ()
    {
      _collection.Contains ((QueryDefinition) null);
    }

    [Test]
    public void GetMandatory ()
    {
      _collection.Add (_definition);

      Assert.AreSame (_definition, _collection.GetMandatory (_definition.ID));
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException),
        ExpectedMessage = "QueryDefinition 'OrderQuery' does not exist.")]
    public void GetMandatoryForNonExisting ()
    {
      _collection.GetMandatory ("OrderQuery");
    }

    [Test]
    public void Merge ()
    {
      QueryDefinition query1 = new QueryDefinition ("id1", "bla", "bla", QueryType.Collection);
      QueryDefinition query2 = new QueryDefinition ("id2", "bla", "bla", QueryType.Collection);
      QueryDefinition query3 = new QueryDefinition ("id3", "bla", "bla", QueryType.Collection);
      QueryDefinition query4 = new QueryDefinition ("id4", "bla", "bla", QueryType.Collection);
      QueryDefinition query5 = new QueryDefinition ("id5", "bla", "bla", QueryType.Collection);

      QueryDefinitionCollection source = new QueryDefinitionCollection ();
      source.Add (query1);
      source.Add (query2);

      QueryDefinitionCollection target = new QueryDefinitionCollection ();
      target.Add (query3);
      target.Add (query4);
      target.Add (query5);

      target.Merge (source);

      Assert.AreEqual (2, source.Count);
      Assert.AreSame (query1, source[0]);
      Assert.AreSame (query2, source[1]);

      Assert.AreEqual (5, target.Count);
      Assert.AreSame (query3, target[0]);
      Assert.AreSame (query4, target[1]);
      Assert.AreSame (query5, target[2]);
      Assert.AreSame (query1, target[3]);
      Assert.AreSame (query2, target[4]);
    }

    [Test]
    [ExpectedException (typeof (DuplicateQueryDefinitionException), ExpectedMessage = "The query with ID 'id1' has a duplicate.")]
    public void Merge_ThrowsOnDuplicates ()
    {
      QueryDefinition query1 = new QueryDefinition ("id1", "bla", "bla", QueryType.Collection);
      QueryDefinition query2 = new QueryDefinition ("id1", "bla", "bla", QueryType.Collection);

      QueryDefinitionCollection source = new QueryDefinitionCollection ();
      source.Add (query1);

      QueryDefinitionCollection target = new QueryDefinitionCollection ();
      target.Add (query2);

      target.Merge (source);

      Assert.AreEqual (1, target.Count);
      Assert.AreSame (query2, target[0]);

      Assert.Fail ();
    }
  }
}
