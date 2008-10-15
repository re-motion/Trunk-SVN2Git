using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class ExtensionMethodsTest : ClientTransactionBaseTest
  {
    [Test]
    public void ToObjectList ()
    {
      IQueryable<Order> queryable = from o in QueryFactory.CreateLinqQuery<Order>() 
                                    where o.OrderNumber == 1 || o.ID == DomainObjectIDs.Order2 
                                    select o;
      
      ObjectList<Order> list = queryable.ToObjectList();
      Assert.That (list, Is.EquivalentTo (new[] { Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2) }));
    }
  }
}