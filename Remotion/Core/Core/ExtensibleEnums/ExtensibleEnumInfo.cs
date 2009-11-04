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
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Holds information about an extensible enum value that is held in the <see cref="ExtensibleEnumDefinition{T}"/>.
  /// </summary>
  /// <typeparam name="T">The type defining the extensible enum.</typeparam>
  public class ExtensibleEnumInfo<T> : IExtensibleEnumInfo
      where T : ExtensibleEnum<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleEnumInfo{T}"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="declaringMethod">The declaring method of the value.</param>
    public ExtensibleEnumInfo (T value, MethodInfo declaringMethod)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("declaringMethod", declaringMethod);
      Value = value;
      DeclaringMethod = declaringMethod;
    }
    
    /// <summary>
    /// Gets the extensible enum value described by this instance.
    /// </summary>
    /// <value>The value.</value>
    public T Value { get; private set; }

    /// <inheritdoc />
    IExtensibleEnum IExtensibleEnumInfo.Value
    {
      get { return Value; }
    }

    /// <summary>
    /// Gets the method declaring the <see cref="Value"/> described by this instance.
    /// </summary>
    /// <value>The declaring method of the <see cref="Value"/>.</value>
    public MethodInfo DeclaringMethod { get; private set; }
  }
}