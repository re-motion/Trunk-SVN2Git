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

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Provides a common interface for classes representing a modification to a <see cref="RelationEndPoint"/>. Implementations of this interface
  /// represent modifications involving exactly one end point; use <see cref="CreateRelationModification"/> to extend that modification
  /// to all involved end points.
  /// </summary>
  public interface IRelationEndPointModification
  {
    RelationEndPoint ModifiedEndPoint { get; }
    DomainObject OldRelatedObject { get; }
    DomainObject NewRelatedObject { get; }

    void Perform ();
    void Begin ();
    void End ();
    void NotifyClientTransactionOfBegin ();
    void NotifyClientTransactionOfEnd ();

    /// <summary>
    /// Extends this <see cref="IRelationEndPointModification"/> with modifications of other end points involved in the same relation and creates
    /// a <see cref="CompositeRelationModification"/> from all these modification modification steps. 
    /// </summary>
    /// <remarks>
    /// If this <see cref="RelationEndPointModification"/> is performed on a unidirectional relation, the <see cref="CompositeRelationModification"/> 
    /// returned by  <see cref="RelationEndPointModification.CreateRelationModification"/> only contains this 
    /// <see cref="RelationEndPointModification"/>,  no other steps. If it is performed on a bidirectional relation, the 
    /// <see cref="CompositeRelationModification"/> contains steps for the old/new opposite end points as well as for end points indirectly modified.
    /// </remarks>
    CompositeRelationModification CreateRelationModification ();
  }
}