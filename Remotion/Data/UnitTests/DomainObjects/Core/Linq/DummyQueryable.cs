// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
