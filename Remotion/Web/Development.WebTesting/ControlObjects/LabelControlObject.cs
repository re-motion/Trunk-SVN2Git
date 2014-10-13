using System;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="Label"/> and all its derivatives (e.g. <see cref="SmartLabel"/>).
  /// </summary>
  public class LabelControlObject : RemotionControlObject
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