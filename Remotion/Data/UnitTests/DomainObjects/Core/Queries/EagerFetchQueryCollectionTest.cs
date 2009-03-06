// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class EagerFetchQueryCollectionTest : StandardMappingTest
  {
    private IQuery _query1;
    private IQuery _query2;
    private EagerFetchQueryCollection _collection;
    private IRelationEndPointDefinition _endPointDefinition1;
    private IRelationEndPointDefinition _endPointDefinition2;

    public override void SetUp ()
    {
      base.SetUp ();

      _query1 = QueryFactory.CreateQuery (TestQueryFactory.CreateOrderSumQueryDefinition ());
      _query2 = QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType());

      _endPointDefinition1 = DomainObjectIDs.Order1.ClassDefinition.GetRelationEndPointDefinitions ()[0];
      _endPointDefinition2 = DomainObjectIDs.Order1.ClassDefinition.GetRelationEndPointDefinitions ()[1];

      _collection = new EagerFetchQueryCollection ();
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_endPointDefinition1, _query1);
      Assert.That (_collection.ToArray (), Is.EquivalentTo (
          new[] { 
              new KeyValuePair<IRelationEndPointDefinition, IQuery> (_endPointDefinition1, _query1) 
          }));

      _collection.Add (_endPointDefinition2, _query2);
      Assert.That (_collection.ToArray (), Is.EquivalentTo (
          new[] { 
              new KeyValuePair<IRelationEndPointDefinition, IQuery> (_endPointDefinition1, _query1), 
              new KeyValuePair<IRelationEndPointDefinition, IQuery> (_endPointDefinition2, _query2) 
          }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is already an eager fetch query for relation end point " 
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official'.")]
    public void Add_Twice ()
    {
      _collection.Add (_endPointDefinition1, _query1);
      _collection.Add (_endPointDefinition1, _query2);
    }

    [Test]
    public void Count ()
    {
      Assert.That (_collection.Count, Is.EqualTo (0));
      _collection.Add (_endPointDefinition1, _query1);
      Assert.That (_collection.Count, Is.EqualTo (1));
      _collection.Add (_endPointDefinition2, _query2);
      Assert.That (_collection.Count, Is.EqualTo (2));
    }
  }
}