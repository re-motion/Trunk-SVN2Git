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
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Specifies the type of items for properties returning an <see cref="IList"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ItemTypeAttribute : Attribute
  {
    private Type _itemType;

    /// <summary>
    /// Instantiates a new object.
    /// </summary>
    /// <param name="itemType">The type of items returned by the property. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="itemType"/> is <see langword="null"/>.</exception>
    public ItemTypeAttribute (Type itemType)
    {
      ArgumentUtility.CheckNotNull ("itemType", itemType);

      _itemType = itemType;
    }

    /// <summary>
    /// Gets the type of items returned by the property.
    /// </summary>
    public Type ItemType
    {
      get { return _itemType; }
    }
  }
}
