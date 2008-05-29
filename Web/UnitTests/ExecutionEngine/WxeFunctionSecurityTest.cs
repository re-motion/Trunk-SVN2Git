using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

  [TestFixture]
  public class WxeFunctionSecurityTest : WxeTest
  {
    private MockRepository _mocks;
    private IWxeSecurityAdapter _mockWxeSecurityAdapter;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _mockWxeSecurityAdapter = _mocks.CreateMock<IWxeSecurityAdapter> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _mockWxeSecurityAdapter);
    }

    [Test]
    public void ExecuteFunctionWithAccessGranted ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityAdapter.CheckAccess (function);
      _mocks.ReplayAll ();

      function.Execute ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void ExecuteFunctionWithAccessDenied ()
    {
      TestFunction function = new TestFunction ();
      _mockWxeSecurityAdapter.CheckAccess (function);
      LastCall.Throw (new PermissionDeniedException ("Test Exception"));
      _mocks.ReplayAll ();

      try
      {
        function.Execute ();
      }
      catch (WxeUnhandledException e)
      {
        _mocks.VerifyAll ();

        Assert.IsInstanceOfType (typeof (PermissionDeniedException), e.InnerException);
        return;
      }
      Assert.Fail ("Expected PermissionDeniedException.");
    }

    [Test]
    public void ExecuteFunctionWithoutWxeSecurityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);

      TestFunction function = new TestFunction ();
      _mocks.ReplayAll ();

      function.Execute ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void HasStatelessAccessGranted ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (true);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasStatelessAccessDenied ()
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (typeof (TestFunction))).Return (false);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasStatelessAccessGrantedWithoutWxeSecurityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
      _mocks.ReplayAll ();

      bool hasAccess = WxeFunction.HasAccess (typeof (TestFunction));

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}
