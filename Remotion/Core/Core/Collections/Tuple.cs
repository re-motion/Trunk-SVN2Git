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

namespace Remotion.Collections
{
  /// <summary>
  /// Provides static methods for creating tuple objects.
  /// </summary>
  public static class Tuple
  {
    public static Tuple<TA, TB> Create<TA, TB> (TA a, TB b)
    {
      return new Tuple<TA, TB> (a, b);
    }

    public static Tuple<TA, TB, TC> Create<TA, TB, TC> (TA a, TB b, TC c)
    {
      return new Tuple<TA, TB, TC> (a, b, c);
    }

    public static Tuple<TA, TB, TC, TD> Create<TA, TB, TC, TD> (TA a, TB b, TC c, TD d)
    {
      return new Tuple<TA, TB, TC, TD> (a, b, c, d);
    }

    [Obsolete ("Use Tuple.Create instead. (Version 1.13.47)")]
    public static Tuple<TA, TB> NewTuple<TA, TB> (TA a, TB b)
    {
      return Tuple.Create (a, b);
    }

    [Obsolete ("Use Tuple.Create instead. (Version 1.13.47)")]
    public static Tuple<TA, TB, TC> NewTuple<TA, TB, TC> (TA a, TB b, TC c)
    {
      return Tuple.Create (a, b, c);
    }

    [Obsolete ("Use Tuple.Create instead. (Version 1.13.47)")]
    public static Tuple<TA, TB, TC, TD> NewTuple<TA, TB, TC, TD> (TA a, TB b, TC c, TD d)
    {
      return Tuple.Create (a, b, c, d);
    }
  }
}
