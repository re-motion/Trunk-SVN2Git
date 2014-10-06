using System;
using Coypu;
using JetBrains.Annotations;
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

    public UnspecifiedPageObject SwitchTo (string localID)
    {
      var commandScope = FindChild (string.Format ("{0}_Command", localID));
      return SwitchToUsingCommandScope (commandScope);
    }

    public UnspecifiedPageObject SwitchToByLabel (string label)
    {
      var commandScope = Scope.FindXPath (string.Format ("div/ul/li/span/span/a[contains(span,'{0}')]", label));
      return SwitchToUsingCommandScope (commandScope);
    }

    private UnspecifiedPageObject SwitchToUsingCommandScope(ElementScope commandScope)
    {
      var commandContext = Context.CloneForScope (commandScope);
      var command = new CommandControlObject (commandScope.Id, commandContext);
      return command.Click (WaitFor.WxePostBack);
    }
  }
}