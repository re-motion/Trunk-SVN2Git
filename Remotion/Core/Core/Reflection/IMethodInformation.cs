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
using System.Globalization;
using System.Reflection;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides information about a method and offers a way to invoke the method.
  /// </summary>
  public interface IMethodInformation : IMemberInformation
  {
    /// <summary>
    /// Gets the return type of the method.
    /// </summary>
    /// <value>The return type of the method.</value>
    Type ReturnType { get; }

    /// <summary>
    /// Invokes the method on the given instance using the given parameters.
    /// </summary>
    /// <param name="instance">The instance on which to invoke the method. If the method is static this argument is ignored.</param>
    /// <param name="parameters">An argument list for the invoked method.</param>
    /// <returns>An object containing the return value of the invoked method.</returns>
    object Invoke (object instance, object[] parameters);

    /// <summary>
    /// Invokes the method on the given instance using the given parameters.
    /// </summary>
    /// <param name="instance">The instance on which to invoke the method. If the method is static this argument is ignored. 
    /// If instance is <see langword="null"/> the value Default for <see cref="BindingFlags"/> is used.</param>
    /// <param name="invokeAttr">Specifies flags that control binding and the way in which the search is conducted by reflection.</param>
    /// <param name="binder">An object that enables the binding. If binder is <see langword="null"/> the default binder is used.</param>
    /// <param name="parameters">An argument list for the invoked method.</param>
    /// <param name="culture">An instance of <see cref="CultureInfo"/> used to govern the coercion of types. 
    /// If culture is null the Culture of the current thread is used.</param>
    /// <returns>An object containing the return value of the invoked method.</returns>
    object Invoke (Object instance, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture);
  }
}