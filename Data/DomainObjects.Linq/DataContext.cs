/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Linq.Expressions;
using Remotion.Data.Linq.Parsing.Details;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.Linq
{
  public static class DataContext
  {
    static DataContext ()
    {
      ResetSqlGenerator();
    }

    public static void ResetSqlGenerator ()
    {
      SqlGenerator = ObjectFactory.Create<SqlServerGenerator>().With (DatabaseInfo.Instance);
      
      WhereConditionParserRegistry whereConditionParserRegistry = SqlGenerator.DetailParser.WhereConditionParser;
      whereConditionParserRegistry.RegisterParser (typeof (MethodCallExpression), new ContainsObjectParser (whereConditionParserRegistry));
    }

    public static ISqlGeneratorBase SqlGenerator { get; set; }
    
    public static DomainObjectQueryable<T> Entity<T> ()
      where T : DomainObject
    {
      return new DomainObjectQueryable<T> (SqlGenerator);
    }
  }
}
