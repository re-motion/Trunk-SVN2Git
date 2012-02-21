// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  public class TestQueryExecutorMixin : Mixin<object, TestQueryExecutorMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      IQuery CreateQuery (string id, StorageProviderDefinition storageProviderDefinition, string statement, CommandParameter[] commandParameters, QueryType queryType);
      IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders, QueryType queryType);

      IQuery CreateQuery (
          string id,
          QueryModel queryModel,
          IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders,
          QueryType queryType,
          ClassDefinition classDefinitionOfResult);

      SqlCommandData CreateSqlCommand (QueryModel queryModel, bool queryType);
    }

    public bool CreateQueryCalled;
    public bool CreateQueryFromModelCalled;
    public bool CreateQueryFromModelWithClassDefinitionCalled;
    public bool CreateSqlCommandCalled;

    [OverrideTarget]
    public IQuery CreateQuery (string id, StorageProviderDefinition storageProviderDefinition, string statement, CommandParameter[] commandParameters, QueryType queryType)
    {
      CreateQueryCalled = true;
      return Next.CreateQuery (id, storageProviderDefinition, statement, commandParameters, queryType);
    }

    [OverrideTarget]
    public IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders, QueryType queryType)
    {
      CreateQueryFromModelCalled = true;
      return Next.CreateQuery (id, queryModel, fetchQueryModelBuilders, queryType);
    }

    [OverrideTarget]
    public IQuery CreateQuery (
        string id,
        QueryModel queryModel,
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders,
        QueryType queryType,
        ClassDefinition classDefinitionOfResult)
    {
      CreateQueryFromModelWithClassDefinitionCalled = true;
      return Next.CreateQuery (id, queryModel, fetchQueryModelBuilders, queryType, classDefinitionOfResult);
    }

    [OverrideTarget]
    public SqlCommandData CreateSqlCommand (QueryModel queryModel, bool checkResultIsDomainObject)
    {
      CreateSqlCommandCalled = true;
      return Next.CreateSqlCommand (queryModel, checkResultIsDomainObject);
    }
  }
}