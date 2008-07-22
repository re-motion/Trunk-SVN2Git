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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  public class DummyQueryable<T> : IQueryable<T>
  {
    IEnumerator<T> IEnumerable<T>.GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    public IEnumerator GetEnumerator ()
    {
      return ((IEnumerable<T>) this).GetEnumerator();
    }

    public Expression Expression
    {
      get { throw new NotImplementedException(); }
    }

    public Type ElementType
    {
      get { throw new NotImplementedException(); }
    }

    public IQueryProvider Provider
    {
      get { throw new NotImplementedException(); }
    }
  }
}