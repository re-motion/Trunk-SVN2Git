using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.TabbedMultiView"/>.
  /// </summary>
  [UsedImplicitly]
  public class TabbedMultiViewControlObject : RemotionControlObject, IControlHost, IControlObjectWithTabs
  {
    public TabbedMultiViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public ScopeControlObject GetTopControls ()
    {
      var scope = Scope.FindChild ("TopControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public ScopeControlObject GetActiveView ()
    {
      var scope = Scope.FindChild ("ActiveView");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public ScopeControlObject GetBottomControls ()
    {
      var scope = Scope.FindChild ("BottomControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public IControlObjectWithTabs SwitchTo ()
    {
      return this;
    }

    public UnspecifiedPageObject SwitchTo (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SwitchTo().WithItemID (itemID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithItemID (string itemID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetTabStrip().SwitchTo (itemID + "_Tab", completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithIndex (int index, ICompletionDetection completionDetection)
    {
      return GetTabStrip().SwitchTo().WithIndex (index, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithHtmlID (string htmlID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      return GetTabStrip().SwitchTo().WithHtmlID (htmlID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithText (string text, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return GetTabStrip().SwitchTo().WithText (text, completionDetection);
    }

    private WebTabStripControlObject GetTabStrip ()
    {
      var scope = Scope.FindChild ("TabStrip");
      return new WebTabStripControlObject (Context.CloneForControl (scope));
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}