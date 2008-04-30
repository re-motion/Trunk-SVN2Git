using System;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SelectCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateWithOrderClause ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      Provider.Connect ();
      SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, orderDefinition, orderDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"), DomainObjectIDs.Customer1);

      using (IDbCommand command = builder.Create ())
      {
        Assert.AreEqual (
            "SELECT * FROM [Order] WHERE [CustomerID] = @CustomerID ORDER BY OrderNo asc;",
            command.CommandText);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];
      SelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, 
          orderDefinition, 
          orderDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"), 
          DomainObjectIDs.Customer1);
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      Provider.Connect ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
            Provider,
            orderDefinition,
            orderDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"),
            DomainObjectIDs.Customer1);

        using (IDbCommand command = builder.Create())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}
