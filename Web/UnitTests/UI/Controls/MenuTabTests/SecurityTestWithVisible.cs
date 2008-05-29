using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class SecurityTestWithVisible : BaseTest
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

      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _mockWebSecurityAdapter);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      mainMenuTab.IsVisible = true;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isVisible = mainMenuTab.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      mainMenuTab.IsVisible = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isVisible = mainMenuTab.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.IsVisible = true;

      bool isVisible = mainMenuTab.EvaluateVisible ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand ();
      mainMenuTab.IsVisible = false;

      bool isVisible = mainMenuTab.EvaluateVisible ();
      Assert.IsFalse (isVisible);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsVisible = true;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (true);
      _mocks.ReplayAll ();

      bool isVisible = mainMenuTab.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsTrue (isVisible);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsVisible = true;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (false);
      _mocks.ReplayAll ();

      bool isVisible = mainMenuTab.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsVisible = false;
      _mocks.ReplayAll ();

      bool isVisible = mainMenuTab.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.IsFalse (isVisible);
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
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      
      return mainMenuTab;
    }
  }
}
