using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="ActaNovaTreePopupTableControlObject"/>.
  /// </summary>
  public class ActaNovaTreePopupTableNodeControlObject : WebFormsControlObject, IClickableControlObject, IControlObjectWithText
  {
    public ActaNovaTreePopupTableNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      return Scope.Text.Trim();
    }

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var anchorControl = Children.GetControl (new SingleControlSelectionCommand<AnchorControlObject> (new AnchorSelector()));
      return anchorControl.Click (completionDetection);
    }
  }
}