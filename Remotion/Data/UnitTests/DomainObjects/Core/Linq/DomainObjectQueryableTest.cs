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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq;
using Remotion.Data.Linq.EagerFetching.Parsing;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Rhino.Mocks;


namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryableTest
  {
    private ISqlPreparationContext _context;
    private DefaultSqlPreparationStage _preparationStage;
    private DefaultMappingResolutionStage _mappingResolutionStage;
    private DefaultSqlGenerationStage _sqlGenerationStage;
    private DomainObjectQueryable<Order> _queryableWithOrder;
    private MethodCallExpressionNodeTypeRegistry _nodeTypeRegistry;
    private IStorageSpecificExpressionResolver _storageSpecificExpressionResolverStub;

    [SetUp]
    public void SetUp ()
    {
      var methodCallTransformerRegistry = CompoundMethodCallTransformerProvider.CreateDefault ();
      var resultOperatorHandlerRegistry = ResultOperatorHandlerRegistry.CreateDefault();
      _nodeTypeRegistry = MethodCallExpressionNodeTypeRegistry.CreateDefault();
      var generator = new UniqueIdentifierGenerator ();
      _storageSpecificExpressionResolverStub = MockRepository.GenerateStub<IStorageSpecificExpressionResolver> ();
      var resolver = new MappingResolver (_storageSpecificExpressionResolverStub);

      _preparationStage = new DefaultSqlPreparationStage (methodCallTransformerRegistry, resultOperatorHandlerRegistry, generator);
      _mappingResolutionStage = new DefaultMappingResolutionStage (resolver, generator);
      _sqlGenerationStage = new DefaultSqlGenerationStage();
      _context = new SqlPreparationContext (new SqlStatementBuilder());

      _queryableWithOrder = new DomainObjectQueryable<Order> (_preparationStage, _mappingResolutionStage, _sqlGenerationStage, _nodeTypeRegistry);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void Initialization_WrongType ()
    {
      new DomainObjectQueryable<DomainObject>(_preparationStage, _mappingResolutionStage, _sqlGenerationStage, _nodeTypeRegistry);
    }

    [Test]
    public void Provider_AutoInitialized ()
    {
      Assert.That (_queryableWithOrder.Provider, Is.Not.Null);
      Assert.That (_queryableWithOrder.Provider, Is.InstanceOfType (typeof (DefaultQueryProvider)));
      Assert.That (((DefaultQueryProvider) _queryableWithOrder.Provider).QueryableType, Is.SameAs (typeof (DomainObjectQueryable<>)));
    }
    
    [Test]
    public void Provider_PassedIn ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var generator = new UniqueIdentifierGenerator();
      var sqlPreparationStage = new DefaultSqlPreparationStage (
          CompoundMethodCallTransformerProvider.CreateDefault(), ResultOperatorHandlerRegistry.CreateDefault(), generator);
      var storageSpecificExpressionResolverStub = MockRepository.GenerateStub<IStorageSpecificExpressionResolver> ();
      var mappingResolver = new MappingResolver(storageSpecificExpressionResolverStub);
      var mappinResolutionStage = new DefaultMappingResolutionStage (mappingResolver, generator);
      var sqlGenerationStage = new DefaultSqlGenerationStage();
      
      var expectedProvider = new DefaultQueryProvider (
          typeof (DomainObjectQueryable<>),
          new DomainObjectQueryExecutor (classDefinition, sqlPreparationStage, mappinResolutionStage, sqlGenerationStage));
      var queryable = new DomainObjectQueryable<Order> (expectedProvider, Expression.Constant (null, typeof (DomainObjectQueryable<Order>)));
      Assert.That (queryable.Provider, Is.Not.Null);
      Assert.That (queryable.Provider, Is.SameAs (expectedProvider));
    }

    [Test]
    public void Executor_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (DomainObjectQueryExecutor)).AddMixin<TestQueryExecutorMixin> ().EnterScope ())
      {
        var queryable = new DomainObjectQueryable<Order> (_preparationStage, _mappingResolutionStage, _sqlGenerationStage, _nodeTypeRegistry);
        Assert.That (Mixin.Get<TestQueryExecutorMixin> (((DefaultQueryProvider) queryable.Provider).Executor), Is.Not.Null);
      }
    }
  }
}