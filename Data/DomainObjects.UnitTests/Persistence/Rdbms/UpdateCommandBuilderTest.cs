using System;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class UpdateCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      new UpdateCommandBuilder (Provider, order.InternalDataContainer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "State of provided DataContainer must not be 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Provider.Connect ();
      new UpdateCommandBuilder (Provider, order.InternalDataContainer);
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Provider.Connect ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        CommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, order.InternalDataContainer);
        using (IDbCommand command = commandBuilder.Create())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}
