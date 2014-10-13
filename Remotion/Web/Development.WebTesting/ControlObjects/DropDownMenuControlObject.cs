using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Base class for all control objects representing a <see cref="DropDownMenu"/>.
  /// </summary>
  public abstract class DropDownMenuControlObjectBase : RemotionControlObject, IClickableItemsControlObject
  {
    protected DropDownMenuControlObjectBase (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    protected abstract void OpenDropDownMenu ();

    public UnspecifiedPageObject ClickItem (string itemID, IWaitingStrategy waitingStrategy = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindDMA ("li.DropDownMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
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
      var scope = dropDownMenuScope.FindDMA ("li.DropDownMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (scope, waitingStrategy);
    }

    private ElementScope GetDropDownMenuScope ()
    {
      OpenDropDownMenu();

      var dropDownMenuOptionsScope = Context.RootElement.FindCss ("ul.DropDownMenuOptions");
      return dropDownMenuOptionsScope;
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, IWaitingStrategy waitingStrategy = null)
    {
      var actualWaitingStrategy = GetActualWaitingStrategy (item, waitingStrategy);

      var anchorScope = item.FindLink();
      anchorScope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }

  /// <summary>
  /// Control object for the default <see cref="DropDownMenu"/>.
  /// </summary>
  public class DropDownMenuControlObject : DropDownMenuControlObjectBase
  {
    public DropDownMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    protected override void OpenDropDownMenu ()
    {
      var dropDownMenuButtonScope = Scope.FindCss ("a.DropDownMenuButton");
      dropDownMenuButtonScope.Click();
    }
  }

  /// <summary>
  /// Control object for a context menu based on <see cref="DropDownMenu"/>.
  /// </summary>
  public class ContextMenuControlObject : DropDownMenuControlObjectBase
  {
    public ContextMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    protected override void OpenDropDownMenu ()
    {
      Scope.ContextClick (Context);
    }
  }
}