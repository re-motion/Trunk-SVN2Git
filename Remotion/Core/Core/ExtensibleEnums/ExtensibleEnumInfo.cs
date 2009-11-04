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
    /// Initializes a new instance of the <see cref="ExtensibleEnumInfo&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ExtensibleEnumInfo (T value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      Value = value;
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
  }
}