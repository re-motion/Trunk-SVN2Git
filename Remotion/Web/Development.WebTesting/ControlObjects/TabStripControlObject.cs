using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class TabStripControlObject : RemotionControlObject, ITabStripControlObject
  {
    public TabStripControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject SwitchTo (string itemID)
    {
      var itemScope = Scope.FindDMA ("span.tabStripTab", DiagnosticMetadataAttributes.ItemID, itemID);
      return SwitchTo (itemScope);
    }

    public UnspecifiedPageObject SwitchTo (int index)
    {
      var xPathSelector = string.Format (
          "(.//span{0})[{1}]",
          XPathUtils.CreateContainsOneOfClassesCheck ("tabStripTab", "tabStripTabSelected"),
          index);
      var itemScope = Scope.FindXPath (xPathSelector);
      return SwitchTo (itemScope);
    }

    public UnspecifiedPageObject SwitchToByHtmlID (string htmlID)
    {
      var itemScope = Scope.FindId (htmlID);
      return SwitchTo (itemScope);
    }

    public UnspecifiedPageObject SwitchToByText (string text)
    {
      var itemScope = Scope.FindXPath (string.Format ("div/ul/li/span/span[contains(a/span,'{0}')]", text));
      return SwitchTo (itemScope);
    }

    private UnspecifiedPageObject SwitchTo (ElementScope tabScope)
    {
      var commandScope = tabScope.FindLink();

      var commandContext = Context.CloneForScope (commandScope);
      var command = new CommandControlObject (commandScope.Id, commandContext);
      return command.Click (Behavior.WaitFor (WaitFor.WxePostBack));
    }
  }
}