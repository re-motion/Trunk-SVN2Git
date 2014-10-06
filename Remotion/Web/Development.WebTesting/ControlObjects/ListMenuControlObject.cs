using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="ListMenu"/>.
  /// </summary>
  public class ListMenuControlObject : RemotionControlObject, IClickableItemsControlObject
  {
    public ListMenuControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public UnspecifiedPageObject ClickItem (string itemID, IWaitingStrategy waitingStrategy = null)
    {
      var item = Scope.FindCss (string.Format ("span[{0}='{1}'][{2}='{3}']", "class", "listMenuItem", DiagnosticMetadataAttributes.ItemID, itemID));
      return ClickItem (item);
    }

    public UnspecifiedPageObject ClickItem (int index, IWaitingStrategy waitingStrategy = null)
    {
      var item = FindChild ((index - 1).ToString());
      return ClickItem (item, waitingStrategy);
    }

    public UnspecifiedPageObject ClickItemByHtmlID (string htmlID, IWaitingStrategy waitingStrategy = null)
    {
      var item = Scope.FindId (htmlID);
      return ClickItem (item, waitingStrategy);
    }
    
    public UnspecifiedPageObject ClickItemByLabel (string label, IWaitingStrategy waitingStrategy = null)
    {
      var item = Scope.FindXPath (string.Format ("tbody/tr/td/span[contains(a,'{0}')]", label));
      return ClickItem (item, waitingStrategy);
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