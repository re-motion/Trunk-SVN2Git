using System;
using Coypu;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Default implementation for <see cref="ControlObject"/> selector implementations.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class ControlSelectorBase<TControlObject> : IPerHtmlIDControlSelector<TControlObject>, IPerLocalIDControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    public TControlObject SelectPerHtmlID (TestObjectContext context, string htmlID)
    {
      var scope = context.Scope.FindId (htmlID);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerLocalID (TestObjectContext context, string localID)
    {
      var scope = context.Scope.FindIdEndingWith ("_" + localID);
      return CreateControlObject (context, scope);
    }

    protected TControlObject CreateControlObject (TestObjectContext context, ElementScope scope)
    {
      var id = scope.Id;
      var newContext = context.CloneForScope (scope);

      return (TControlObject) Activator.CreateInstance (typeof (TControlObject), new object[] { id, newContext });
    }
  }
}