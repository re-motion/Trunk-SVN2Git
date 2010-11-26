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
using Remotion.Reflection;
using System.Linq;

namespace Remotion.Mixins.Samples.CompositionPattern.Core.Framework
{
  /// <summary>
  /// Acts as a convenience base class for domain objects in the mixin-based composition pattern. Implements the <see cref="IDomainObject"/> interface
  /// and provides a <see cref="This"/> property that provides access to the composed interface.
  /// </summary>
  /// <typeparam name="TComposedInterface">The composed interface of the derived class.</typeparam>
  public abstract class ComposedDomainObject<TComposedInterface> : IDomainObject
      where TComposedInterface: class, IDomainObject
  {
    /// <summary>
    /// Used to create instances of the composite domain object class. When the complete interface is automatically detected via 
    /// <see cref="ComposedDomainObject{TComposedInterface}"/>, it is automatically guaranteed (by the mixin engine) that the derived class has all 
    /// parts of the composed interface. Until then, the method checks that this is the case and throws an exception if otherwise.
    /// </summary>
    /// <typeparam name="TComposite">The type of the composite domain object.</typeparam>
    /// <param name="ctorArgs">The constructor arguments.</param>
    /// <returns>An instance of <see cref="TComposite"/>, cased to <see cref="TComposedInterface"/>.</returns>
    protected static TComposedInterface NewObject<TComposite> (ParamList ctorArgs)
        where TComposite: ComposedDomainObject<TComposedInterface>
    {
      var result = ObjectFactory.Create<TComposite> (ctorArgs) as TComposedInterface;
      if (result == null)
      {
        var message = string.Format (
            "Type '{0}' does not implement the complete interface '{1}'. Did you forget the CompleteInterfaceAttribute?",
            typeof (TComposite),
            typeof (TComposedInterface).Name);
        throw new InvalidOperationException (message);
      }

      return result;
    }

    private readonly Guid _id;
    private readonly DomainObjectEventSource _events;

    protected ComposedDomainObject ()
    {
      _id = Guid.NewGuid();
      _events = new DomainObjectEventSource (this);

      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      OnReferenceInitializing();
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    protected virtual void OnReferenceInitializing ()
    {
      var mixinTarget = this as IMixinTarget;
      if (mixinTarget != null)
      {
        foreach (var mixin in mixinTarget.Mixins.OfType<IDomainObjectMixin>())
          mixin.OnTargetReferenceInitializing();
      }
    }

    /// <summary>
    /// Gets this <see cref="ComposedDomainObject{TComposedInterface}"/> instance, cast to the <see cref="TComposedInterface"/> type. This enables
    /// callers to access the members of all composed mixins.
    /// </summary>
    /// <value>The this.</value>
    protected TComposedInterface This
    {
      get
      {
        IDomainObject thisAsDomainObject = this;
        return (TComposedInterface) thisAsDomainObject;
      }
    }

    public Guid ID
    {
      get { return _id; }
    }

    public DomainObjectEventSource Events
    {
      get { return _events; }
    }
  }
}