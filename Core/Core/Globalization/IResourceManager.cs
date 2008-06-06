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
using System.Collections.Specialized;

namespace Remotion.Globalization
{
  /// <summary>
  ///   An interface for defining a string resource manager.
  /// </summary>
  public interface IResourceManager : INullObject
  {
    /// <summary>
    ///   Returns all string resources inside the resource manager.
    /// </summary>
    /// <returns>
    ///   A collection of string pairs, the key being the resource's ID, the value being the string.
    /// </returns>
    NameValueCollection GetAllStrings ();

    /// <summary>
    ///   Searches for all string resources inside the resource manager whose name is prefixed 
    ///   with a matching tag.
    /// </summary>
    /// <param name="prefix"> The prefix all returned string resources must have. </param>
    /// <returns>
    ///   A collection of string pairs, the key being the resource's ID, the value being the string.
    /// </returns>
    NameValueCollection GetAllStrings (string prefix);

    /// <summary>
    ///   Gets the value of the specified String resource.
    /// </summary>
    /// <param name="id">The ID of the resource to get. </param>
    /// <returns>
    ///   The value of the resource. If no match is possible, the identifier is returned.
    /// </returns>
    string GetString (string id);

    /// <summary>
    ///   Gets the value of the specified string resource. The resource is identified by
    ///   concatenating type and value name.
    /// </summary>
    /// <remarks> See <see cref="ResourceIdentifiersAttribute.GetResourceIdentifier"/> for resource identifier syntax. </remarks>
    /// <returns>
    ///   The value of the resource. If no match is possible, the identifier is returned.
    /// </returns>
    string GetString (Enum enumValue);

    /// <summary>Tests whether the <see cref="IResourceManager"/> contains the specified resource.</summary>
    /// <param name="id">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    bool ContainsResource (string id);

    /// <summary>Tests whether the <see cref="IResourceManager"/> contains the specified resource.</summary>
    /// <param name="enumValue">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    bool ContainsResource (Enum enumValue);

    string Name { get; }
  }
}
