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
namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Indicates the kind of a <see cref="DomainObject">DomainObject's</see> property.
  /// </summary>
  public enum PropertyKind
  {
    /// <summary>
    /// The property is a simple value.
    /// </summary>
    PropertyValue,
    /// <summary>
    /// The property is a single related domain object.
    /// </summary>
    RelatedObject,
    /// <summary>
    /// The property is a collection of related domain objects.
    /// </summary>
    RelatedObjectCollection
  }
}
