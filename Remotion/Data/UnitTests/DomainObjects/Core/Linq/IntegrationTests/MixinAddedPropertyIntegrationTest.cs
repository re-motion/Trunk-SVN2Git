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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  // TODO Review 2980: Fix the tests below by adding new rows to the database. To make this easier, you can write a test that builds the desired 
  // TODO Review 2980: data structure (creating or loading objects via NewObject()/GetObject()), then commits that transaction using 
  // TODO Review 2980: SetDatabaseModifyable()/ClientTransaction.Current.Commit(). Fire up SQL profiler and log the INSERT/UPDATE statements, then 
  // TODO Review 2980: add those statements to DataDomainObjects_CreateTestData.sql. Also document the data structure in the DomainObjectIDs class. 
  // TODO Review 2980: Note that you have to "Clean" the project before building if you make changes only to the SQL script; otherwise your tests
  // TODO Review 2980: will still run with the old SQL script...
  [TestFixture]
  public class MixinAddedPropertyIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void PropertyDeclaredByMixin_AppliedToSameObject ()
    {
      var mixins = (from t in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
                    where ((IMixinAddingPersistentProperties) t).PersistentProperty == 99
                    select t);

      CheckQueryResult (mixins, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToBaseObject ()
    {
      var mixins = (from m in QueryFactory.CreateLinqQuery<DerivedTargetClassForPersistentMixin> ()
                    where ((IMixinAddingPersistentProperties) m).PersistentProperty == 199
                    select m);

      CheckQueryResult (mixins, DomainObjectIDs.DerivedTargetClassForPersistentMixin1);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToBaseBaseObject ()
    {
      var mixins = (from m in QueryFactory.CreateLinqQuery<DerivedDerivedTargetClassForPersistentMixin> ()
                    where ((IMixinAddingPersistentProperties) m).PersistentProperty == 299
                    select m);

      CheckQueryResult (mixins, DomainObjectIDs.DerivedDerivedTargetClassForPersistentMixin1);
    }

    [Test]
    [Ignore ("TODO Review 2980: This test does not work because there is no instance of TargetClassForPersistentMixin in the database that has a non-null RelationPropertyID column. To remedy, change DataDomainObjects_CreateTestData.sql to include a few instances of RelationTargetForPersistentMixin, and then change the Insert statement, eg. for 784EBDDD-EE94-456D-A5F4-F6CB1B41B6CA, to include the RelationPropertyID column pointing back to one of those RelationTargets.)")]
    public void RelationWithForeignKeyAddedByMixin ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
          where ((IMixinAddingPersistentProperties) o).RelationProperty != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    [Ignore ("TODO Review 2980: This test does not work because there is no instance of RelationTargetForPersistentMixin in the database that has a non-null VirtualRelationPropertyID column. To remedy, change DataDomainObjects_CreateTestData.sql to include a few instances of RelationTargetForPersistentMixin, and have one of them include a back-reference to 4ED563B8-B337-4C8E-9A77-5FA907919377 and one to have a back-reference to 784EBDDD-EE94-456D-A5F4-F6CB1B41B6CA.)")]
    public void RelationWithoutForeignKeyAddedByMixin ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where ((IMixinAddingPersistentProperties) o).VirtualRelationProperty != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    [Ignore ("TODO Review 2980: See above.)")]
    public void RelationWithForeignKeyAddedByMixin_PropertyOfRelatedObject ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where ((IMixinAddingPersistentProperties) o).RelationProperty.ID != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    [Ignore ("TODO Review 2980: See above.)")]
    public void RelationWithoutForeignKeyAddedByMixin_PropertyOfRelatedObject ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where ((IMixinAddingPersistentProperties) o).VirtualRelationProperty.ID != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    [Ignore ("TODO Review 2980: This test does not work because DefaultMappingResolutionStage.ResolveCollectionSourceExpression cannot handle "
        + "UnaryExpressions. Change return type of ResolveCollectionSourceExpression to return Expression. In ResolvingJoinInfoVisitor, loop while the " 
        + "resolved expression is a UnaryExpression, then cast to SqlEntityExpression using an 'as' cast. If the cast fails, throw an exception "
        + "(e.g.: Only entities can be used as the collection source in from expressions, '{0}' cannot.)")]
    public void CollectionValuedRelationAddedByMixin_UsedInFromExpression ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          from related in ((IMixinAddingPersistentProperties) o).CollectionProperty1Side
          select related;
      // TODO Review 2980: To get a more meaningful result, also add a few data records for RelationTargetForPersistentMixin that point back to some TargetClassForPersistentMixins via RelationProperty3
      CheckQueryResult (query);
    }
  }
}