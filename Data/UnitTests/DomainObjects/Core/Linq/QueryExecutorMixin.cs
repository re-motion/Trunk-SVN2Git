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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  //TODO: Move to Remotion.Development.Data.DomainObjects.Linq
  [Extends (typeof (QueryExecutor<>))]
  [AcceptsAlphabeticOrderingAttribute]
  public class QueryExecutorMixin : Mixin<object, QueryExecutorMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      Query CreateQuery (ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters);
    }

    [OverrideTarget]
    public Query CreateQuery (ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters)
    {
      Query query = Base.CreateQuery (classDefinition, statement, commandParameters);
      QueryConstructed(query);
      return query;
    }

    private static void QueryConstructed (Query query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      Console.WriteLine (query.Statement);
      foreach (QueryParameter parameter in query.Parameters)
        Console.WriteLine ("{0} = {1}", parameter.Name, parameter.Value);
    }
  }
}