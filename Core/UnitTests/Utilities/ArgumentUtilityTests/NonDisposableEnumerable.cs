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

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  class NonDisposableEnumerable : IEnumerable
  {
    private readonly bool _hasElements;

    public NonDisposableEnumerable(bool hasElements)
    {
      _hasElements = hasElements;
    }

    private class Enumerator : IEnumerator
    {
      private readonly bool _hasNext;

      public Enumerator(bool hasNext)
      {
        _hasNext = hasNext;
      }

      public bool MoveNext()
      {
        return _hasNext;
      }

      public void Reset()
      {
        throw new NotImplementedException();
      }

      public object Current
      {
        get { return 7; }
      }
    }

    public IEnumerator GetEnumerator()
    {
      return new Enumerator (_hasElements);
    }
  }
}