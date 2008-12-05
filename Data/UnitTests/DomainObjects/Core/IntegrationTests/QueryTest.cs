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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class QueryTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void GetCollectionWithNullValues ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithNullValues");
      queryManager.GetCollection (query);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A database query returned duplicates of the domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not supported.")]
    public void GetCollectionWithDuplicates ()
    {
      IQueryManager queryManager = new RootQueryManager (ClientTransactionMock);
      var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithDuplicates");
      queryManager.GetCollection (query);
    }

  }
}
