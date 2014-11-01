using System;
using Coypu;
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
    public TControlObject SelectPerHtmlID (WebTestObjectContext context, string htmlID)
    {
      var scope = context.Scope.FindId (htmlID);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerLocalID (WebTestObjectContext context, string localID)
    {
      var scope = context.Scope.FindIdEndingWith ("_" + localID);
      if (!scope.Exists())
        scope = context.Scope.FindId (localID);

      return CreateControlObject (context, scope);
    }

    protected TControlObject CreateControlObject (WebTestObjectContext context, ElementScope scope)
    {
      var newContext = context.CloneForControl (scope);
      return (TControlObject) Activator.CreateInstance (typeof (TControlObject), new object[] { newContext });
    }
  }
}