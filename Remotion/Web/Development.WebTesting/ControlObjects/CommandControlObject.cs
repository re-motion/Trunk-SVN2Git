﻿using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a <see cref="T:Remotion.Web.UI.Controls.Command"/>.
  /// </summary>
  public class CommandControlObject : RemotionControlObject, IClickableControlObject
  {
    public CommandControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      Scope.ClickAndWait (Context, actualCompletionDetector);
      return UnspecifiedPage();
    }
  }
}