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
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.SessionState;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="HttpRequestWrapper"/> type is the default implementation of the <see cref="IHttpRequest"/> interface.
  /// </summary>
  public class HttpSessionStateWrapper : IHttpSessionState
  {
    private readonly HttpSessionState _sessionState;

    public HttpSessionStateWrapper (HttpSessionState sessionState)
    {
      ArgumentUtility.CheckNotNull ("sessionState", sessionState);

      _sessionState = sessionState;
    }

    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpSessionState"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="HttpRequest"/>. </exception>
    public HttpSessionState WrappedInstance
    {
      get { return _sessionState; }
    }

    /// <summary>
    /// Cancels the current session.
    /// </summary>
    public void Abandon ()
    {
      _sessionState.Abandon();
    }

    /// <summary>
    /// Adds a new item to the session-state collection.
    /// </summary>
    /// <param name="name">
    /// The name of the item to add to the session-state collection. 
    /// </param>
    /// <param name="value">
    /// The value of the item to add to the session-state collection. 
    /// </param>
    public void Add (string name, object value)
    {
      _sessionState.Add(name, value);
    }

    /// <summary>
    /// Deletes an item from the session-state collection.
    /// </summary>
    /// <param name="name">
    /// The name of the item to delete from the session-state collection. 
    /// </param>
    public void Remove (string name)
    {
      _sessionState.Remove(name);
    }

    /// <summary>
    /// Deletes an item at a specified index from the session-state collection.
    /// </summary>
    /// <param name="index">
    /// The index of the item to remove from the session-state collection. 
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.
    /// <para>- or -</para>
    /// <paramref name="index" /> is equal to or greater than <see cref="P:System.Web.SessionState.HttpSessionState.Count" />.
    /// </exception>
    public void RemoveAt (int index)
    {
      _sessionState.RemoveAt(index);
    }

    /// <summary>
    /// Removes all keys and values from the session-state collection.
    /// </summary>
    public void Clear ()
    {
      _sessionState.Clear();
    }

    /// <summary>
    /// Removes all keys and values from the session-state collection.
    /// </summary>
    public void RemoveAll ()
    {
      _sessionState.RemoveAll();
    }

    /// <summary>
    /// Returns an enumerator that can be used to read all the session-state variable names in the current session.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> that can iterate through the variable names in the session-state collection.
    /// </returns>
    public IEnumerator GetEnumerator ()
    {
      return _sessionState.GetEnumerator();
    }

    /// <summary>
    /// Copies the collection of session-state values to a one-dimensional array, starting at the specified index in the array.
    /// </summary>
    /// <param name="array">
    /// The <see cref="T:System.Array" /> that receives the session values. 
    /// </param>
    /// <param name="index">
    /// The zero-based index in <paramref name="array" /> from which copying starts. 
    /// </param>
    public void CopyTo (Array array, int index)
    {
      _sessionState.CopyTo(array, index);
    }

    /// <summary>
    /// Gets the unique identifier for the session.
    /// </summary>
    /// <returns>
    /// The unique session identifier.
    /// </returns>
    public string SessionID
    {
      get { return _sessionState.SessionID; }
    }

    /// <summary>
    /// Gets and sets the amount of time, in minutes, allowed between requests before the session-state provider terminates the session.
    /// </summary>
    /// <returns>
    /// The time-out period, in minutes.
    /// </returns>
    public int Timeout
    {
      get { return _sessionState.Timeout; }
      set { _sessionState.Timeout = value; }
    }

    /// <summary>
    /// Gets a value indicating whether the session was created with the current request.
    /// </summary>
    /// <returns>
    /// true if the session was created with the current request; otherwise, false.
    /// </returns>
    public bool IsNewSession
    {
      get { return _sessionState.IsNewSession; }
    }

    /// <summary>
    /// Gets the current session-state mode.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Web.SessionState.SessionStateMode" /> values.
    /// </returns>
    public SessionStateMode Mode
    {
      get { return _sessionState.Mode; }
    }

    /// <summary>
    /// Gets a value indicating whether the session ID is embedded in the URL or stored in an HTTP cookie.
    /// </summary>
    /// <returns>
    /// true if the session is embedded in the URL; otherwise, false.
    /// </returns>
    public bool IsCookieless
    {
      get { return _sessionState.IsCookieless; }
    }

    /// <summary>
    /// Gets a value that indicates whether the application is configured for cookieless sessions.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Web.HttpCookieMode" /> values that indicate whether the application is configured for cookieless sessions. The default is <see cref="F:System.Web.HttpCookieMode.UseCookies" />.
    /// </returns>
    public HttpCookieMode CookieMode
    {
      get { return _sessionState.CookieMode; }
    }

    /// <summary>
    /// Gets or sets the locale identifier (LCID) of the current session.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Globalization.CultureInfo" /> instance that specifies the culture of the current session.
    /// </returns>
    public int LCID
    {
      get { return _sessionState.LCID; }
      set { _sessionState.LCID = value; }
    }

    /// <summary>
    /// Gets or sets the character-set identifier for the current session.
    /// </summary>
    /// <returns>
    /// The character-set identifier for the current session.
    /// </returns>
    public int CodePage
    {
      get { return _sessionState.CodePage; }
      set { _sessionState.CodePage = value; }
    }

    /// <summary>
    /// Gets a reference to the current session-state object.
    /// </summary>
    /// <returns>
    /// The current <see cref="T:System.Web.SessionState.HttpSessionState" />.
    /// </returns>
    public HttpSessionState Contents
    {
      get { return _sessionState.Contents; }
    }

    /// <summary>
    /// Gets a collection of objects declared by &lt;object Runat="Server" Scope="Session"/&gt; tags within the ASP.NET application file Global.asax.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpStaticObjectsCollection" /> containing objects declared in the Global.asax file.
    /// </returns>
    public HttpStaticObjectsCollection StaticObjects
    {
      get { return _sessionState.StaticObjects; }
    }

    /// <summary>
    /// Gets or sets a session value by name.
    /// </summary>
    /// <returns>
    /// The session-state value with the specified name, or <see langword="null" /> if the item does not exist.
    /// </returns>
    /// <param name="name">
    /// The key name of the session value. 
    /// </param>
    public object this [string name]
    {
      get { return _sessionState[name]; }
      set { _sessionState[name] = value; }
    }

    /// <summary>
    /// Gets or sets a session value by numerical index.
    /// </summary>
    /// <returns>
    /// The session-state value stored at the specified index, or <see langword="null" /> if the item does not exist.
    /// </returns>
    /// <param name="index">
    /// The numerical index of the session value. 
    /// </param>
    public object this [int index]
    {
      get { return _sessionState[index]; }
      set { _sessionState[index] = value; }
    }

    /// <summary>
    /// Gets the number of items in the session-state collection.
    /// </summary>
    /// <returns>
    /// The number of items in the collection.
    /// </returns>
    public int Count
    {
      get { return _sessionState.Count; }
    }

    /// <summary>
    /// Gets a collection of the keys for all values stored in the session-state collection.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Collections.Specialized.NameObjectCollectionBase.KeysCollection" /> that contains all the session keys.
    /// </returns>
    public NameObjectCollectionBase.KeysCollection Keys
    {
      get { return _sessionState.Keys; }
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the collection of session-state values.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the collection.
    /// </returns>
    public object SyncRoot
    {
      get { return _sessionState.SyncRoot; }
    }

    /// <summary>
    /// Gets a value indicating whether the session is read-only.
    /// </summary>
    /// <returns>
    /// true if the session is read-only; otherwise, false.
    /// </returns>
    public bool IsReadOnly
    {
      get { return _sessionState.IsReadOnly; }
    }

    /// <summary>
    /// Gets a value indicating whether access to the collection of session-state values is synchronized (thread safe).
    /// </summary>
    /// <returns>
    /// true if access to the collection is synchronized (thread safe); otherwise, false.
    /// </returns>
    public bool IsSynchronized
    {
      get { return _sessionState.IsSynchronized; }
    }
  }
}