﻿using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueControlObject : BocControlObject, IFillableControlObject
  {
    /// <summary>
    /// Initializes the control object.
    /// </summary>
    /// <param name="id">The control object's ID.</param>
    /// <param name="context">The control object's context.</param>
    public BocAutoCompleteReferenceValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return FindChild ("Label").Text;

      return FindChild ("TextValue").Value;
    }

    public UnspecifiedPageObject FillWith ([NotNull] string text, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, Then.TabAway, actionBehavior);
    }

    public UnspecifiedPageObject FillWith ([NotNull] string text, [NotNull] ThenAction then, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("then", then);

      var actualActionBehavior = GetActualActionBehavior (actionBehavior);
      FindChild ("TextValue").FillWithAndWait (text, then, Context, actualActionBehavior);
      return UnspecifiedPage();
    }

    public CommandControlObject GetCommand ()
    {
      var commandScope = FindChild ("Command");
      var context = Context.CloneForScope (commandScope);
      return new CommandControlObject (commandScope.Id, context);
    }

    public UnspecifiedPageObject ExecuteCommand ([CanBeNull] IActionBehavior actionBehavior = null)
    {
      return GetCommand().Click (actionBehavior);
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = FindChild ("Boc_OptionsMenu");
      var context = Context.CloneForScope (dropDownMenuScope);
      return new DropDownMenuControlObject (dropDownMenuScope.Id, context);
    }
  }
}