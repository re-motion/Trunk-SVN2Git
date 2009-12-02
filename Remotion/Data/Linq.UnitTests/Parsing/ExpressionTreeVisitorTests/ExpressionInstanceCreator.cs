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
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Remotion.Data.UnitTests.Linq.Parsing.ExpressionTreeVisitorTests
{
  public static class ExpressionInstanceCreator
  {
    private static readonly Dictionary<ExpressionType, Expression> s_expressionTypeMap = InitializeExpressionTypeMap ();

    private static Dictionary<ExpressionType, Expression> InitializeExpressionTypeMap ()
    {
      var map = new Dictionary<ExpressionType, Expression> ();

      Expression zero = Expression.Constant (0);
      Expression zeroDouble = Expression.Constant (0.0);
      NewArrayExpression arrayExpression = Expression.NewArrayInit (typeof (object));
      Expression trueExpression = Expression.Constant (true);
      LambdaExpression lambdaExpression = Expression.Lambda (zero);
      NewExpression newExpression = Expression.New (typeof (List<int>));

      map[ExpressionType.Add] = Expression.Add (zero, zero);
      map[ExpressionType.AddChecked] = Expression.AddChecked (zero, zero);
      map[ExpressionType.And] = Expression.And (zero, zero);
      map[ExpressionType.AndAlso] = Expression.AndAlso (trueExpression, trueExpression);
      map[ExpressionType.ArrayLength] = Expression.ArrayLength (arrayExpression);
      map[ExpressionType.ArrayIndex] = Expression.ArrayIndex (arrayExpression, zero);
      map[ExpressionType.Call] = Expression.Call (zero, typeof (int).GetMethod ("GetType"));
      map[ExpressionType.Coalesce] = Expression.Coalesce (arrayExpression, arrayExpression);
      map[ExpressionType.Conditional] = Expression.Condition (trueExpression, zero, zero);
      map[ExpressionType.Constant] = Expression.Constant (zero);
      map[ExpressionType.Convert] = Expression.Convert (zero, typeof (object));
      map[ExpressionType.ConvertChecked] = Expression.ConvertChecked (zero, typeof (object));
      map[ExpressionType.Divide] = Expression.Divide (zero, zero);
      map[ExpressionType.Equal] = Expression.Equal (zero, zero);
      map[ExpressionType.ExclusiveOr] = Expression.ExclusiveOr (trueExpression, trueExpression);
      map[ExpressionType.GreaterThan] = Expression.GreaterThan (zero, zero);
      map[ExpressionType.GreaterThanOrEqual] = Expression.GreaterThanOrEqual (zero, zero);
      map[ExpressionType.Invoke] = Expression.Invoke (lambdaExpression);
      map[ExpressionType.Lambda] = lambdaExpression;
      map[ExpressionType.LeftShift] = Expression.LeftShift (zero, zero);
      map[ExpressionType.LessThan] = Expression.LessThan (zero, zero);
      map[ExpressionType.LessThanOrEqual] = Expression.LessThanOrEqual (zero, zero);
      map[ExpressionType.ListInit] = Expression.ListInit (newExpression, zero);
      map[ExpressionType.MemberAccess] = Expression.MakeMemberAccess (zero, typeof (DateTime).GetProperty ("Now"));
      map[ExpressionType.MemberInit] = Expression.MemberInit (newExpression);
      map[ExpressionType.Modulo] = Expression.Modulo (zero, zero);
      map[ExpressionType.Multiply] = Expression.Multiply (zero, zero);
      map[ExpressionType.MultiplyChecked] = Expression.MultiplyChecked (zero, zero);
      map[ExpressionType.Negate] = Expression.Negate (zero);
      map[ExpressionType.UnaryPlus] = Expression.UnaryPlus (zero);
      map[ExpressionType.NegateChecked] = Expression.NegateChecked (zero);
      map[ExpressionType.New] = newExpression;
      map[ExpressionType.NewArrayInit] = arrayExpression;
      map[ExpressionType.NewArrayBounds] = Expression.NewArrayBounds (typeof (object[]), zero);
      map[ExpressionType.Not] = Expression.Not (trueExpression);
      map[ExpressionType.NotEqual] = Expression.NotEqual (zero, zero);
      map[ExpressionType.Or] = Expression.Or (trueExpression, trueExpression);
      map[ExpressionType.OrElse] = Expression.OrElse (trueExpression, trueExpression);
      map[ExpressionType.Parameter] = Expression.Parameter (typeof (object), "bla");
      map[ExpressionType.Power] = Expression.Power (zeroDouble, zeroDouble);
      map[ExpressionType.Quote] = Expression.Quote (zero);
      map[ExpressionType.RightShift] = Expression.RightShift (zero, zero);
      map[ExpressionType.Subtract] = Expression.Subtract (zero, zero);
      map[ExpressionType.SubtractChecked] = Expression.SubtractChecked (zero, zero);
      map[ExpressionType.TypeAs] = Expression.TypeAs (zero, typeof (object));
      map[ExpressionType.TypeIs] = Expression.TypeIs (zero, typeof (object));
      map[(ExpressionType)(-1)] = new SpecialExpressionNode ((ExpressionType)(-1), typeof (int));

      return map;
    }

    public static Expression GetExpressionInstance (ExpressionType type)
    {
      return s_expressionTypeMap[type];
    }

    public static ElementInit CreateElementInit ()
    {
      return Expression.ElementInit (typeof (List<int>).GetMethod ("Add"), Expression.Constant (1));
    }

    public static MemberAssignment CreateMemberAssignment ()
    {
      return Expression.Bind (typeof (SimpleClass).GetField ("Value"), Expression.Constant ("test"));
    }

    public static MemberMemberBinding CreateMemberMemberBinding ()
    {
      return Expression.MemberBind (typeof (SimpleClass).GetField ("Value"), new List<MemberBinding>());
    }

    public static MemberListBinding CreateMemberListBinding ()
    {
      return Expression.ListBind (typeof (SimpleClass).GetField ("Value"), new ElementInit[] { });
    }
  }
}
