using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Base implementation for <see cref="ControlObject"/> selector implementations.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class ControlSelectorBase<TControlObject> : IPerHtmlIDControlSelector<TControlObject>, IPerLocalIDControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    public TControlObject SelectPerHtmlID (ControlSelectionContext context, string htmlID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("htmlID", htmlID);

      var scope = context.Scope.FindId (htmlID);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerLocalID (ControlSelectionContext context, string localID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("localID", localID);

      var scope = context.Scope.FindIdEndingWith ("_" + localID);
      if (!scope.Exists())
        scope = context.Scope.FindId (localID);

      return CreateControlObject (context, scope);
    }

    protected TControlObject CreateControlObject ([NotNull] ControlSelectionContext context, [NotNull] ElementScope scope)
    {
      var newControlObjectContext = context.CloneForControl (context.PageObject, scope);
      return CreateControlObject (newControlObjectContext, context);
    }

    protected abstract TControlObject CreateControlObject (
        [NotNull] ControlObjectContext newControlObjectContext,
        [NotNull] ControlSelectionContext controlSelectionContext);
  }
}