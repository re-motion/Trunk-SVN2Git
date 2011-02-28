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
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation.PredefinedTransformations
{
  /// <summary>
  /// Detects <see cref="NewExpression"/> nodes for <see cref="KeyValuePair{TKey,TValue}"/> and adds <see cref="MemberInfo"/> metadata to those nodes.
  /// This allows LINQ providers to match member access and constructor arguments more easily.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="KeyValuePairNewExpressionTransformer"/> tries to mimic the expressions created by the C# compiler when
  /// an anonymous type is instantiated. This means that in .NET 3.5, getter methods are associated with the constructor
  /// arguments; in .NET 4, the respective <see cref="PropertyInfo"/> instances are associated with the constructor arguments.
  /// See <see cref="MemberAddingNewExpressionTransformerBase"/> for an example.
  /// </para>
  /// </remarks>
  public class KeyValuePairNewExpressionTransformer : MemberAddingNewExpressionTransformerBase
  {
    public KeyValuePairNewExpressionTransformer (Version frameworkVersion)
        : base(frameworkVersion)
    {
    }

    protected override MemberInfo[] GetMembers (ConstructorInfo constructorInfo, ReadOnlyCollection<Expression> arguments)
    {
      return new[] 
      { 
          GetMemberForNewExpression (constructorInfo.DeclaringType, "Key"), 
          GetMemberForNewExpression (constructorInfo.DeclaringType, "Value") 
      };
    }

    protected override bool CanAddMembers (Type instantiatedType, ReadOnlyCollection<Expression> arguments)
    {
      return instantiatedType.Name == typeof (KeyValuePair<,>).Name 
             && instantiatedType.Namespace == typeof (KeyValuePair<,>).Namespace 
             && arguments.Count == 2;
    }
  }
}