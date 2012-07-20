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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents a <see cref="DomainObject"/> that can be instantiated (via <see cref="NewObject(Remotion.Reflection.ParamList)"/>), retrieved (via
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
  public abstract class SimpleDomainObject<TDomainObject> : BindableDomainObject
      where TDomainObject : SimpleDomainObject<TDomainObject>
  {
    /// <summary>
    /// Returns a new instance of a concrete domain object for the current<see cref="DomainObjects.ClientTransaction"/>. The instance is constructed
    /// with the default constructor.
    /// </summary>
    /// <returns>A new <typeparamref name="TDomainObject"/> instance.</returns>
    /// <remarks>
    /// <para>
    /// This method is identical to <see cref="DomainObject.NewObject{T}()"/>, but it can be called from any other class whereas
    /// <see cref="DomainObject.NewObject{T}()"/> can only be called from classes derived from <see cref="DomainObject"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="DomainObject.NewObject{T}()"/>
    /// <exception cref="ArgumentException">The type <typeparamref name="TDomainObject"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The given type <typeparamref name="TDomainObject"/> does not implement the required protected
    /// constructor.
    /// </exception>
    public static TDomainObject NewObject ()
    {
      return DomainObject.NewObject<TDomainObject>();
    }

    /// <summary>
    /// Returns a new instance of a concrete domain object for the current<see cref="DomainObjects.ClientTransaction"/>. The instance is constructed
    /// with a constructor accepting the given parameter list.
    /// </summary>
    /// <param name="constructorParameters">A <see cref="ParamList"/> object holding the parameters to be passed to the constructor. Instantiate this
    /// by using one of the <see cref="ParamList.Create{A1,A2}"/> methods.</param>
    /// <returns>A new <typeparamref name="TDomainObject"/> instance.</returns>
    /// <remarks>
    /// <para>
    /// This method is identical to <see cref="DomainObject.NewObject{T}(ParamList)"/>, but it can be called from any other class whereas
    /// <see cref="DomainObject.NewObject{T}(ParamList)"/> can only be called from classes derived from <see cref="DomainObject"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="DomainObject.NewObject{T}(ParamList)"/>
    /// <exception cref="ArgumentException">The type <typeparamref name="TDomainObject"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The given type <typeparamref name="TDomainObject"/> does not implement the required protected
    /// constructor.
    /// </exception>
    public static TDomainObject NewObject (ParamList constructorParameters)
    {
      ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);
      return DomainObject.NewObject<TDomainObject> (constructorParameters);
    }

    /// <summary>
    /// Gets a <see cref="SimpleDomainObject{TDomainObject}"/> that is already loaded or attempts to load it from the data source.
    /// If the object's data can't be found, an 
    /// exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>.
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
    /// <exception cref="ObjectsNotFoundException">
    /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
    /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
    /// <see cref="ObjectInvalidException"/> being thrown.
    /// </exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the data source.
    /// </exception>
    /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
    /// <exception cref="ObjectDeletedException">The object has already been deleted.</exception>
    /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="TDomainObject"/>.</exception>
    public static TDomainObject GetObject (ObjectID id)
    {
      return DomainObject.GetObject<TDomainObject> (id);
    }

    /// <summary>
    /// Gets a <see cref="SimpleDomainObject{TDomainObject}"/> that is already loaded or attempts to load it from the data source.
    /// If the object's data can't be found, an 
    /// exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>.
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
    /// <exception cref="ObjectsNotFoundException">
    /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
    /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
    /// <see cref="ObjectInvalidException"/> being thrown.
    /// </exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the data source.
    /// </exception>
    /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
    /// <exception cref="ObjectDeletedException">The object has already been deleted and the <paramref name="includeDeleted"/> flag is 
    /// <see langword="false" />.</exception>
    /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="TDomainObject"/>.</exception>
    public static TDomainObject GetObject (ObjectID id, bool includeDeleted)
    {
      return DomainObject.GetObject<TDomainObject> (id, includeDeleted);
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that already exists or attempts to load it from the data source. 
    /// If an object cannot be found, it will be marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>, and the method will
    /// return a <see langword="null" /> reference in its place.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <returns>
    /// The <see cref="DomainObject"/> with the specified <paramref name="id"/>, or <see langword="null" /> if it couldn't be found.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the data source.
    /// </exception>
    /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="TDomainObject"/>.</exception>
    public static TDomainObject TryGetObject (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      return DomainObject.TryGetObject<TDomainObject> (id);
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
    /// <exception cref="ObjectInvalidException">The object is invalid. See <see cref="ObjectInvalidException"/> for further information.</exception>
    /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="DomainObject.OnDeleting"/> and <see cref="DomainObject.OnDeleted"/> should be overridden.</remarks>
    public new void Delete ()
    {
      base.Delete();
    }
  }
}
