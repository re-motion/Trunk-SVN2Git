using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.TabbedMenu"/>.
  /// </summary>
  [UsedImplicitly]
  public class TabbedMenuControlObject : RemotionControlObject
  {
    public TabbedMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetStatusText ()
    {
      return Scope.FindCss ("td.tabbedMenuStatusCell").Text.Trim();
    }

    public UnspecifiedPageObject SelectMenuItem ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetMainMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public UnspecifiedPageObject SelectMenuItem (int index, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      var menuItemScope = GetMainMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public UnspecifiedPageObject SelectMenuItemByHtmlID ([NotNull] string htmlID, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public UnspecifiedPageObject SelectMenuItemByText ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var menuItemScope = GetMainMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.Text, text);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public UnspecifiedPageObject SelectSubMenuItem ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetSubMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public UnspecifiedPageObject SelectSubMenuItem (int index, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      var menuItemScope = GetSubMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public UnspecifiedPageObject SelectSubMenuItemByHtmlID ([NotNull] string htmlID, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public UnspecifiedPageObject SelectSubMenuItemByText ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var menuItemScope = GetSubMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.Text, text);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    private UnspecifiedPageObject SelectMenuOrSubMenuItem (ElementScope menuItemScope, ICompletionDetection completionDetection)
    {
      var commandScope = menuItemScope.FindLink();

      var commandContext = Context.CloneForControl (commandScope);
      var command = new CommandControlObject (commandContext);
      return command.Click (completionDetection);
    }

    private ElementScope GetMainMenuScope ()
    {
      return Scope.FindCss ("td.tabbedMainMenuCell");
    }

    private ElementScope GetSubMenuScope ()
    {
      return Scope.FindCss ("td.tabbedSubMenuCell");
    }
  }
}