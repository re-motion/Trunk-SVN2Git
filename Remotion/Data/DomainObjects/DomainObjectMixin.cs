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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Base class for mixins adding persistent properties to domain objects.
  /// </summary>
  /// <typeparam name="TDomainObject">The type of the <see cref="Mixin{TThis}.This"/> property within the mixin. This type must be assignable from
  /// the domain object extended by the mixin. Note that an <see cref="ExtendsAttribute"/> or
  /// <see cref="UsesAttribute"/> is still required to actually apply the mixin to the domain object type.</typeparam>
  /// <remarks>Use this base class to implement a mixin adding persistent properties to a domain object which does not need to call any
  /// overridden base methods. Use <see cref="DomainObjectMixin{TDomainObject,TBaseCallRequirements}"/> if the mixin needs to call overridden
  /// base methods.</remarks>
  [Serializable]
  public class DomainObjectMixin<TDomainObject> : DomainObjectMixin<TDomainObject, IDomainObjectBaseCallRequirements>
    where TDomainObject : DomainObject
  {
  }

  /// <summary>
  /// Base class for mixins adding persistent properties to domain objects.
  /// </summary>
  /// <typeparam name="TDomainObject">The type of the <see cref="Mixin{TThis}.This"/> property within the mixin. This type must be assignable from
  /// the domain object extended by the mixin. Note that an <see cref="ExtendsAttribute"/> or
  /// <see cref="UsesAttribute"/> is still required to actually apply the mixin to the domain object type.</typeparam>
  /// <typeparam name="TBaseCallRequirements">An interface type specifying the members whose base implementation needs to be called via the
  /// <see cref="Mixin{TThis,TBase}.Base"/> property when overridden by this mixin. The interface needs to implement
  /// <see cref="IDomainObjectBaseCallRequirements"/>.</typeparam>
  /// <remarks><para>Use this base class to implement a mixin adding persistent properties to a domain object which overrides mixin members and needs to
  /// call the base implementations of these members on its target object. Specify those members you need to call via the
  /// <see cref="Mixin{TThis,TBase}.Base"/> property via the <typeparamref name="TBaseCallRequirements"/> type parameter; the target object does not
  /// have to actually implement this interface.</para>
  /// <para>Use <see cref="DomainObjectMixin{TDomainObject}"/> if the mixin does not need to call any base implementations of overridden members.</para></remarks>
  [NonIntroduced (typeof (IDomainObjectMixin))]
  [Serializable]
  public class DomainObjectMixin<TDomainObject, TBaseCallRequirements>
      : Mixin<TDomainObject, TBaseCallRequirements>, IDomainObjectMixin
      where TDomainObject : DomainObject
      where TBaseCallRequirements : class, IDomainObjectBaseCallRequirements
  {
    /// <summary>
    /// Gets the <see cref="ObjectID"/> of this mixin's target object.
    /// </summary>
    /// <value>The <see cref="ObjectID"/> of this mixin's target object.</value>
    [StorageClassNone]
    protected ObjectID ID
    {
      get { return This.ID; }
    }

    /// <summary>
    /// Gets the type returned by <see cref="DomainObject.GetPublicDomainObjectType"/> when called on this mixin's target object.
    /// </summary>
    /// <value>The public domain object type of this mixin's target object.</value>
    protected Type GetPublicDomainObjectType ()
    {
      return This.GetPublicDomainObjectType ();
    }

    /// <summary>
    /// Gets the <see cref="StateType"/> returned by this mixin's target object's <see cref="DomainObject.State"/> property.
    /// </summary>
    /// <value>The state of this mixin's target object.</value>
    [StorageClassNone]
    protected StateType State
    {
      get { return This.State; }
    }

    /// <summary>
    /// Gets a value indicating whether this mixin's target object is discarded.
    /// </summary>
    /// <value>true if this mixin's target object is discarded; otherwise, false.</value>
    [StorageClassNone]
    protected bool IsDiscarded
    {
      get { return This.IsDiscarded; }
    }

    /// <summary>
    /// Gets the properties of this mixin's target object, as returned by the <see cref="DomainObject.Properties"/> property.
    /// </summary>
    /// <value>The properties of the mixin's targetr object.</value>
    [StorageClassNone]
    protected PropertyIndexer Properties
    {
      get { return This.Properties; }
    }

    void IDomainObjectMixin.OnDomainObjectCreated ()
    {
      OnDomainObjectCreated ();
    }

    /// <summary>
    /// Called when the mixin's target domain object has been created.
    /// </summary>
    protected virtual void OnDomainObjectCreated ()
    {
    }

    void IDomainObjectMixin.OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnDomainObjectLoaded (loadMode);
    }

    /// <summary>
    /// Called when the mixin's target domain object has been loaded.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="DataContainer"/> has been
    /// newly loaded.</param>
    protected virtual void OnDomainObjectLoaded (LoadMode loadMode)
    {
    }
  }

  /// <summary>
  /// Describes the minimum base call requirements that <see cref="DomainObjectMixin{TDomainObject,TBaseCallRequirements}"/> has to its target
  /// objects.
  /// </summary>
  public interface IDomainObjectBaseCallRequirements
  {
    /// <summary>
    /// Defines that the mixin's target object must have a property called Properties which returns an object of type <see cref="PropertyIndexer"/>.
    /// The <see cref="DomainObject"/> base class already defines this property.
    /// </summary>
    PropertyIndexer Properties { get; }
  }
}
