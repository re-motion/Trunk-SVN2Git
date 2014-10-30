using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a cell within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/> in grid mode.
  /// </summary>
  public class BocListAsGridCellControlObject : BocControlObject, IControlHost, ICommandHost
  {
    private readonly BocListCellFunctionality _impl;

    public BocListAsGridCellControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _impl = new BocListCellFunctionality (id, context);
    }

    /// <summary>
    /// Returns the text content of the cell.
    /// </summary>
    public string GetText ()
    {
      return _impl.GetText();
    }

    public CommandControlObject GetCommand ()
    {
      return _impl.GetCommand();
    }

    public UnspecifiedPageObject ExecuteCommand (ICompletionDetection completionDetection = null)
    {
      return _impl.ExecuteCommand();
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return _impl.GetControl (controlSelectionCommand);
    }
  }
}