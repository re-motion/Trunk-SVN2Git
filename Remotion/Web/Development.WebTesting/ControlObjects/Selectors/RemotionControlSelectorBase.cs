using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Default implementation for <see cref="RemotionControlObject"/> selector implementations.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class RemotionControlSelectorBase<TControlObject>
      : IFirstControlSelector<TControlObject>,
          IPerHtmlIDControlSelector<TControlObject>,
          IPerIndexControlSelector<TControlObject>,
          IPerLocalIDControlSelector<TControlObject>,
          ISingleControlSelector<TControlObject>
      where TControlObject : RemotionControlObject
  {
    protected readonly string _rootTag;
    private readonly string _cssClass;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="rootTag">The root tag of the control object.</param>
    /// <param name="cssClass">A defining CSS class on the root tag of the control object.</param>
    protected RemotionControlSelectorBase ([NotNull] string rootTag, [NotNull] string cssClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("rootTag", rootTag);
      ArgumentUtility.CheckNotNullOrEmpty ("cssClass", cssClass);

      _rootTag = rootTag;
      _cssClass = cssClass;
    }

    public TControlObject SelectFirst (TestObjectContext context)
    {
      var scope = context.Scope.FindCss ("." + _cssClass);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectSingle (TestObjectContext context)
    {
      var scope = context.Scope.FindCss ("." + _cssClass, Options.Single);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerHtmlID (TestObjectContext context, string htmlID)
    {
      var scope = context.Scope.FindId (htmlID);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerIndex (TestObjectContext context, int index)
    {
      var scope = context.Scope.FindXPath (string.Format ("(.//{0}{1})[{2}]", _rootTag, XPathUtils.CreateContainsClassCheck (_cssClass), index));
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