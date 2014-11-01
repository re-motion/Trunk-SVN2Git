﻿using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an ActaNova message box.
  /// </summary>
  public class ActaNovaMessageBoxControlObject : ActaNovaControlObject
  {
    public ActaNovaMessageBoxControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Confirms the ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject Okay ()
    {
      return ClickButton ("OK");
    }

    /// <summary>
    /// Cancels the ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject Cancel ()
    {
      return ClickButton ("Cancel");
    }

    private UnspecifiedPageObject ClickButton (string buttonId)
    {
      var id = string.Format ("DisplayBoxPopUp_MessageBoxControl_Popup{0}Button", buttonId);
      var buttonScope = Scope.FindId (id);

      RetryUntilTimeout.Run (() => buttonScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted)));
      
      return UnspecifiedPage();
    }
  }
}