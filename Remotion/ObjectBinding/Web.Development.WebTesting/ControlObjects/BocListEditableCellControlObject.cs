using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a cell in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableCellControlObject : BocControlObject, IControlHost
  {
    private readonly BocListCellFunctionality _impl;

    public BocListEditableCellControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _impl = new BocListCellFunctionality (context);
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return _impl.GetControl (controlSelectionCommand);
    }
  }
}