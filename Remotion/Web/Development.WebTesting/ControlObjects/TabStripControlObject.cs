using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="WebTabStrip"/>.
  /// </summary>
  public class TabStripControlObject : RemotionControlObject
  {
    private readonly string _itemSuffix;

    public TabStripControlObject ([NotNull] string id, [NotNull]string itemSuffix, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      ArgumentUtility.CheckNotNull ("itemSuffix", itemSuffix);

      _itemSuffix = itemSuffix;
    }

    public UnspecifiedPageObject SwitchTo (string localID)
    {
      var commandScope = FindChild (string.Format ("{0}{1}_Command", localID, _itemSuffix));
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
      return command.Click (WaitingStrategies.WxePostBack);
    }
  }
}