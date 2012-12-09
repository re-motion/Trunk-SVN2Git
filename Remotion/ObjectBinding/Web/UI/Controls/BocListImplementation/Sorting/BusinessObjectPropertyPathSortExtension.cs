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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  /// <summary>
  /// Extension methods for sorting rows based on an <see cref="IBusinessObjectPropertyPath"/>.
  /// </summary>
  public static class BusinessObjectPropertyPathSortExtension
  {
    /// <summary>
    /// Creates a <see cref="IComparer{T}"/> for the <paramref name="propertyPath"/>.
    /// </summary>
    /// <returns>
    /// <see cref="BusinessObjectPropertyPathBasedComparer"/> or a <see cref="NullComparer{T}"/>
    ///  if <paramref name="propertyPath"/> is <see langword="null" />.
    /// </returns>
    public static IComparer<BocListRow> CreateComparer (this IBusinessObjectPropertyPath propertyPath)
    {
      if (propertyPath == null)
        return new NullComparer<BocListRow>();

      return new BusinessObjectPropertyPathBasedComparer (propertyPath);
    }
  }
}