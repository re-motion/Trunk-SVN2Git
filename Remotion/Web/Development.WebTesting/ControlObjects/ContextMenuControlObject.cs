using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for a context menu based on <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
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