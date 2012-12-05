// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.ObjectModel;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;

namespace Remotion.ObjectBinding
{
  /// <summary> A collection of business object properties that result in each other. </summary>
  /// <remarks>
  ///   <para>
  ///     A property path is comprised of zero or more <see cref="IBusinessObjectReferenceProperty"/> instances and 
  ///     a final <see cref="IBusinessObjectProperty"/>.
  ///   </para><para>
  ///     In its string representation, the property path uses the <see cref="char"/> returned by the 
  ///     <see cref="IBusinessObjectProvider.GetPropertyPathSeparator"/> method as the separator. The 
  ///     current property supplies the <see cref="IBusinessObjectProvider"/>.
  ///   </para>
  /// </remarks>
  public interface IBusinessObjectPropertyPath
  {
    /// <summary> Gets the string representation of this property path. </summary>
    string Identifier { get; }

    /// <summary> Gets the list of properties in this path. </summary>
    ReadOnlyCollection<IBusinessObjectProperty> Properties { get; }

    bool IsDynamic { get; }

    IBusinessObjectPropertyPathResult GetResult (
        IBusinessObject root,
        BusinessObjectPropertyPath.UnreachableValueBehavior unreachableValueBehavior,
        BusinessObjectPropertyPath.ListValueBehavior listValueBehavior);
  }
}
