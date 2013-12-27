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
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe
{
  /// <summary>
  /// Represents a strongly typed list of parameters to be passed to a function or action delegate.
  /// </summary>
  public abstract partial class ParamList
  {
    private static readonly ParamList _empty = new ParamListImplementation ();

    /// <summary>
    /// Represents an empty parameter list. This is equivalent to calling the <see cref="Create()"/> overload without parameters.
    /// </summary>
    public static ParamList Empty 
    { 
      // Must be a property rather than a static field 
      // stupid exception messages!
      get { return _empty; } 
    }

    /// <summary>
    /// Returns an empty parameter list to be passed to a function or action.
    /// </summary>
    /// <returns>An empty <see cref="ParamList"/>. This is the same value returned by <see cref="Empty"/>.</returns>
    public static ParamList Create () { return Empty; }

    /// <summary>
    /// Creates a strongly typed list of parameters to be passed to a function or action.
    /// </summary>
    /// <returns>A <see cref="ParamList"/> encapsulating the passed parameters.</returns>
    public static ParamList CreateDynamic (Type[] parameterTypes, object[] parameterValues)
    {
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);
      ArgumentUtility.CheckNotNull ("parameterValues", parameterValues);

      return new DynamicParamList (parameterTypes, parameterValues);
    }

    /// <summary>
    /// Creates a list of parameters to be passed to a function or action. This overload takes a <c>params</c> array, which is convenient, but might
    /// lead to subtle bugs, especially when <see langword="null"/> values are supplied.
    /// </summary>
    /// <returns>A <see cref="ParamList"/> encapsulating the passed parameters.</returns>
    public static ParamList CreateDynamic (params object[] parameterValues)
    {
      ArgumentUtility.CheckNotNull ("parameterValues", parameterValues);

      var parameterTypes = Array.ConvertAll (parameterValues, p => p != null ? p.GetType () : typeof (object));
      return CreateDynamic (parameterTypes, parameterValues);
    }

    /// <summary>
    /// Gets the type of <see cref="Func{TResult}"/> delegates supported by this <see cref="ParamList"/> instance.
    /// </summary>
    /// <value>The function delegate type supported by this <see cref="ParamList"/> instance.</value>
    public abstract Type FuncType { get; }

    /// <summary>
    /// Gets the type of <see cref="Action"/> delegates supported by this <see cref="ParamList"/> instance.
    /// </summary>
    /// <value>The action delegate type supported by this <see cref="ParamList"/> instance.</value>
    public abstract Type ActionType { get; }

    /// <summary>
    /// Executes the given action delegate, passing in the parameters encapsulated by this <see cref="ParamList"/>.
    /// </summary>
    /// <param name="action">The action to be executed. This delegate must match <see cref="ActionType"/>. If <see cref="ActionType"/> is null,
    /// it must match the types returned by <see cref="GetParameterTypes"/>.</param>
    public abstract void InvokeAction (Delegate action);

    /// <summary>
    /// Executes the given function delegate, passing in the parameters encapsulated by this <see cref="ParamList"/>.
    /// </summary>
    /// <param name="func">The function to be executed. This delegate must match <see cref="FuncType"/>. If <see cref="FuncType"/> is null,
    /// it must match the types returned by <see cref="GetParameterTypes"/>, plus <see cref="System.Object"/> as the return type.</param>
    /// <returns>The result of the delegate execution.</returns>
    public abstract object InvokeFunc (Delegate func);

    /// <summary>
    /// Gets the parameter types of the parameters encapsulated by this <see cref="ParamList"/>.
    /// </summary>
    /// <returns>The parameter types.</returns>
    public abstract Type[] GetParameterTypes ();

    /// <summary>
    /// Gets the parameter values of the parameters encapsulated by this <see cref="ParamList"/>.
    /// </summary>
    /// <returns>The parameter values.</returns>
    public abstract object[] GetParameterValues ();
  }
}
