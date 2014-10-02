using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListControlObject : BocControlObject
  {
    public BocListControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }
  }
}