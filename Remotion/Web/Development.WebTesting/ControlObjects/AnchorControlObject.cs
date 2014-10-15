﻿using System;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="LinkButton"/>, <see cref="HyperLink"/> and all their derivatives (e.g.
  /// <see cref="T:Remotion.Web.UI.Controls.WebLinkButton"/> or <see cref="T:Remotion.Web.UI.Controls.SmartHyperLink"/>). Also represents a simple
  /// HTML anchor &lt;a&gt; control within a re-motion applicaiton.
  /// </summary>
  [UsedImplicitly]
  public class AnchorControlObject : ControlObject, IClickableControlObject
  {
    public AnchorControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject Click (IActionBehavior actionBehavior = null)
    {
      var actualActionBehavior = GetActualActionBehavior (actionBehavior);
      Scope.ClickAndWait (Context, actualActionBehavior);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Returns the actual <see cref="IActionBehavior"/> to be used.
    /// </summary>
    /// <param name="userDefinedActionBehavior">User-provided <see cref="IActionBehavior"/>.</param>
    /// <returns><see cref="IActionBehavior"/> to be used.</returns>
    private IActionBehavior GetActualActionBehavior ([CanBeNull] IActionBehavior userDefinedActionBehavior)
    {
      if (userDefinedActionBehavior != null)
        return userDefinedActionBehavior;

      if (IsPostBackLink())
        return Behavior.WaitFor (WaitFor.WxePostBack);

      return Behavior.WaitFor (WaitFor.WxeReset);
    }

    private bool IsPostBackLink ()
    {
      const string remotionDoPostBackScript = "DoPostBackWithOptions";
      const string aspNetDoPostBackScript = "__doPostBack";

      return Scope["href"].Contains (remotionDoPostBackScript) ||
             Scope["href"].Contains (aspNetDoPostBackScript) ||
             (Scope["href"].Equals ("#") && Scope["onclick"] != null && Scope["onclick"].Contains (aspNetDoPostBackScript));
    }
  }
}