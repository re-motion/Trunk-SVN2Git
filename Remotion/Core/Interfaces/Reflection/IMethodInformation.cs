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
    /// Finds the implementation <see cref="IMethodInformation"/> corresponding to this <see cref="IMethodInformation"/> on the given 
    /// <see cref="Type"/>. This <see cref="IMethodInformation"/> object must denote an interface property.
    /// </summary>
    /// <param name="implementationType">The type to search for an implementation of this <see cref="IMethodInformation"/> on.</param>
    /// <returns>An instance of <see cref="IMethodInformation"/> describing the method implementing this interface 
    /// <see cref="IMethodInformation"/> on <paramref name="implementationType"/>, or <see langword="null" /> if the 
    /// <paramref name="implementationType"/> does not implement the interface.</returns>
    new IMethodInformation FindInterfaceImplementation (Type implementationType);

    /// <summary>
    /// Finds the property declaration corresponding to this <see cref="IMethodInformation"/> on the given <see cref="Type"/> and it's base types.
    /// </summary>
    /// <param name="implementationType">The type to search for the property declaration.</param>
    /// <returns>Returns the <see cref="IPropertyInformation"/> of the declared property or null if no corresponding property was found.</returns>
    IPropertyInformation FindDeclaringProperty (Type implementationType);

    /// <summary>
    /// Finds the interface declaration for the <see cref="IMethodInformation"/>. This <see cref="IMethodInformation"/> object must denote an 
    /// implementation method. 
    /// </summary>
    /// <returns>Returns the <see cref="IMethodInformation"/> of the declared property accessor or null if no corresponding accesor was found.</returns>
    new IMethodInformation FindInterfaceDeclaration ();

    T GetFastInvoker<T> () where T: class;

    Delegate GetFastInvoker (Type delegateType);
  }
}