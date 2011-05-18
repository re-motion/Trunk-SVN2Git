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
using NUnit.Framework;

namespace Remotion.Data.UnitTests.DomainObjects.UberProfIntegration
{
  public class FakeLinqToSqlProfiler
  {
    private static bool s_initialized;
    private static readonly object s_initializedLock = new object ();

    public static void Initialize ()
    {
      lock (s_initializedLock)
      {
        Assert.That (s_initialized, Is.False, "Initialize must not be called twice.");
        s_initialized = true;
      }
    }

    public static MockableLinqToSqlAppender GetAppender (string name)
    {
      lock (s_initializedLock)
      {
        Assert.That (s_initialized, Is.True, "Initialize must be called before GetAppender.");
        s_initialized = false;
      }

      return new MockableLinqToSqlAppender (name);
    }
  }
}