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

// ReSharper disable CheckNamespace
namespace JetBrains.Annotations
// ReSharper restore CheckNamespace
{
  /// <summary>
  /// Used for ReSharper intellisense only.
  /// When applied to target attribute, specifies a requirement for any type which is marked with 
  /// target attribute to implement or inherit specific type or types
  /// </summary>
  /// <example>
  /// <code>
  /// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
  /// public class ComponentAttribute : Attribute 
  /// {}
  /// 
  /// [Component] // ComponentAttribute requires implementing IComponent interface
  /// public class MyComponent : IComponent
  /// {}
  /// </code>
  /// </example>
  [BaseTypeRequired (new[] { typeof (Attribute) }), AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  [CLSCompliant (false)]
  public sealed class BaseTypeRequiredAttribute : Attribute
  {
    private readonly Type[] myBaseTypes;

    /// <summary>
    /// Initializes new instance of BaseTypeRequiredAttribute
    /// </summary>
    /// <param name="baseTypes">Specifies which types are required</param>
    public BaseTypeRequiredAttribute (params Type[] baseTypes)
    {
      myBaseTypes = baseTypes;
    }

    /// <summary>
    /// Gets enumerations of specified base types
    /// </summary>
    public IEnumerable<Type> BaseTypes
    {
      get { return myBaseTypes; }
    }
  }

}