using System;
using System.Web.UI.WebControls;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="Label"/> and all its derivatives (e.g. <see cref="T:Remotion.Web.UI.Controls.SmartLabel"/>).
  /// </summary>
  [UsedImplicitly]
  public class LabelControlObject : ControlObject
  {
    public LabelControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      return Scope.Text;
    }
  }
}