// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="HttpServerUtilityWrapper"/> type is the default implementation of the <see cref="IHttpServerUtility"/> interface.
  /// </summary>
  public class HttpApplicationStateWrapper : IHttpApplicationState
  {
    private readonly HttpApplicationState _applicationState;

    public HttpApplicationStateWrapper (HttpApplicationState applicationState)
    {
      ArgumentUtility.CheckNotNull ("applicationState", applicationState);
      _applicationState = applicationState;
    }

    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpApplicationState"/> wrapper.
    /// </summary>
    public HttpApplicationState WrappedInstance
    {
      get { return _applicationState; }
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. 
    /// </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. 
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. 
    /// </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. 
    /// </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
    ///     -or- 
    /// <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
    ///     -or- 
    ///     The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. 
    /// </exception><exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. 
    /// </exception><filterpriority>2</filterpriority>
    void ICollection.CopyTo (Array array, int index)
    {
      ((ICollection) _applicationState).CopyTo (array, index);
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    object ICollection.SyncRoot
    {
      get { return ((ICollection) _applicationState).SyncRoot; }
    }

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
    /// </summary>
    /// <returns>
    /// true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    bool ICollection.IsSynchronized
    {
      get { return ((ICollection) _applicationState).IsSynchronized; }
    }

    /// <summary>
    /// Implements the <see cref="T:System.Runtime.Serialization.ISerializable"/> interface and returns the data needed to serialize the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </summary>
    /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object that contains the information required to serialize the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </param><param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext"/> object that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="info"/> is null.
    /// </exception>
    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      _applicationState.GetObjectData (info, context);
    }

    /// <summary>
    /// Implements the <see cref="T:System.Runtime.Serialization.ISerializable"/> interface and raises the deserialization event when the deserialization is complete.
    /// </summary>
    /// <param name="sender">The source of the deserialization event.
    /// </param><exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object associated with the current <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance is invalid.
    /// </exception>
    public void OnDeserialization (object sender)
    {
      _applicationState.OnDeserialization (sender);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> for the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </returns>
    public IEnumerator GetEnumerator ()
    {
      return _applicationState.GetEnumerator();
    }

    /// <summary>
    /// Gets a <see cref="T:System.Collections.Specialized.NameObjectCollectionBase.KeysCollection"/> instance that contains all the keys in the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameObjectCollectionBase.KeysCollection"/> instance that contains all the keys in the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </returns>
    public NameObjectCollectionBase.KeysCollection Keys
    {
      get { return _applicationState.Keys; }
    }

    /// <summary>
    /// Adds a new object to the <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <param name="name">The name of the object to be added to the collection. 
    /// </param><param name="value">The value of the object. 
    /// </param>
    public void Add (string name, object value)
    {
      _applicationState.Add (name, value);
    }

    /// <summary>
    /// Updates the value of an object in an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <param name="name">The name of the object to be updated. 
    /// </param><param name="value">The updated value of the object. 
    /// </param>
    public void Set (string name, object value)
    {
      _applicationState.Set (name, value);
    }

    /// <summary>
    /// Removes the named object from an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <param name="name">The name of the object to be removed from the collection. 
    /// </param>
    public void Remove (string name)
    {
      _applicationState.Remove (name);
    }

    /// <summary>
    /// Removes an <see cref="T:System.Web.HttpApplicationState"/> object from a collection by index.
    /// </summary>
    /// <param name="index">The position in the collection of the item to remove. 
    /// </param>
    public void RemoveAt (int index)
    {
      _applicationState.RemoveAt (index);
    }

    /// <summary>
    /// Removes all objects from an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    public void Clear ()
    {
      _applicationState.Clear();
    }

    /// <summary>
    /// Removes all objects from an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    public void RemoveAll ()
    {
      _applicationState.RemoveAll();
    }

    /// <summary>
    /// Gets an <see cref="T:System.Web.HttpApplicationState"/> object by name.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="name"/>.
    /// </returns>
    /// <param name="name">The name of the object. 
    /// </param>
    public object Get (string name)
    {
      return _applicationState.Get (name);
    }

    /// <summary>
    /// Gets an <see cref="T:System.Web.HttpApplicationState"/> object by numerical index.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="index"/>.
    /// </returns>
    /// <param name="index">The index of the application state object. 
    /// </param>
    public object Get (int index)
    {
      return _applicationState.Get (index);
    }

    /// <summary>
    /// Gets an <see cref="T:System.Web.HttpApplicationState"/> object name by index.
    /// </summary>
    /// <returns>
    /// The name under which the application state object was saved.
    /// </returns>
    /// <param name="index">The index of the application state object. 
    /// </param>
    public string GetKey (int index)
    {
      return _applicationState.GetKey (index);
    }

    /// <summary>
    /// Locks access to an <see cref="T:System.Web.HttpApplicationState"/> variable to facilitate access synchronization.
    /// </summary>
    public void Lock ()
    {
      _applicationState.Lock();
    }

    /// <summary>
    /// Unlocks access to an <see cref="T:System.Web.HttpApplicationState"/> variable to facilitate access synchronization.
    /// </summary>
    public void UnLock ()
    {
      _applicationState.UnLock();
    }

    /// <summary>
    /// Gets the number of objects in the <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <returns>
    /// The number of item objects in the collection. The default is 0.
    /// </returns>
    public int Count
    {
      get { return _applicationState.Count; }
    }

    /// <summary>
    /// Gets the value of a single <see cref="T:System.Web.HttpApplicationState"/> object by name.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="name"/>.
    /// </returns>
    /// <param name="name">The name of the object in the collection. 
    /// </param>
    public object this [string name]
    {
      get { return _applicationState[name]; }
      set { _applicationState[name] = value; }
    }

    /// <summary>
    /// Gets a single <see cref="T:System.Web.HttpApplicationState"/> object by index.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="index"/>.
    /// </returns>
    /// <param name="index">The numerical index of the object in the collection. 
    /// </param>
    public object this [int index]
    {
      get { return _applicationState[index]; }
    }

    /// <summary>
    /// Gets the access keys in the <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <returns>
    /// A string array of <see cref="T:System.Web.HttpApplicationState"/> object names.
    /// </returns>
    public string[] AllKeys
    {
      get { return _applicationState.AllKeys; }
    }

    /// <summary>
    /// Gets a reference to the <see cref="T:System.Web.HttpApplicationState"/> object.
    /// </summary>
    /// <returns>
    /// A reference to the <see cref="T:System.Web.HttpApplicationState"/> object.
    /// </returns>
    public HttpApplicationState Contents
    {
      get { return _applicationState.Contents; }
    }

    /// <summary>
    /// Gets all objects declared by an &lt;object&gt; tag where the scope is set to "Application" within the ASP.NET application.
    /// </summary>
    /// <returns>
    /// A collection of objects on the page.
    /// </returns>
    public HttpStaticObjectsCollection StaticObjects
    {
      get { return _applicationState.StaticObjects; }
    }
  }
}