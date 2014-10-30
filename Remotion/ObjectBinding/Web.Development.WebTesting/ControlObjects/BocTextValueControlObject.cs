﻿using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocTextValue"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocTextValueControlObject : BocControlObject, IFillableControlObject
  {
    /// <summary>
    /// Initializes the control object.
    /// </summary>
    /// <param name="id">The control object's ID.</param>
    /// <param name="context">The control object's context.</param>
    public BocTextValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      var valueScope = FindChild ("Value");

      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return valueScope.Text; // do not trim

      return valueScope.Value; // do not trim
    }

    public UnspecifiedPageObject FillWith (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, Then.TabAway, completionDetection);
    }

    public UnspecifiedPageObject FillWith (string text, ThenAction then, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("then", then);

      var actualCompletionDetection = DetermineActualCompletionDetection (then, completionDetection);
      FindChild ("Value").FillWithAndWait (text, then, Context, actualCompletionDetection);
      return UnspecifiedPage();
    }

    private ICompletionDetection DetermineActualCompletionDetection (ThenAction then, ICompletionDetection userDefinedCompletionDetection)
    {
      return then == Then.DoNothing && userDefinedCompletionDetection == null
          ? Behavior.WaitFor (WaitFor.Nothing)
          : DetermineActualCompletionDetection (userDefinedCompletionDetection);
    }
  }
}