/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Linq;

namespace Remotion.Data.DomainObjects.Linq
{
  public static class ExtensionMethods
  {
    public static ObjectList<T> ToObjectList<T> (this IEnumerable<T> source) 
        where T : DomainObject
    {
      ObjectList<T> result = new ObjectList<T>();
      foreach (T item in source)
        result.Add (item);
      return result;
    }
  }
}