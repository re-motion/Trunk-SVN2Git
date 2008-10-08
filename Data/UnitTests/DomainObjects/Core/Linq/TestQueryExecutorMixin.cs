/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  public class TestQueryExecutorMixin : Mixin<object, TestQueryExecutorMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      ClassDefinition GetClassDefinition ();
      IQuery CreateQuery (ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters);
      IQuery CreateQuery (QueryModel queryModel);
      CommandData CreateStatement (QueryModel queryModel);
    }

    public bool GetClassDefinitionCalled = false;
    public bool CreateQueryCalled = false;
    public bool CreateQueryFromModelCalled = false;
    public bool GetStatementCalled = false;

    [OverrideTarget]
    public ClassDefinition GetClassDefinition ()
    {
      GetClassDefinitionCalled = true;
      return Base.GetClassDefinition ();
    }

    [OverrideTarget]
    public IQuery CreateQuery (ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters)
    {
      CreateQueryCalled = true;
      return Base.CreateQuery (classDefinition, statement, commandParameters);
    }

    [OverrideTarget]
    public IQuery CreateQuery (QueryModel queryModel)
    {
      CreateQueryFromModelCalled = true;
      return Base.CreateQuery (queryModel);
    }

    [OverrideTarget]
    public CommandData CreateStatement (QueryModel queryModel)
    {
      GetStatementCalled = true;
      return Base.CreateStatement (queryModel);
    }
  }
}