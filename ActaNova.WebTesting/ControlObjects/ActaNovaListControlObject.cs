using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova BocList.
  /// </summary>
  public class ActaNovaListControlObject : BocListControlObject
  {
    public ActaNovaListControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the top scope of the list.
    /// Of course this method fails, if the actual list doesn't feature a top block.
    /// </summary>
    public ScopeControlObject GetTopBlock ()
    {
      var scope = Scope.FindCss ("div.bocListTopBlock");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    /// <summary>
    /// Returns the top right-aligned scope of the list.
    /// Of course this method fails, if the actual list doesn't feature a top right-aligned block.
    /// </summary>
    public ScopeControlObject GetTopRightAlignedBlock ()
    {
      var scope = Scope.FindXPath (string.Format ("../div{0}", XPathUtils.CreateHasClassCheck ("bocListTopRightAlignedSection")));
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    /// <summary>
    /// Uses the text filter feature to filter the list.
    /// Of course this method fails, if the actual list doesn't feature a text filter.
    /// </summary>
    public void Filter (string filter)
    {
      var textBox = GetTopBlock().GetControl (new PerLocalIDControlSelectionCommand<TextBoxControlObject> (new TextBoxSelector(), "TextFilterField"));
      textBox.FillWith (filter);
    }

    /// <summary>
    /// Clears the text filter of the list.
    /// Of course this method fails, if the actual list doesn't feature a text filter.
    /// </summary>
    public void ClearFilter ()
    {
      var clearButton =
          GetTopBlock().GetControl (new PerLocalIDControlSelectionCommand<AnchorControlObject> (new AnchorSelector(), "TextFilterClearButton"));
      clearButton.Click();
    }

    /// <summary>
    /// Opens the column configuration for the list.
    /// Of course this method fails, if the actual list doesn't allow column configuration.
    /// </summary>
    public UnspecifiedPageObject OpenColumnConfiguration ()
    {
      var openConfigurationButtonScope = Scope.FindChild ("OpenConfigurationButton");
      var openConfigurationButton = new ImageButtonControlObject (Context.CloneForControl (openConfigurationButtonScope));
      return openConfigurationButton.Click();
    }

    /// <summary>
    /// Sets the current column configuration as preferred view.
    /// Of course this method fails, if the actual list doesn't allow setting a preferred view.
    /// </summary>
    public void SetPreferredView ()
    {
      var setPreferredViewButtonScope = Scope.FindChild ("SetPreferredViewButton");
      var setPreferredViewButton = new ImageButtonControlObject (Context.CloneForControl (setPreferredViewButtonScope));
      setPreferredViewButton.Click();
    }

    protected override ElementScope GetAvailableViewsScope ()
    {
      return Scope.FindChild ("CurrentViewSelector");
    }
  }
}