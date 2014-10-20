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
    public TabbedMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string StatusText
    {
      get { return Scope.FindCss ("td.tabbedMenuStatusCell").Text.Trim(); }
    }

    public UnspecifiedPageObject SelectMenuItem ([NotNull] string itemID, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetMainMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    public UnspecifiedPageObject SelectMenuItem (int index, IActionBehavior actionBehavior = null)
    {
      var menuItemScope = GetMainMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    public UnspecifiedPageObject SelectMenuItemByHtmlID ([NotNull] string htmlID, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    public UnspecifiedPageObject SelectMenuItemByText ([NotNull] string text, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var menuItemScope = GetMainMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.Text, text);
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    public UnspecifiedPageObject SelectSubMenuItem ([NotNull] string itemID, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetSubMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    public UnspecifiedPageObject SelectSubMenuItem (int index, IActionBehavior actionBehavior = null)
    {
      var menuItemScope = GetSubMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    public UnspecifiedPageObject SelectSubMenuItemByHtmlID ([NotNull] string htmlID, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    public UnspecifiedPageObject SelectSubMenuItemByText ([NotNull] string text, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var menuItemScope = GetSubMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.Text, text);
      return SelectMenuItem (menuItemScope, actionBehavior);
    }

    private UnspecifiedPageObject SelectMenuItem (ElementScope menuItemScope, IActionBehavior actionBehavior)
    {
      var commandScope = menuItemScope.FindLink();

      var commandContext = Context.CloneForScope (commandScope);
      var command = new CommandControlObject (commandScope.Id, commandContext);
      return command.Click (actionBehavior);
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