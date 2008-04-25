using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class SelectCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateForIDLookupWithMultipleValues ()
    {
      ClassDefinition personClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Person));

      SelectCommandBuilder sqlCommandBuilder = SelectCommandBuilder.CreateForIDLookup (
          Provider, personClass.GetEntityName (), new ObjectID[] { DomainObjectIDs.Person, DomainObjectIDs.Customer });

      Assert.IsNotNull (sqlCommandBuilder);

      Provider.Connect ();
      using (IDbCommand command = sqlCommandBuilder.Create ())
      {
        string expectedCommandText = "SELECT * FROM [TableInheritance_Person] WHERE [ID] IN (@ID1, @ID2);";
        Assert.AreEqual (expectedCommandText, command.CommandText);
        Assert.AreEqual (2, command.Parameters.Count);
        Assert.AreEqual (DomainObjectIDs.Person.Value, ((SqlParameter) command.Parameters["@ID1"]).Value);
        Assert.AreEqual (DomainObjectIDs.Customer.Value, ((SqlParameter) command.Parameters["@ID2"]).Value);
      }
    }

  }
}
