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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses.ResultOperators
{
  /// <summary>
  /// Represents the group part of a query, grouping items given by an <see cref="ElementSelector"/> according to some key retrieved by a
  /// <see cref="KeySelector"/>. This is a result operator, operating on the whole result set of the query.
  /// </summary>
  /// <example>
  /// In C#, the "group by" clause in the following sample corresponds to a <see cref="GroupResultOperator"/>. "s" (a reference to the query source "s", see
  /// <see cref="QuerySourceReferenceExpression"/>) is the <see cref="ElementSelector"/> expression, "s.Country" is the <see cref="KeySelector"/>
  /// expression:
  /// <code>
  /// var query = from s in Students
  ///             where s.First == "Hugo"
  ///             group s by s.Country;
  /// </code>
  /// </example>
  public class GroupResultOperator : ResultOperatorBase
  {
    private InputDependentExpression _keySelector;
    private InputDependentExpression _elementSelector;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupResultOperator"/> class.
    /// </summary>
    /// <param name="keySelector">The selector retrieving the key by which to group items.</param>
    /// <param name="elementSelector">The selector retrieving the elements to group.</param>
    public GroupResultOperator (InputDependentExpression keySelector, InputDependentExpression elementSelector)
      : base (CollectionExecutionStrategy.Instance)
    {
      ArgumentUtility.CheckNotNull ("elementSelector", elementSelector);
      ArgumentUtility.CheckNotNull ("keySelector", keySelector);

      _elementSelector = elementSelector;
      _keySelector = keySelector;
    }

    /// <summary>
    /// Gets or sets the selector retrieving the key by which to group items.
    /// </summary>
    /// <value>The key selector.</value>
    public InputDependentExpression KeySelector
    {
      get { return _keySelector; }
      set { _keySelector = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Gets or sets the selector retrieving the elements to group.
    /// </summary>
    /// <value>The element selector.</value>
    public InputDependentExpression ElementSelector
    {
      get { return _elementSelector; }
      set { _elementSelector = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Clones this clause, adjusting all <see cref="QuerySourceReferenceExpression"/> instances held by it as defined by
    /// <paramref name="cloneContext"/>.
    /// </summary>
    /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
    /// <returns>A clone of this clause.</returns>
    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clone = new GroupResultOperator (KeySelector, ElementSelector);
      return clone;
    }

    /// <summary>
    /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this
    /// clause, and those expressions will be replaced with what the delegate returns.</param>
    public override void TransformExpressions (Func<Expression, Expression> transformation)
    {
      ArgumentUtility.CheckNotNull ("transformation", transformation);

      ElementSelector = new InputDependentExpression (
          Expression.Lambda (transformation (ElementSelector.DependentExpression.Body), ElementSelector.InputParameter),
          transformation (ElementSelector.ExpectedInput));

      KeySelector = new InputDependentExpression (
          Expression.Lambda (transformation (KeySelector.DependentExpression.Body), KeySelector.InputParameter),
          transformation (KeySelector.ExpectedInput));
    }

    public override object ExecuteInMemory (object input)
    {
      ArgumentUtility.CheckNotNull ("input", input);

      var itemType = GetInputItemType (input);

      var executeMethod = typeof (GroupResultOperator).GetMethod ("ExecuteGroupingInMemory");
      var closedExecuteMethod = executeMethod.MakeGenericMethod (
          KeySelector.DependentExpression.Body.Type,
          ElementSelector.DependentExpression.Body.Type,
          itemType);

      return InvokeExecuteMethod (input, closedExecuteMethod);
    }

    public IEnumerable<IGrouping<TKey, TElement>> ExecuteGroupingInMemory<TKey, TElement, TInput> (IEnumerable<TInput> input)
    {
      ArgumentUtility.CheckNotNull ("input", input);

      return input.GroupBy (
          (Func<TInput, TKey>) KeySelector.DependentExpression.Compile (),
          (Func<TInput, TElement>) ElementSelector.DependentExpression.Compile ());
    }

    public override Type GetResultType (Type inputResultType)
    {
      ArgumentUtility.CheckNotNull ("inputResultType", inputResultType);
      var itemType = ReflectionUtility.GetItemTypeOfIEnumerable (inputResultType, "inputResultType");

      if (KeySelector.InputParameter.Type != ElementSelector.InputParameter.Type)
      {
        throw new InvalidOperationException (
            "Cannot get a result type for this GroupResultOperator, its KeySelector and ElementSelector's input types don't match.");
      }

      if (!KeySelector.InputParameter.Type.IsAssignableFrom (itemType))
      {
        var expectedEnumerableType = typeof (IEnumerable<>).MakeGenericType (KeySelector.InputParameter.Type);
        var message = string.Format (
            "The input's item type must be assignable to the input type of the KeySelector and ElementSelector. Expected a type assignable to '{0}', "
            + "but got '{1}'.",
            expectedEnumerableType,
            inputResultType);
        throw new ArgumentTypeException (
            message, 
            "inputResultType",
            expectedEnumerableType, 
            inputResultType);
      }

      var groupingType = typeof (IGrouping<,>).MakeGenericType (KeySelector.ResolvedExpression.Type, ElementSelector.ResolvedExpression.Type);
      return typeof (IQueryable<>).MakeGenericType (groupingType);
    }

    public override string ToString ()
    {
      return string.Format ("GroupBy({0}, {1})", KeySelector, ElementSelector);
    }
  }
}