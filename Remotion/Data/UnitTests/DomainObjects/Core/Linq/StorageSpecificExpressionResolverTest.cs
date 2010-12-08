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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class StorageSpecificExpressionResolverTest
  {
    private StorageSpecificExpressionResolver _storageSpecificExpressionResolver;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageSpecificExpressionResolver = new StorageSpecificExpressionResolver();
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Order));
    }

    [Test]
    public void ResolveEntity ()
    {
      var result = _storageSpecificExpressionResolver.ResolveEntity (_classDefinition, "o");

      var primaryKeyColumn = new SqlColumnDefinitionExpression (typeof (ObjectID), "o", "ID", true);
      var starColumn = new SqlColumnDefinitionExpression (typeof (Order), "o", "*", false);
      var expectedExpression = new SqlEntityDefinitionExpression (typeof (Order), "o", null, primaryKeyColumn, starColumn);
      ExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }

    [Test]
    public void ResolveTableInfo ()
    {
      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTableInfo (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("OrderView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

  }
}