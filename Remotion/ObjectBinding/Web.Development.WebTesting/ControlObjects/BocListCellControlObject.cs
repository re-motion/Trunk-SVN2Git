using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a cell within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListCellControlObject : BocControlObject, ICommandHost
  {
    public BocListCellControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns the text content of the cell.
    /// </summary>
    public string GetText ()
    {
      return Scope.Text.Trim();
    }

    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindLink();
      var context = Context.CloneForScope (commandScope);
      return new CommandControlObject (commandScope.Id, context);
    }

    public UnspecifiedPageObject ExecuteCommand (IActionBehavior actionBehavior = null)
    {
      return GetCommand().Click (actionBehavior);
    }
  }
}