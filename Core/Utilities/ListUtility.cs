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

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides utility methods for processing IList instances. 
  /// </summary>
  public static class ListUtility
  {
    public static void CopyTo (IList source, IList destination)
    {
      int len = Math.Min (source.Count, destination.Count);
      for (int i = 0; i < len; ++i)
        destination[i] = source[i];
    }

    public static int IndexOf (IList list, object value)
    {
      ArgumentUtility.CheckNotNull ("list", list);
      try
      {
        return list.IndexOf (value);
      }
      catch (NotSupportedException)
      {
        return IndexOfInternal (list, value);
      }
    }

    public static int[] IndicesOf (IList list, IList values)
    {
      return IndicesOf (list, values, true);
    }

    public static int[] IndicesOf (IList list, IList values, bool includeMissing)
    {
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("values", values);

      ArrayList indices = new ArrayList (values.Count);
      bool isIndexOfSupported = true;
      for (int i = 0; i < values.Count; i++)
      {
        int index = -1;
        if (isIndexOfSupported)
        {
          try
          {
            index = list.IndexOf (values[i]);
          }
          catch (NotSupportedException)
          {
            isIndexOfSupported = false;
          }
        }

        if (! isIndexOfSupported)
          index = IndexOfInternal (list, values[i]);

        if (index > -1 || includeMissing)
          indices.Add (index);
      }
      return (int[]) indices.ToArray (typeof (int));
    }

    private static int IndexOfInternal (IList list, object value)
    {
      for (int i = 0; i < list.Count; i++)
      {
        if (list[i] == value)
          return i;
      }
      return -1;
    }
  }
}
