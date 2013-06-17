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
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Defines the concrete implementation for a service type (usually an interface or abstract class) as well as its <see cref="LifetimeKind"/>.
  /// This attribute is used by the DefaultServiceProvider to determine how to instantiate a service type. Mutiple 
  /// <see cref="ConcreteImplementationAttribute"/> instances can be applied to a single service type. They are not inherited.
  /// </summary>
  [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class ConcreteImplementationAttribute : Attribute
  {
    private readonly string _typeNameTemplate;
    private readonly bool _ignoreIfNotFound;

    /// <summary>
    /// Defines a concrete implementation for a service type by means of a type name template. The template can contain placeholders such as
    /// "&lt;version&gt;" and "&lt;publicKeyToken&gt;" that are replaced with the version and public key token of the re-motion assemblies when
    /// the type is resolved. See <see cref="TypeNameTemplateResolver"/> for details.
    /// </summary>
    /// <param name="typeNameTemplate">A type name indicating the concrete implementation for the service type, optionally containing
    /// placeholders.</param>
    /// <param name="ignoreIfNotFound">A boolean indicating whether the attribute should be ignored if the implementation type can not be loaded.
    /// The default is <see langword="false" />.</param>
    public ConcreteImplementationAttribute (string typeNameTemplate, bool ignoreIfNotFound = false)
    {
      _typeNameTemplate = ArgumentUtility.CheckNotNull ("typeNameTemplate", typeNameTemplate);
      _ignoreIfNotFound = ignoreIfNotFound;
      Lifetime = LifetimeKind.Instance;
    }

    /// <summary>
    /// Defines a concrete implementation for a service type.
    /// </summary>
    /// <param name="type">The type representing the concrete implementation for the service type.</param>
    public ConcreteImplementationAttribute (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _typeNameTemplate = type.AssemblyQualifiedName;
      Lifetime = LifetimeKind.Instance;
    }

    /// <summary>
    /// Gets the type name template for the concrete implementation type. The template can contain placeholders and can be resolved using the
    /// <see cref="TypeNameTemplateResolver"/> class.
    /// </summary>
    /// <value>The type name template.</value>
    public string TypeNameTemplate
    {
      get { return _typeNameTemplate; }
    }

    /// <summary>
    /// Gets a boolean indicating whether the attribute should be ignored if the implementation type can not be loaded.
    /// </summary>
    /// <value>A boolean indicating whether the attribute should be ignored if the implementation type can not be loaded.</value>
    public bool IgnoreIfNotFound
    {
      get { return _ignoreIfNotFound; }
    }

    /// <summary>
    /// Gets or sets the lifetime of instances of the concrete implementation type. The lifetime is used by service locators to control when to reuse 
    /// instances of the concrete implementation type and when to create new ones. The default value is <see cref="LifetimeKind.Instance"/>.
    /// </summary>
    /// <value>The lifetime of instances of the concrete implementation type.</value>
    public LifetimeKind Lifetime { get; set; }

    /// <summary>
    /// Gets the position of the concrete implementation in the list of all concrete implementations for the respective service type. The position
    /// does not denote the exact index; instead, it only influences the relative ordering of this implementation with respect to the other
    /// implementations.
    /// </summary>
    /// <value>The position of the concrete implementation in the list of all concrete implementations.</value>
    public int Position  { get; set; }
  }
}
