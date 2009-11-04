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
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Data.Linq.Parsing.ExpressionTreeVisitors.MemberBindings
{
  /// <summary>
  /// Represents a <see cref="FieldInfo"/> being bound to an associated <see cref="Expression"/> instance. This binding's 
  /// <see cref="MatchesReadAccess"/> method returns <see langword="true"/> only for the same <see cref="FieldInfo"/> the expression is bound to.
  /// <seealso cref="System.Linq.Expressions.MemberBinding"/>
  /// </summary>
  public class FieldInfoBinding : MemberBinding
  {
    public FieldInfoBinding (FieldInfo boundMember, Expression associatedExpression)
        : base (boundMember, associatedExpression)
    {
    }

    public override bool MatchesReadAccess (MemberInfo member)
    {
      return member == BoundMember;
    }
  }
}
