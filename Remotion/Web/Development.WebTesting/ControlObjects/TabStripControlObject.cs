using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="WebTabStrip"/>.
  /// </summary>
  public class TabStripControlObject : RemotionControlObject, ITabStripControlObject
  {
    public TabStripControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject SwitchTo (string itemID)
    {
      var itemScope = Scope.FindCss (string.Format ("span.tabStripTab[{0}='{1}']", DiagnosticMetadataAttributes.ItemID, itemID));
      return SwitchToUsingCommandScope (itemScope);
    }

    public UnspecifiedPageObject SwitchTo (int index)
    {
      var xPathSelector = string.Format ("(.//span{0})[{1}]", XPathUtils.CreateContainsOneOfClassesCheck ("tabStripTab", "tabStripTabSelected"), index);
      var itemScope = Scope.FindXPath (xPathSelector);
      return SwitchToUsingCommandScope (itemScope);
    }

    public UnspecifiedPageObject SwitchToByHtmlID (string htmlID)
    {
      var itemScope = Scope.FindId (htmlID);
      return SwitchToUsingCommandScope (itemScope);
    }

    public UnspecifiedPageObject SwitchToByText (string text)
    {
      var itemScope = Scope.FindXPath (string.Format ("div/ul/li/span/span[contains(a/span,'{0}')]", text));
      return SwitchToUsingCommandScope (itemScope);
    }

    private UnspecifiedPageObject SwitchToUsingCommandScope(ElementScope tabScope)
    {
      var commandScope = tabScope.FindCss ("a");

      var commandContext = Context.CloneForScope (commandScope);
      var command = new CommandControlObject (commandScope.Id, commandContext);
      return command.Click (WaitFor.WxePostBack);
    }
  }
}