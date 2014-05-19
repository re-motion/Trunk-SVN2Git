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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Defines extension methods for working with <see cref="MethodInfo"/>.
  /// </summary>
  public static class MethodInfoExtensions
  {
    /// <summary>
    /// Returns the <see cref="Type"/> where the method was initially declared.
    /// </summary>
    /// <param name="methodInfo">The method whose type should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The <see cref="Type"/> where the method was declared for the first time.</returns>
    public static Type GetOriginalDeclaringType (this MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      return methodInfo.GetBaseDefinition ().DeclaringType;
    }
  }
}