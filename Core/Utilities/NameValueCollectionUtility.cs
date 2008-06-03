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

namespace Remotion.Utilities
{
  /// <summary>
  ///   Utility class for <see cref="NameValueCollection"/>
  /// </summary>
  public static class NameValueCollectionUtility
  {
    public static NameValueCollection Clone (NameValueCollection collection)
    {
      return new NameValueCollection (collection);
    }

    /// <summary>
    ///   Adds the second dictionary to the first. If a key occurs in both dictionaries, the value of the second
    ///   dictionaries is taken.
    /// </summary>
    /// <param name="first"> Must not be <see langword="null"/>. </param>
    /// <param name="second"> Must not be <see langword="null"/>. </param>
    public static void Append (NameValueCollection first, NameValueCollection second)
    {
      ArgumentUtility.CheckNotNull ("first", first);
      
      if (second != null)
      {
        for (int i = 0; i < second.Count; i++)
          first.Set (second.GetKey(i), second.Get(i));
      }
    }

    /// <summary>
    ///   Merges two collections. If a key occurs in both collections, the value of the second collections is taken.
    /// </summary>
    public static NameValueCollection Merge (NameValueCollection first, NameValueCollection second)
    {
      if (first == null && second == null)
        return null;
      else if (first != null && second == null)
        return Clone (first);
      else if (first == null && second != null)
        return Clone (second);

      NameValueCollection result = Clone (first);
      Append (result, second);
      return result;
    }
  }
}
