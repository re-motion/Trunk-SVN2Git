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

namespace Remotion.Context
{
  /// <summary>
  /// Common interface for classes implementing a storage mechanism for <see cref="SafeContext"/>.
  /// </summary>
  public interface ISafeContextStorageProvider
  {
    /// <summary>
    /// Retrieves a data item from the context storage.
    /// </summary>
    /// <param name="key">The key identifying the data item.</param>
    /// <returns>The data item identified by the given key, or <see langword="null"/> if no such item exists in the storage.</returns>
    object GetData (string key);

    /// <summary>
    /// Sets a data item in the context storage, overwriting a previous value identified by the same key.
    /// </summary>
    /// <param name="key">The key identifying the data item.</param>
    /// <param name="value">The value to be stored in the context storage.</param>
    void SetData (string key, object value);

    /// <summary>
    /// Frees the resources used by a specific data item in the context storage.
    /// </summary>
    /// <param name="key">The key identifying the data item to be freed.</param>
    void FreeData (string key);
  }
}
