using System;
using System.Web.UI.WebControls;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="Label"/> and all its derivatives (e.g. <see cref="T:Remotion.Web.UI.Controls.SmartLabel"/>).
  /// </summary>
  [UsedImplicitly]
  public class LabelControlObject : WebFormsControlObject, IControlObjectWithText
  {
    public LabelControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      return Scope.Text.Trim();
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      throw new NotSupportedException ("The LabelControlObject does not support any interaction.");
    }
  }
}