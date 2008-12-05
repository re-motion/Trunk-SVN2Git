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
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Queries
{
  [TestFixture]
  public class QueryDefinitionTest : StandardMappingTest
  {
    [Test]
    public void InitializeCollectionType ()
    {
      QueryDefinition definition = new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Collection);

      Assert.AreEqual (typeof (DomainObjectCollection), definition.CollectionType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The scalar query 'QueryID' must not specify a collectionType.\r\nParameter name: collectionType")]
    public void InitializeScalarQueryWithCollectionType ()
    {
      QueryDefinition definition = 
          new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Scalar, typeof (DomainObjectCollection));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collectionType of query 'QueryID' must be 'Remotion.Data.DomainObjects.DomainObjectCollection' or derived from it.\r\n"
        + "Parameter name: collectionType")]
    public void InitializeInvalidCollectionType ()
    {
      QueryDefinition definition = new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Collection, this.GetType ());
    }

    [Test]
    public void InitializeWithDomainObjectCollectionType ()
    {
      QueryDefinition definition = 
          new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Collection, typeof (DomainObjectCollection));

      Assert.AreEqual (typeof (DomainObjectCollection), definition.CollectionType);
    }
  }
}
