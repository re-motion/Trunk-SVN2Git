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
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.EagerFetching.Parsing;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Linq
{
  public class DomainObjectQueryable<T> : QueryableBase<T>
  {
    private static IQueryProvider CreateProvider ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (T));
      var generator = new UniqueIdentifierGenerator();
      var context = new SqlPreparationContext();
      var sqlPreparationStage = new DefaultSqlPreparationStage (
          MethodCallTransformerRegistry.CreateDefault(), context, generator);
      var mappinResolutionStage = new DefaultMappingResolutionStage (new MappingResolver(), generator);
      var sqlGenerationStage = new DefaultSqlGenerationStage();

      var executor =
          ObjectFactory.Create<DomainObjectQueryExecutor> (
              ParamList.Create (classDefinition, sqlPreparationStage, mappinResolutionStage, sqlGenerationStage, context));

      var nodeTypeRegistry = MethodCallExpressionNodeTypeRegistry.CreateDefault();

      nodeTypeRegistry.Register (ContainsObjectExpressionNode.SupportedMethods, typeof (ContainsObjectExpressionNode));

      nodeTypeRegistry.Register (new[] { typeof (EagerFetchingExtensionMethods).GetMethod ("FetchOne") }, typeof (FetchOneExpressionNode));
      nodeTypeRegistry.Register (new[] { typeof (EagerFetchingExtensionMethods).GetMethod ("FetchMany") }, typeof (FetchManyExpressionNode));
      nodeTypeRegistry.Register (new[] { typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchOne") }, typeof (ThenFetchOneExpressionNode));
      nodeTypeRegistry.Register (new[] { typeof (EagerFetchingExtensionMethods).GetMethod ("ThenFetchMany") }, typeof (ThenFetchManyExpressionNode));

      return new DefaultQueryProvider (typeof (DomainObjectQueryable<>), executor, nodeTypeRegistry);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable{T}"/> class.
    /// </summary>
    /// <param name="provider">The provider to be used for querying.</param>
    /// <param name="expression">The expression encapsulated by this <see cref="DomainObjectQueryable{T}"/> instance.</param>
    /// <remarks>
    /// This constructor is used by the standard query methods defined in <see cref="Queryable"/> when a LINQ query is constructed.
    /// </remarks>
    public DomainObjectQueryable (QueryProviderBase provider, Expression expression)
        : base (provider, expression)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable{T}"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This constructor marks the default entry point into a LINQ query for <see cref="DomainObject"/> instances. It is normally used to define
    /// the data source on which the first <c>from</c> expression operates.
    /// </para>
    /// <para>
    /// The <see cref="QueryFactory"/> class wraps this constructor and provides some additional support, so it should usually be preferred to a
    /// direct constructor call.
    /// </para>
    /// </remarks>
    public DomainObjectQueryable ()
        : base (CreateProvider ())
    {
    }

    public override string ToString ()
    {
      return "DomainObjectQueryable<" + typeof (T).Name + ">";
    }

    public DomainObjectQueryExecutor GetExecutor ()
    {
      return (DomainObjectQueryExecutor) ((QueryProviderBase) Provider).Executor;
    }
  }
}