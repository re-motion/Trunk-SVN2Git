// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.Linq;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Mixins;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  public class TestQueryExecutorMixin : Mixin<object, TestQueryExecutorMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      IQuery CreateQuery (string id, string storageProviderID, string statement, CommandParameter[] commandParameters, QueryType queryType);
      IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchRequestBase> fetchRequests, QueryType queryType);
      IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchRequestBase> fetchRequests, QueryType queryType, ClassDefinition classDefinitionOfResult, string sortExpression);
      CommandData CreateStatement (QueryModel queryModel);
    }

    public bool CreateQueryCalled = false;
    public bool CreateQueryFromModelCalled = false;
    public bool CreateQueryFromModelWithClassDefinitionCalled = false;
    public bool GetStatementCalled = false;

    [OverrideTarget]
    public IQuery CreateQuery (string id, string storageProviderID, string statement, CommandParameter[] commandParameters, QueryType queryType)
    {
      CreateQueryCalled = true;
      return Base.CreateQuery (id, storageProviderID, statement, commandParameters, queryType);
    }

    [OverrideTarget]
    public IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchRequestBase> fetchRequests, QueryType queryType)
    {
      CreateQueryFromModelCalled = true;
      return Base.CreateQuery (id, queryModel, fetchRequests, queryType);
    }

    [OverrideTarget]
    public IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchRequestBase> fetchRequests, QueryType queryType, ClassDefinition classDefinitionOfResult, string sortExpression)
    {
      CreateQueryFromModelWithClassDefinitionCalled = true;
      return Base.CreateQuery (id, queryModel, fetchRequests, queryType, classDefinitionOfResult, sortExpression);
    }

    [OverrideTarget]
    public CommandData CreateStatement (QueryModel queryModel)
    {
      GetStatementCalled = true;
      return Base.CreateStatement (queryModel);
    }
  }
}
