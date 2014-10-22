﻿using System;
using JetBrains.Annotations;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox"/> control.
  /// </summary>
  [UsedImplicitly]
  public class BocCheckBoxControlObject : BocControlObject
  {
    public BocCheckBoxControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns the current state of the check box.
    /// </summary>
    public bool State
    {
      get
      {
        if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
          return ParseState (FindChild ("Value")["data-value"]);

        return FindChild ("Value")["checked"] != null;
      }
    }

    /// <summary>
    /// Sets the state of the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox"/> to <paramref name="newState"/>.
    /// </summary>
    public UnspecifiedPageObject SetTo (bool newState, IActionBehavior actionBehavior = null)
    {
      if (State == newState)
        return UnspecifiedPage();

      var actualActionBehavior = GetActualActionBehavior (actionBehavior);
      FindChild ("Value").PerformAction (
          s =>
          {
            if (newState)
              s.Check();
            else
              s.Uncheck();
          },
          Context,
          actualActionBehavior);

      return UnspecifiedPage();
    }

    private bool ParseState (string state)
    {
      if (state == "False")
        return false;

      if (state == "True")
        return true;

      throw new ArgumentException ("must be either 'True' or 'False'", "state");
    }
  }
}