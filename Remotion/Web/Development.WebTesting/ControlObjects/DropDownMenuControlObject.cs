using System;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public class DropDownMenuControlObject : RemotionControlObject
  {
    public DropDownMenuControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns the current label of the DropDownMenu.
    /// </summary>
    public string Label
    {
      get { return Scope.FindCss ("a.DropDownMenuLabel").Text; }
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    /// <param name="label"></param>
    /// <param name="waitStrategy"></param>
    /// <returns></returns>
    public UnspecifiedPageObject SelectMenuItem (string label, IWaitingStrategy waitStrategy)
    {
      var dropDownMenuButton = Scope.FindCss ("a.DropDownMenuButton");
      dropDownMenuButton.Click();

      var dropDownMenuOptions = Context.RootElement.FindCss ("ul.DropDownMenuOptions");
      var dropDownMenuItem = dropDownMenuOptions.FindXPath (string.Format ("./li/a[contains(span, '{0}')]", label));

      // Todo RM-6297: Implementation
      //dropDownMenuItem.ClickUsingWaitStrategy (Context, waitStrategy);

      return UnspecifiedPage();
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    /// <param name="index"></param>
    /// <param name="waitStrategy"></param>
    /// <returns></returns>
    public UnspecifiedPageObject SelectMenuItem (int index, IWaitingStrategy waitStrategy)
    {
      var dropDownMenuButton = Scope.FindCss ("a.DropDownMenuButton");
      dropDownMenuButton.Click();

      var dropDownMenuOptions = Context.RootElement.FindCss ("ul.DropDownMenuOptions");
      var dropDownMenuItem = dropDownMenuOptions.FindXPath (string.Format ("./li/a[{0}]", index));

      // Todo RM-6297: Implementation
      //dropDownMenuItem.ClickUsingWaitStrategy (Context, waitStrategy);

      return UnspecifiedPage();
    }
  }
}