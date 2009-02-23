// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
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



    ///////////////////////only for testing///////////////////////////////
    //[Test]
    //public void ToFetch ()
    //{
    //  var query1 = QueryFactory.CreateLinqQuery<Customer> ();
    //  var result1 = from c in query1
    //               where c.Name == "Kunde 3"
    //               select c;
      
    //  QueryModel fetchModel = result1.Fetch (o => o.Orders);

    //  //possible sql
    //  var provider = result1.Provider as QueryProvider;
    //  var queryExecutor = (QueryExecutorBase) provider.Executor;
    //  CommandData statement = queryExecutor.CreateStatement (fetchModel);
    //  string sql = statement.Statement;
    //  Console.WriteLine (sql);

    //  //similar query model
    //  var query = QueryFactory.CreateLinqQuery<Customer> ();
    //  var result = from c in query
    //               from o in c.Orders
    //               where c.Name == "Kunde 3"
    //               select o;
    //  var ex = ((IQueryable) result).Expression;
    //  QueryParser parser = new QueryParser (ex);
    //  QueryModel queryModel = parser.GetParsedQuery(); //should be similar to fetchModel
    //  CommandData statement1 = queryExecutor.CreateStatement (fetchModel);
    //  Console.WriteLine (statement1.Statement);
    //}
    /////////////////////////////////////////////////////////////////////
  }
}