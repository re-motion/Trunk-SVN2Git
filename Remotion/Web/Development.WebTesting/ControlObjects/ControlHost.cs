using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Implements the <see cref="IControlHost"/> interface for an arbitrary <see cref="WebTestObjectContext"/> scope.
  /// </summary>
  internal class ControlHost : IControlHost
  {
    private readonly WebTestObjectContext _context;

    public ControlHost ([NotNull] WebTestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      return controlSelectionCommand.Select (_context);
    }
  }
}