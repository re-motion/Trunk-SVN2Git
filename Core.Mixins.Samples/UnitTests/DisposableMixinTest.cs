using System;
using NUnit.Framework;
using Remotion.Mixins;

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
      DisposableMixinTest.C c = ObjectFactory.Create<C>().With();
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
      DisposableMixinTest.C c = ObjectFactory.Create<C> ().With ();
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