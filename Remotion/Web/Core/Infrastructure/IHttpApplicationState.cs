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
using System.Collections;
using System.Collections.Specialized;
using System.Web;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="IHttpApplicationState"/> interface defines a wrapper for the <see cref="HttpApplicationState"/> type.
  /// </summary>
  public interface IHttpApplicationState : ICollection
  {
    /// <summary>
    /// Gets a <see cref="T:System.Collections.Specialized.NameObjectCollectionBase.KeysCollection"/> instance that contains all the keys in the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameObjectCollectionBase.KeysCollection"/> instance that contains all the keys in the <see cref="T:System.Collections.Specialized.NameObjectCollectionBase"/> instance.
    /// </returns>
    NameObjectCollectionBase.KeysCollection Keys { get; }

    /// <summary>
    /// Gets the access keys in the <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <returns>
    /// A string array of <see cref="T:System.Web.HttpApplicationState"/> object names.
    /// </returns>
    string[] AllKeys { get; }

    /// <summary>
    /// Gets a reference to the <see cref="T:System.Web.HttpApplicationState"/> object.
    /// </summary>
    /// <returns>
    /// A reference to the <see cref="T:System.Web.HttpApplicationState"/> object.
    /// </returns>
    HttpApplicationState Contents { get; }

    /// <summary>
    /// Gets all objects declared by an &lt;object&gt; tag where the scope is set to "Application" within the ASP.NET application.
    /// </summary>
    /// <returns>
    /// A collection of objects on the page.
    /// </returns>
    HttpStaticObjectsCollection StaticObjects { get; }

    /// <summary>
    /// Adds a new object to the <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <param name="name">The name of the object to be added to the collection. 
    ///                 </param><param name="value">The value of the object. 
    ///                 </param>
    void Add (string name, object value);

    /// <summary>
    /// Updates the value of an object in an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <param name="name">The name of the object to be updated. 
    ///                 </param><param name="value">The updated value of the object. 
    ///                 </param>
    void Set (string name, object value);

    /// <summary>
    /// Removes the named object from an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    /// <param name="name">The name of the object to be removed from the collection. 
    ///                 </param>
    void Remove (string name);

    /// <summary>
    /// Removes an <see cref="T:System.Web.HttpApplicationState"/> object from a collection by index.
    /// </summary>
    /// <param name="index">The position in the collection of the item to remove. 
    ///                 </param>
    void RemoveAt (int index);

    /// <summary>
    /// Removes all objects from an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    void Clear ();

    /// <summary>
    /// Removes all objects from an <see cref="T:System.Web.HttpApplicationState"/> collection.
    /// </summary>
    void RemoveAll ();

    /// <summary>
    /// Gets an <see cref="T:System.Web.HttpApplicationState"/> object by name.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="name"/>.
    /// </returns>
    /// <param name="name">The name of the object. 
    ///                 </param>
    object Get (string name);

    /// <summary>
    /// Gets an <see cref="T:System.Web.HttpApplicationState"/> object by numerical index.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="index"/>.
    /// </returns>
    /// <param name="index">The index of the application state object. 
    ///                 </param>
    object Get (int index);

    /// <summary>
    /// Gets an <see cref="T:System.Web.HttpApplicationState"/> object name by index.
    /// </summary>
    /// <returns>
    /// The name under which the application state object was saved.
    /// </returns>
    /// <param name="index">The index of the application state object. 
    ///                 </param>
    string GetKey (int index);

    /// <summary>
    /// Locks access to an <see cref="T:System.Web.HttpApplicationState"/> variable to facilitate access synchronization.
    /// </summary>
    void Lock ();

    /// <summary>
    /// Unlocks access to an <see cref="T:System.Web.HttpApplicationState"/> variable to facilitate access synchronization.
    /// </summary>
    void UnLock ();

    /// <summary>
    /// Gets the value of a single <see cref="T:System.Web.HttpApplicationState"/> object by name.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="name"/>.
    /// </returns>
    /// <param name="name">The name of the object in the collection. 
    ///                 </param>
    object this [string name] { get; set; }

    /// <summary>
    /// Gets a single <see cref="T:System.Web.HttpApplicationState"/> object by index.
    /// </summary>
    /// <returns>
    /// The object referenced by <paramref name="index"/>.
    /// </returns>
    /// <param name="index">The numerical index of the object in the collection. 
    ///                 </param>
    object this [int index] { get; }
  }
}
