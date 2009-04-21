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
using NUnit.Framework;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UnitTests
{
  [TestFixture]
  public class DisposableMixinTest
  {
    public class Data
    {
      public bool ManagedCalled = false;
      public bool UnmanagedCalled = false;
    }

    [Uses (typeof (DisposableMixin))]
    public class C
    {
      public Data Data = new Data();

      [OverrideMixin]
      public void CleanupManagedResources()
      {
        Data.ManagedCalled = true;
      }

      [OverrideMixin]
      public void CleanupUnmanagedResources ()
      {
        Data.UnmanagedCalled = true;
      }
    }

    [Test]
    public void DisposeCallsAllCleanupMethods()
    {
      C c = ObjectFactory.Create<C>(ParamList.Empty);
      Data data = c.Data;

      Assert.IsFalse (data.ManagedCalled);
      Assert.IsFalse (data.UnmanagedCalled);
      
      using ((IDisposable)c)
      {
        Assert.IsFalse (data.ManagedCalled);
        Assert.IsFalse (data.UnmanagedCalled);
      }
      Assert.IsTrue (data.ManagedCalled);
      Assert.IsTrue (data.UnmanagedCalled);
      GC.KeepAlive (c);
    }

    [Test]
    public void GCCallsAllUnmanagedCleanup ()
    {
      C c = ObjectFactory.Create<C> (ParamList.Empty);
      Data data = c.Data;

      Assert.IsFalse (data.ManagedCalled);
      Assert.IsFalse (data.UnmanagedCalled);

      GC.KeepAlive (c);
      c = null;

      GC.Collect ();
      GC.WaitForPendingFinalizers();

      Assert.IsFalse (data.ManagedCalled);
      Assert.IsTrue (data.UnmanagedCalled);
    }
  }
}
