/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;

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

      IQuery query = CreateQuery(queryModel);
      return ClientTransaction.Current.QueryManager.GetCollection (query);
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

    public virtual IQuery CreateQuery (QueryModel queryModel)
    {
      ClassDefinition classDefinition = GetClassDefinition ();

      CommandData commandData = CreateStatement (queryModel);
      CheckProjection (commandData.SqlGenerationData.SelectEvaluation);

      return CreateQuery (classDefinition, commandData.Statement, commandData.Parameters);
    }

    public virtual IQuery CreateQuery(ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters)
    {
      var queryParameters = new QueryParameterCollection();
      foreach (CommandParameter commandParameter in commandParameters)
        queryParameters.Add (commandParameter.Name, commandParameter.Value, QueryParameterType.Value);

      return QueryFactory.CreateCollectionQuery ("<dynamic query>", classDefinition.StorageProviderID, statement, queryParameters, typeof (DomainObjectCollection));
    }

    public abstract ClassDefinition GetClassDefinition ();

    public virtual CommandData CreateStatement (QueryModel queryModel)
    {
      return SqlGenerator.BuildCommand (queryModel);
    }
  }
}