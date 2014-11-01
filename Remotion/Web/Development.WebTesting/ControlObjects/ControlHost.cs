using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Implements the <see cref="IControlHost"/> interface for an arbitrary <see cref="ControlObject"/>.
  /// </summary>
  internal class ControlHost : IControlHost
  {
    private readonly ControlObjectContext _context;

    public ControlHost ([NotNull] ControlObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      return controlSelectionCommand.Select (_context.CloneForControlSelection());
    }
  }
}