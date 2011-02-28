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
namespace Remotion.Linq.Clauses
{
  /// <summary>
  /// Represents a clause in a <see cref="QueryModel"/>'s <see cref="QueryModel.BodyClauses"/> collection. Body clauses take the items generated by 
  /// the <see cref="QueryModel.MainFromClause"/>, filtering (<see cref="WhereClause"/>), ordering (<see cref="OrderByClause"/>), augmenting 
  /// (<see cref="AdditionalFromClause"/>), or otherwise processing them before they are passed to the <see cref="QueryModel.SelectClause"/>.
  /// </summary>
  public interface IBodyClause : IClause
  {
    /// <summary>
    /// Accepts the specified visitor by calling one of its Visit... methods.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <param name="queryModel">The query model in whose context this clause is visited.</param>
    /// <param name="index">The index of this clause in the <paramref name="queryModel"/>'s <see cref="QueryModel.BodyClauses"/> collection.</param>
    void Accept (IQueryModelVisitor visitor, QueryModel queryModel, int index);

    /// <summary>
    /// Clones this clause, registering its clone with the <paramref name="cloneContext"/> if it is a query source clause.
    /// </summary>
    /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
    /// <returns>A clone of this clause.</returns>
    IBodyClause Clone (CloneContext cloneContext);
  }
}
