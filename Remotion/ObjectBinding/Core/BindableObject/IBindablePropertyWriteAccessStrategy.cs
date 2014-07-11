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
using JetBrains.Annotations;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Defines an interface to check whether the value of a <see cref="PropertyBase"/> can be written to.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public interface IBindablePropertyWriteAccessStrategy
  {
    /// <summary>
    /// Evaluated if setting the <paramref name="propertyBase"/> is supported for the <paramref name="businessObject"/>.
    /// </summary>
    /// <param name="propertyBase">The <see cref="PropertyBase"/> for which the check will be performed. Must not be <see langword="null" />.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the check will be performed. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the <paramref name="propertyBase"/> can be set.</returns>
    bool CanWrite ([NotNull]PropertyBase propertyBase, [NotNull]IBusinessObject businessObject);
  }
}