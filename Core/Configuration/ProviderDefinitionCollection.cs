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
using System.ComponentModel;
using System.Configuration.Provider;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>Represents a collection of provider objects that inherit from <typeparamref name="T"/>.</summary>
  /// <typeparam name="T">The type of elements in the list.</typeparam>
  public class ProviderCollection<T>: ProviderCollection where T: class
  {
    /// <summary>Initializes a new instance of the <see cref="ProviderCollection{T}"/> class.</summary>
    public ProviderCollection()
    {
    }

    /// <summary>Gets the provider with the specified name.</summary>
    /// <param name="name">The key by which the provider is identified.</param>
    /// <returns>The provider with the specified name.</returns>
    public new T this[string name] 
    {
      get { return (T) (object) base[name]; }
    }

    /// <summary>Adds a provider to the collection.</summary>
    /// <param name="provider">The provider to be added.</param>
    /// <exception cref="System.ArgumentException">
    /// The <see cref="ProviderBase.Name"/> of <paramref name="provider"/> is <see langword="null"/>.<para>- or -</para>
    /// The length of the <see cref="ProviderBase.Name"/> of <paramref name="provider"/> is less than 1.
    /// </exception>
    /// <exception cref="ArgumentTypeException">The <paramref name="provider"/> is not derived from <see cref="ProviderBase"/>.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="provider"/> is null.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void Add (T provider)
    {
      base.Add (ArgumentUtility.CheckType<ProviderBase> ("provider", provider));
    }

    /// <summary>Adds a provider to the collection.</summary>
    /// <param name="provider">The provider to be added.</param>
    /// <exception cref="System.ArgumentException">
    /// The <see cref="ProviderBase.Name"/> of <paramref name="provider"/> is <see langword="null"/>.<para>- or -</para>
    /// The length of the <see cref="ProviderBase.Name"/> of <paramref name="provider"/> is less than 1.<para>- or -</para>
    /// </exception>
    /// <exception cref="ArgumentTypeException">The <paramref name="provider"/> is not assignable to <typeparamref name="T"/>.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="provider"/> is null.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public override void Add (ProviderBase provider)
    {
      ArgumentUtility.CheckType<T> ("provider", provider);
      base.Add (provider);
    }

    ///// <summary>Copies the contents of the collection to the given array starting at the specified index.</summary>
    ///// <param name="array">The array to copy the elements of the collection to.</param>
    ///// <param name="index">The index of the collection item at which to start the copying process.</param>
    //public void CopyTo (T[] array, int index)
    //{
    //  base.CopyTo (array, index);
    //}

    public T GetMandatory (string name)
    {
      T value = this[name];
      if (value == null)
        throw new ArgumentException (string.Format ("Provider '{0}' does not exist.", name));

      return value;
    }
    
  }
}
