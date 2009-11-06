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
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Holds information about an extensible enum value, including the <see cref="Value"/> itself and meta-info such as the 
  /// <see cref="DefiningMethod"/> and the associated <see cref="ResourceManager"/>.
  /// </summary>
  /// <typeparam name="T">The extensible enum type.</typeparam>
  public class ExtensibleEnumInfo<T> : IExtensibleEnumInfo
      where T : ExtensibleEnum<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleEnumInfo{T}"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="declaringMethod">The declaring method of the value.</param>
    /// <param name="resourceManager">The resource manager for the value.</param>
    public ExtensibleEnumInfo (T value, MethodInfo declaringMethod, IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("declaringMethod", declaringMethod);
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      Value = value;
      DefiningMethod = declaringMethod;
      ResourceManager = resourceManager;
    }
    
    /// <summary>
    /// Gets the <see cref="ExtensibleEnum{T}"/> value described by this instance.
    /// </summary>
    /// <value>The value.</value>
    public T Value { get; private set; }

    /// <inheritdoc />
    IExtensibleEnum IExtensibleEnumInfo.Value
    {
      get { return Value; }
    }

    /// <summary>
    /// Gets the method defining the <see cref="Value"/> described by this instance.
    /// </summary>
    /// <value>The defining method of the <see cref="Value"/>.</value>
    public MethodInfo DefiningMethod { get; private set; }

    /// <summary>
    /// Gets the resource manager associated with the <see cref="DefiningMethod"/> of the <see cref="Value"/> described by this instance.
    /// </summary>
    /// <value>The resource manager of this <see cref="Value"/>.</value>
    public IResourceManager ResourceManager { get; private set; }
  }
}