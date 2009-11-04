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
using System.Reflection;
using Remotion.Data.Linq.Clauses.Expressions;

namespace Remotion.Data.Linq.Backend.FieldResolving
{
  /// <summary>
  /// Holds information about the members involved in a field access. This is produced by <see cref="FieldResolverVisitor"/> and 
  /// used by <see cref="FieldResolver"/>.
  /// </summary>
  public struct FieldAccessInfo
  {
    public FieldAccessInfo (MemberInfo accessedMember, MemberInfo[] joinMembers, QuerySourceReferenceExpression querySourceReferenceExpression)
        : this()
    {
      AccessedMember = accessedMember;
      JoinMembers = joinMembers;
      QuerySourceReferenceExpression = querySourceReferenceExpression;
    }

    public MemberInfo AccessedMember { get; private set; }
    public MemberInfo[] JoinMembers { get; private set; }
    public QuerySourceReferenceExpression QuerySourceReferenceExpression { get; private set; }
  }
}
