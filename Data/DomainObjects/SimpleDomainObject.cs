/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Reflection;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents a <see cref="DomainObject"/> that can be instantiated (via <see cref="NewObject"/>), retrieved (via
  /// <see cref="GetObject(ObjectID)"/>), and deleted via public methods.
  /// </summary>
  /// <typeparam name="TDomainObject">The type derived from <see cref="SimpleDomainObject{TDomainObject}"/>.</typeparam>
  /// <remarks>
  /// The only difference between this class and <see cref="DomainObject"/> is that <see cref="SimpleDomainObject{TDomainObject}"/> has public
  /// methods for instantiation, retrieval, and deletion, whereas these methods are protected on <see cref="DomainObject"/>. Derive
  /// from <see cref="DomainObject"/> if you need to hide these methods from the public; derive from <see cref="SimpleDomainObject{TDomainObject}"/>
  /// if you don't.
  /// </remarks>
  [Serializable]
  public abstract class SimpleDomainObject<TDomainObject> : DomainObject
      where TDomainObject : SimpleDomainObject<TDomainObject>
  {
    /// <summary>
    /// Returns an invocation object creating a new instance of a concrete domain object for the current
    /// <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <returns>An <see cref="IFuncInvoker{T}"/> object used to create a new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// This method is identical to <see cref="DomainObject.NewObject{T}"/>, but can be called from any other class, whereas
    /// <see cref="DomainObject.NewObject{T}"/> can only be called from classes derived from <see cref="DomainObject"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="DomainObject.NewObject{T}"/>
    /// <exception cref="ArgumentException">The type <typeparamref name="TDomainObject"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The given type <typeparamref name="TDomainObject"/> does not implement the required protected
    /// constructor.
    /// </exception>
    public static IFuncInvoker<TDomainObject> NewObject ()
    {
      return DomainObject.NewObject<TDomainObject>();
    }

    /// <summary>
    /// Gets a <see cref="SimpleDomainObject{TDomainObject}"/> that is already loaded or attempts to load it from the datasource.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="SimpleDomainObject{TDomainObject}"/> that should be loaded. Must not be 
    /// <see langword="null"/>.</param>
    /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
    /// <remarks>
    /// <para>
    /// This method is identical to <see cref="DomainObject.GetObject{T}(ObjectID)"/>, but can be called from any other class, whereas
    /// <see cref="DomainObject.GetObject{T}(ObjectID)"/> can only be called from classes derived from <see cref="DomainObject"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="DomainObject.GetObject{T}(ObjectID)"/>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the datasource.
    /// </exception>
    /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
    /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="TDomainObject"/>.</exception>
    public static TDomainObject GetObject (ObjectID id)
    {
      return DomainObject.GetObject<TDomainObject> (id);
    }

    /// <summary>
    /// Gets a <see cref="SimpleDomainObject{TDomainObject}"/> that is already loaded or attempts to load it from the datasource.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="SimpleDomainObject{TDomainObject}"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <param name="includeDeleted">Indicates if the method should return <see cref="SimpleDomainObject{TDomainObject}"/>s that are already deleted.</param>
    /// <returns>The <see cref="SimpleDomainObject{TDomainObject}"/> with the specified <paramref name="id"/>.</returns>
    /// <remarks>
    /// <para>
    /// This method is identical to <see cref="DomainObject.GetObject{T}(ObjectID,bool)"/>, but can be called from any other class, whereas
    /// <see cref="DomainObject.GetObject{T}(ObjectID,bool)"/> can only be called from classes derived from <see cref="DomainObject"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="DomainObject.GetObject{T}(ObjectID,bool)"/>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the datasource.
    /// </exception>
    /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
    /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="TDomainObject"/>.</exception>
    public static TDomainObject GetObject (ObjectID id, bool includeDeleted)
    {
      return DomainObject.GetObject<TDomainObject> (id, includeDeleted);
    }

    protected SimpleDomainObject ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleDomainObject{TDomainObject}"/> class in the process of deserialization.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> coming from the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> coming from the .NET serialization infrastructure.</param>
    /// <remarks>Be sure to call this base constructor from the deserialization constructor of any concrete <see cref="SimpleDomainObject{TDomainObject}"/> type
    /// implementing the <see cref="ISerializable"/> interface.</remarks>
    protected SimpleDomainObject (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    /// <summary>
    /// Deletes the <see cref="SimpleDomainObject{TDomainObject}"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="DomainObject.OnDeleting"/> and <see cref="DomainObject.OnDeleted"/> should be overridden.</remarks>
    public new void Delete ()
    {
      base.Delete();
    }
  }
}
