using System;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Context for a <see cref="IControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  public class ControlSelectionContext : ControlObjectContext
  {
    /// <summary>
    /// Private constructor, may be obtained only via a <see cref="PageObjectContext"/> or <see cref="ControlObjectContext"/>.
    /// </summary>
    internal ControlSelectionContext ([NotNull] PageObject pageObject, [NotNull] ElementScope scope)
        : base (pageObject, scope)
    {
    }
  }
}