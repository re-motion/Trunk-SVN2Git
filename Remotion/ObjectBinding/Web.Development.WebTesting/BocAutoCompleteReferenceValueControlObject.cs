using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting
{
  /// <summary>
  /// Control object representing the <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueControlObject : RemotionControlObject
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

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public string Text
    {
      get { return FindChild ("TextValue").Text; }
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public UnspecifiedPageObject FillWith ([NotNull] string text, [CanBeNull] IWaitingStrategy waitStrategy = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, Then.TabAway, waitStrategy);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public UnspecifiedPageObject FillWith ([NotNull] string text, [NotNull] ThenAction then, [CanBeNull] IWaitingStrategy waitStrategy = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("then", then);

      var actualWaitStrategy = GetActualWaitingStrategy (waitStrategy);
      FindChild ("TextValue").FillWithAndWait (text, then, Context, actualWaitStrategy);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public CommandControlObject GetCommand ()
    {
      var commandScope = FindChild ("Command");
      var context = Context.CloneForScope (commandScope);
      return new CommandControlObject (commandScope.Id, context);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public UnspecifiedPageObject ExecuteCommand ([CanBeNull] IWaitingStrategy waitStrategy = null)
    {
      return GetCommand().Click (waitStrategy);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = FindChild ("Boc_OptionsMenu");
      var context = Context.CloneForScope (dropDownMenuScope);
      return new DropDownMenuControlObject (dropDownMenuScope.Id, context);
    }
  }
}