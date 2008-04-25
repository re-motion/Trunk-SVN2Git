using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityAdapter _mockWebSecurityAdapter;
    private ISecurableObject _mockSecurableObject;
    private NavigationCommand _mockNavigationCommand;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityAdapter = _mocks.CreateMock<IWebSecurityAdapter> ();
      _mockSecurableObject = _mocks.CreateMock<ISecurableObject> ();
      _mockNavigationCommand = _mocks.CreateMock<NavigationCommand> ();

      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _mockWebSecurityAdapter);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = true;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.IsDisabled = false;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.IsDisabled = true;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (true);
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isEnabled);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (false);
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = true;
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isEnabled);
    }

    private MainMenuTab CreateMainMenuTab ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.Command = _mockNavigationCommand;
      
      return mainMenuTab;
    }

    private MainMenuTab CreateMainMenuTabWithoutCommand ()
    {
      MainMenuTab mainMenuTab = new MainMenuTab ();
      mainMenuTab.Command.Type = CommandType.None;
      mainMenuTab.Command = null;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;

      return mainMenuTab;
    }
  }
}
