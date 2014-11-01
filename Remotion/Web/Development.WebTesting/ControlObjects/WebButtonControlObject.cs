﻿using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:Remotion.Web.UI.Controls.WebButton"/>.
  /// </summary>
  [UsedImplicitly]
  public class WebButtonControlObject : RemotionControlObject, IClickableControlObject
  {
    public WebButtonControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetection = DetermineActualCompletionDetection (completionDetection);
      Scope.ClickAndWait (Context, actualCompletionDetection);
      return UnspecifiedPage();
    }
  }
}