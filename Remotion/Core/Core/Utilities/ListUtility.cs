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
