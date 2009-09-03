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
using System;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.DetailParsing;
using Remotion.Data.Linq.Backend.DetailParsing.WhereConditionParsing;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  /// <summary>
  /// Parses usages of the "?:" operator (<see cref="ConditionalExpression"/>) used in Where conditions. The parser only works if the test
  /// can be decided in memory, it does not generate conditional expressions in the database query.
  /// Remove when https://dev.rubicon-it.com/jira/browse/COMMONS-1587 is implemented.
  /// </summary>
  /// <remarks>
  /// Register with a storage provider, like this:
  /// <code>
  /// ConditionalExpressionWhereConditionParser.Register (DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition);
  /// </code>
  /// or
  /// <code>
  /// foreach (StorageProviderDefinition definition in DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions)
  ///   ConditionalExpressionWhereConditionParser.Register (definition);
  /// </code>
  /// </remarks>
  public class ConditionalExpressionWhereConditionParser : IWhereConditionParser
  {
    public static void Register (StorageProviderDefinition storageProviderDefinition)
    {
      var registry = storageProviderDefinition.LinqSqlGenerator.DetailParserRegistries.WhereConditionParser;
      registry.RegisterParser (typeof (ConditionalExpression), new ConditionalExpressionWhereConditionParser (registry));
    }

    private readonly WhereConditionParserRegistry _parserRegistry;

    public ConditionalExpressionWhereConditionParser (WhereConditionParserRegistry parserRegistry)
    {
      ArgumentUtility.CheckNotNull ("parserRegistry", parserRegistry);
      _parserRegistry = parserRegistry;
    }

    public bool CanParse (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return expression.NodeType == ExpressionType.Conditional;
    }

    public ICriterion Parse (Expression expression, ParseContext parseContext)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("parseContext", parseContext);

      var conditionalExpression = (ConditionalExpression) expression;

      var testExpression = conditionalExpression.Test as ConstantExpression;
      if (testExpression == null)
        throw new NotSupportedException (
            "The conditional operator can only be parsed when the test part can be evaluated before going to the database.");

      if ((bool) testExpression.Value)
        return _parserRegistry.GetParser (conditionalExpression.IfTrue).Parse (conditionalExpression.IfTrue, parseContext);
      else
        return _parserRegistry.GetParser (conditionalExpression.IfFalse).Parse (conditionalExpression.IfFalse, parseContext);
    }
  }
}