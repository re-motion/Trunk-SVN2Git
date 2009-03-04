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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class QueryTest : ClientTransactionBaseTest
  {
    [Test]
    [Ignore ("TODO 545")]
    public void GetCollectionWithNullValues ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Employee].* FROM [Computer] LEFT OUTER JOIN [Employee] ON [Computer].[EmployeeID] = [Employee].[ID] "
          + "WHERE [Computer].[ID] IN (@1, @2, @3) "
          + "ORDER BY [Computer].[ID] asc",
          new QueryParameterCollection(), typeof (DomainObjectCollection));
      
      query.Parameters.Add ("@1", DomainObjectIDs.Computer5); // no employee
      query.Parameters.Add ("@2", DomainObjectIDs.Computer1); // employee 1
      query.Parameters.Add ("@3", DomainObjectIDs.Computer4); // no employee
      
      var result = queryManager.GetCollection (query);
      Assert.That (result.ContainsNulls (), Is.True);
      Assert.That (result.ToArray (), Is.EqualTo (new[] { null, Employee.GetObject (DomainObjectIDs.Employee1), null }));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A database query returned duplicates of the domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not supported.")]
    public void GetCollectionWithDuplicates ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      var query = QueryFactory.CreateCollectionQuery ("test", DomainObjectIDs.Computer1.ClassDefinition.StorageProviderID,
          "SELECT [Order].* FROM [OrderItem] INNER JOIN [Order] ON [OrderItem].[OrderID] = [Order].[ID] WHERE [Order].[OrderNo] = 1",
          new QueryParameterCollection (), typeof (DomainObjectCollection));
      queryManager.GetCollection (query);
    }

  }
}
