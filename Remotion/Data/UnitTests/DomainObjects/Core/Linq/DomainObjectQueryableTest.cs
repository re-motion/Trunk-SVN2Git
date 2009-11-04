// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Backend.SqlGeneration.SqlServer;
using Remotion.Data.Linq.EagerFetching.Parsing;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryableTest
  {
    private SqlServerGenerator _sqlGenerator;

    [SetUp]
    public void Setup ()
    {
      _sqlGenerator = new SqlServerGenerator (DatabaseInfo.Instance);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void Initialization_WrongType ()
    {
      new DomainObjectQueryable<DomainObject> (_sqlGenerator);
    }

    [Test]
    public void Provider_AutoInitialized()
    {
      var queryable = new DomainObjectQueryable<Order> (_sqlGenerator);
      Assert.That (queryable.Provider, Is.Not.Null);
      Assert.That (queryable.Provider, Is.InstanceOfType (typeof (DefaultQueryProvider)));
      Assert.That (((DefaultQueryProvider) queryable.Provider).QueryableType, Is.SameAs (typeof (DomainObjectQueryable<>)));
    }

    [Test]
    public void Provider_AutoInitialized_ContainsObjectIsRegistered ()
    {
      var queryable = new DomainObjectQueryable<Order> (_sqlGenerator);
      var containsObjectMethod = typeof (DomainObjectCollection).GetMethod ("ContainsObject");
      Assert.That (((DefaultQueryProvider) queryable.Provider).ExpressionTreeParser.NodeTypeRegistry.GetNodeType (containsObjectMethod),
          Is.SameAs (typeof (ContainsObjectExpressionNode)));
    }

    [Test]
    public void Provider_AutoInitialized_ContainsFetchMethods ()
    {
      var queryable = new DomainObjectQueryable<Order> (_sqlGenerator);

      var fetchOneMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("FetchOne");
      var fetchManyMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("FetchMany");
      var thenFetchOneMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchOne");
      var thenFetchManyMethod = typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchMany");
      
      Assert.That (((DefaultQueryProvider) queryable.Provider).ExpressionTreeParser.NodeTypeRegistry.GetNodeType (fetchOneMethod),
          Is.SameAs (typeof (FetchOneExpressionNode)));
      Assert.That (((DefaultQueryProvider) queryable.Provider).ExpressionTreeParser.NodeTypeRegistry.GetNodeType (fetchManyMethod),
          Is.SameAs (typeof (FetchManyExpressionNode)));
      Assert.That (((DefaultQueryProvider) queryable.Provider).ExpressionTreeParser.NodeTypeRegistry.GetNodeType (thenFetchOneMethod),
          Is.SameAs (typeof (ThenFetchOneExpressionNode)));
      Assert.That (((DefaultQueryProvider) queryable.Provider).ExpressionTreeParser.NodeTypeRegistry.GetNodeType (thenFetchManyMethod),
          Is.SameAs (typeof (ThenFetchManyExpressionNode)));
    }


    [Test]
    public void Provider_PassedIn ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var expectedProvider = new DefaultQueryProvider (typeof (DomainObjectQueryable<>), new DomainObjectQueryExecutor (_sqlGenerator, classDefinition));
      var queryable = new DomainObjectQueryable<Order> (expectedProvider, Expression.Constant (null, typeof (DomainObjectQueryable<Order>)));
      Assert.That (queryable.Provider, Is.Not.Null);
      Assert.That (queryable.Provider, Is.SameAs (expectedProvider));
    }
  }
}
