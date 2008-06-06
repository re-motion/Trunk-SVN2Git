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

namespace Remotion.Collections
{
  public static class Tuple
  {
    public static Tuple<TA, TB> NewTuple<TA, TB> (TA a, TB b)
    {
      return new Tuple<TA, TB> (a, b);
    }

    public static Tuple<TA, TB, TC> NewTuple<TA, TB, TC> (TA a, TB b, TC c)
    {
      return new Tuple<TA, TB, TC> (a, b, c);
    }

    public static Tuple<TA, TB, TC, TD> NewTuple<TA, TB, TC, TD> (TA a, TB b, TC c, TD d)
    {
      return new Tuple<TA, TB, TC, TD> (a, b, c, d);
    }
  }
}
