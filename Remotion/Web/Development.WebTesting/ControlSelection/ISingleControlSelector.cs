using System;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Interface for <see cref="IControlSelector"/> implementations which provide the possibility to select the only existing control matching their
  /// supported type of <typeparamref name="TControlObject"/>.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface ISingleControlSelector<out TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Selects the only control within the given <paramref name="context"/> matching the type <typeparamref name="TControlObject"/>.
    /// </summary>
    /// <returns>The control object.</returns>
    /// <exception cref="AmbiguousException">If multiple controls of type <typeparamref name="TControlObject"/> are found.</exception>
    /// <exception cref="MissingHtmlException">If no matching control can be found.</exception>
    TControlObject SelectSingle ([NotNull] TestObjectContext context);
  }
}