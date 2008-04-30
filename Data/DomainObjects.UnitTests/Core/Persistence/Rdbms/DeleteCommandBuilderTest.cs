using System;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence.Rdbms
{
  [TestFixture]
  public class DeleteCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateWithoutForeignKeyColumn ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      classWithAllDataTypes.Delete ();
      DataContainer deletedContainer = classWithAllDataTypes.InternalDataContainer;

      Provider.Connect ();
      CommandBuilder commandBuilder = new DeleteCommandBuilder (Provider, deletedContainer);

      using (IDbCommand deleteCommand = commandBuilder.Create ())
      {
        string expectedCommandText = "DELETE FROM [TableWithAllDataTypes] WHERE [ID] = @ID AND [Timestamp] = @Timestamp;";
        Assert.AreEqual (expectedCommandText, deleteCommand.CommandText);

        Assert.AreEqual (2, deleteCommand.Parameters.Count);

        IDataParameter idParameter = (IDataParameter) deleteCommand.Parameters["@ID"];
        IDataParameter timestampParameter = (IDataParameter) deleteCommand.Parameters["@Timestamp"];

        Assert.AreEqual (deletedContainer.ID.Value, idParameter.Value);
        Assert.AreEqual (deletedContainer.Timestamp, timestampParameter.Value);
      }
    }

    [Test]
    public void CreateWithForeignKeyColumn ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();
      DataContainer deletedOrderContainer = order.InternalDataContainer;
      
      Provider.Connect ();
      CommandBuilder commandBuilder = new DeleteCommandBuilder (Provider, deletedOrderContainer);

      using (IDbCommand deleteCommand = commandBuilder.Create ())
      {
        string expectedCommandText = "DELETE FROM [Order] WHERE [ID] = @ID;";
        Assert.AreEqual (expectedCommandText, deleteCommand.CommandText);

        Assert.AreEqual (1, deleteCommand.Parameters.Count);

        IDataParameter idParameter = (IDataParameter) deleteCommand.Parameters["@ID"];

        Assert.AreEqual (deletedOrderContainer.ID.Value, idParameter.Value);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      new DeleteCommandBuilder (Provider, TestDataContainerFactory.CreateOrder1DataContainer ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "State of provided DataContainer must be 'Deleted', but is 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Provider.Connect ();
      new DeleteCommandBuilder (Provider, TestDataContainerFactory.CreateOrder1DataContainer ());
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      classWithAllDataTypes.Delete ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        DataContainer deletedContainer = classWithAllDataTypes.InternalDataContainer;

        Provider.Connect();
        CommandBuilder commandBuilder = new DeleteCommandBuilder (Provider, deletedContainer);

        using (IDbCommand deleteCommand = commandBuilder.Create())
        {
          Assert.IsTrue (deleteCommand.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}
