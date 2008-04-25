using System;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class DataContainerFactoryTest : SqlProviderBaseTest
  {
    [Test]
    public void RelationClassIDColumnRefersToAbstractClass ()
    {
      ObjectID id = new ObjectID (typeof (Order), new Guid ("{F404FD2C-B92F-46d8-BEAC-F92C0599BFD3}"));
      SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", "TableInheritance_Order", id);

      using (IDbCommand command = builder.Create ())
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          DataContainerFactory factory = new DataContainerFactory (Provider, reader);

          try
          {
            factory.CreateDataContainer ();
            Assert.Fail ("RdbmsProviderException was expected.");
          }
          catch (RdbmsProviderException ex)
          {
            string expectedBeginOfMessage = string.Format ("Error while reading property 'Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.Order.Customer' of object '{0}':", id);
            Assert.IsTrue (ex.Message.StartsWith (expectedBeginOfMessage));
          }
        }
      }
    }
  }
}
