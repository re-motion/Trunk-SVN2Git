using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="WebTreeViewControlObject"/>.
  /// </summary>
  public class WebTreeViewSelector : TypedControlSelectorBase<WebTreeViewControlObject>
  {
    public WebTreeViewSelector ()
        : base ("WebTreeView")
    {
    }

    protected override WebTreeViewControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new WebTreeViewControlObject (newControlObjectContext);
    }
  }
}