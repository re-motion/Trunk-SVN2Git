// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Linq.Clauses.Expressions;

namespace Remotion.Linq.Clauses
{
  /// <summary>
  /// Represents a clause or result operator that generates items which are streamed to the following clauses or operators.
  /// </summary>
  public interface IQuerySource
  {
    /// <summary>
    /// Gets the name of the items generated by this <see cref="IQuerySource"/>.
    /// </summary>
    /// <remarks>
    /// Item names are inferred when a query expression is parsed, and they usually correspond to the variable names present in that expression. 
    /// However, note that names are not necessarily unique within a <see cref="QueryModel"/>. Use names only for readability and debugging, not for 
    /// uniquely identifying <see cref="IQuerySource"/> objects. To match an <see cref="IQuerySource"/> with its references, use the 
    /// <see cref="QuerySourceReferenceExpression.ReferencedQuerySource"/> property rather than the <see cref="ItemName"/>.
    /// </remarks>
    string ItemName { get; }

    /// <summary>
    /// Gets the type of the items generated by this <see cref="IQuerySource"/>.
    /// </summary>
    Type ItemType { get; }
  }
}
