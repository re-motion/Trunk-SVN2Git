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

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Defines the kind of a given <see cref="RelationDefinition"/>.
  /// </summary>
  public enum RelationKindType
  {
    /// <summary>
    /// There is a one-to-one relationship between referenced objects.
    /// </summary>
    OneToOne,
    /// <summary>
    /// There is a one-to-many (or many-to-one) relationship between referenced objects.
    /// </summary>
    OneToMany,
    /// <summary>
    /// There is a one-to-many relationship between referenced objects, but only the "many" side has a reference to its one related object; there
    /// is no back-reference to the many objects from the "one" side.
    /// </summary>
    Unidirectional
  }
}
