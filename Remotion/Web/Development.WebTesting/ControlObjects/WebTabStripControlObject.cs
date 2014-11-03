using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class WebTabStripControlObject : RemotionControlObject, IControlObjectWithTabs
  {
    public WebTabStripControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public IControlObjectWithTabs SwitchTo ()
    {
      return this;
    }

    public UnspecifiedPageObject SwitchTo (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SwitchTo().WithItemID (itemID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithItemID (string itemID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var itemScope = Scope.FindDMA ("span.tabStripTab", DiagnosticMetadataAttributes.ItemID, itemID);
      return SwitchTo (itemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithIndex (int index, ICompletionDetection completionDetection)
    {
      var xPathSelector = string.Format (
          "(.//span{0})[{1}]",
          XPathUtils.CreateHasOneOfClassesCheck ("tabStripTab", "tabStripTabSelected"),
          index);
      var itemScope = Scope.FindXPath (xPathSelector);
      return SwitchTo (itemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithHtmlID (string htmlID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var itemScope = Scope.FindId (htmlID);
      return SwitchTo (itemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithText (string text, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var itemScope = Scope.FindDMA ("span.tabStripTab", DiagnosticMetadataAttributes.Text, text);
      return SwitchTo (itemScope, completionDetection);
    }

    private UnspecifiedPageObject SwitchTo (ElementScope tabScope, ICompletionDetection completionDetection)
    {
      var commandScope = tabScope.FindLink();

      var commandContext = Context.CloneForControl (commandScope);
      var command = new CommandControlObject (commandContext);
      return command.Click (completionDetection);
    }
  }
}