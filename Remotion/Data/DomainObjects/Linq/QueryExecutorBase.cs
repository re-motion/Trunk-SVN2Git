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
using System.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  public abstract class QueryExecutorBase : IQueryExecutor
  {
    public QueryExecutorBase (ISqlGenerator sqlGenerator)
    {
      SqlGenerator = sqlGenerator;
    }

    public ISqlGenerator SqlGenerator { get; private set; }

    public object ExecuteSingle (QueryModel queryModel)
    {
      IEnumerable results = ExecuteCollection (queryModel);
      var resultList = new ArrayList();
      foreach (object o in results)
        resultList.Add (o);
      if (resultList.Count == 1)
        return resultList[0];
      else
      {
        string message = string.Format ("ExecuteSingle must return a single object, but the query returned {0} objects.", resultList.Count);
        throw new InvalidOperationException (message);
      }
    }

    public IEnumerable ExecuteCollection (QueryModel queryModel)
    {
      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      IQuery query = CreateQuery("<dynamic query>", queryModel);
      return ClientTransaction.Current.QueryManager.GetCollection (query).AsEnumerable();
    }

    private void CheckProjection (IEvaluation evaluation)
    {
      if (!(evaluation is Column))
      {
        string message = string.Format ("This query provider does not support the given select projection ('{0}'). The projection must select "
                                        + "single DomainObject instances.", evaluation.GetType ().Name);
        throw new InvalidOperationException (message);
      }
      
      var column = (Column) evaluation;
      if (column.Name != "*")
      {
        string message = string.Format (
            "This query provider does not support selecting single columns ('{0}'). The projection must select whole DomainObject instances.",
            column.ColumnSource.AliasString + "." + column.Name);
        throw new InvalidOperationException (message);
      }
    }

    public virtual IQuery CreateQuery (string id, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      ClassDefinition classDefinition = GetClassDefinition ();

      CommandData commandData = CreateStatement (queryModel);
      CheckProjection (commandData.SqlGenerationData.SelectEvaluation);

      return CreateQuery (id, classDefinition, commandData.Statement, commandData.Parameters);
    }

    public virtual IQuery CreateQuery(string id, ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("commandParameters", commandParameters);

      var queryParameters = new QueryParameterCollection();
      foreach (CommandParameter commandParameter in commandParameters)
        queryParameters.Add (commandParameter.Name, commandParameter.Value, QueryParameterType.Value);

      return QueryFactory.CreateCollectionQuery (id, classDefinition.StorageProviderID, statement, queryParameters, typeof (DomainObjectCollection));
    }

    public abstract ClassDefinition GetClassDefinition ();

    public virtual CommandData CreateStatement (QueryModel queryModel)
    {
      return SqlGenerator.BuildCommand (queryModel);
    }
  }
}
