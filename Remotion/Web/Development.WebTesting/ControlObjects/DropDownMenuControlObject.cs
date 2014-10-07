using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="DropDownMenu"/>.
  /// </summary>
  public class DropDownMenuControlObject : RemotionControlObject, IClickableItemsControlObject
  {
    public DropDownMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject ClickItem (string itemID, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindCss (string.Format ("li.DropDownMenuItem[{0}='{1}']", DiagnosticMetadataAttributes.ItemID, itemID));
      return ClickItem (scope, waitingStrategy);
    }

    public UnspecifiedPageObject ClickItem (int index, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindXPath (string.Format ("li[{0}]", index));
      return ClickItem (scope, waitingStrategy);
    }

    public UnspecifiedPageObject ClickItemByHtmlID (string htmlID, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindId (htmlID);
      return ClickItem (scope, waitingStrategy);
    }

    public UnspecifiedPageObject ClickItemByText (string text, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindCss (string.Format ("li.DropDownMenuItem[{0}='{1}']", DiagnosticMetadataAttributes.Text, text));
      return ClickItem (scope, waitingStrategy);
    }

    private ElementScope GetDropDownMenuScope ()
    {
      var dropDownMenuButtonScope = Scope.FindCss ("a.DropDownMenuButton");
      dropDownMenuButtonScope.Click();

      var dropDownMenuOptionsScope = Context.RootElement.FindCss ("ul.DropDownMenuOptions");
      return dropDownMenuOptionsScope;
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, IWaitingStrategy waitingStrategy = null)
    {
      var actualWaitingStrategy = GetActualWaitingStrategy (item, waitingStrategy);

      var anchorScope = item.FindCss ("a");
      anchorScope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }
}