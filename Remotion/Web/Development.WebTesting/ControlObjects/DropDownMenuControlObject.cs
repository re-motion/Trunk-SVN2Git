using System;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium.Interactions;
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

    public UnspecifiedPageObject ClickItem (string itemID, IActionBehavior actionBehavior = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindDMA ("li.DropDownMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (scope, actionBehavior);
    }

    public UnspecifiedPageObject ClickItem (int index, IActionBehavior actionBehavior = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindXPath (string.Format ("li[{0}]", index));
      return ClickItem (scope, actionBehavior);
    }

    public UnspecifiedPageObject ClickItemByHtmlID (string htmlID, IActionBehavior actionBehavior = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindId (htmlID);
      return ClickItem (scope, actionBehavior);
    }

    public UnspecifiedPageObject ClickItemByText (string text, IActionBehavior actionBehavior = null)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindDMA ("li.DropDownMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (scope, actionBehavior);
    }

    private ElementScope GetDropDownMenuScope ()
    {
      OpenDropDownMenu();

      var dropDownMenuOptionsScope = Context.RootElement.FindCss ("ul.DropDownMenuOptions");
      return dropDownMenuOptionsScope;
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, IActionBehavior actionBehavior = null)
    {
      var actualWaitingStrategy = GetActualActionBehavior (item, actionBehavior);

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