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
      throw new NotImplementedException();
    }

    public UnspecifiedPageObject ClickItem (int index, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      throw new NotImplementedException();
    }

    public UnspecifiedPageObject ClickItemByHtmlID (string htmlID, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      throw new NotImplementedException();
    }

    public UnspecifiedPageObject ClickItemByLabel (string label, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      throw new NotImplementedException();
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
      var anchorScope = item.FindCss ("a");

      var actualWaitingStrategy = GetActualWaitingStrategy (anchorScope, waitingStrategy);
      anchorScope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }
}