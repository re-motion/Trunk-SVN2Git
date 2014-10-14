using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Default implementation, suitable for most <see cref="RemotionControlObject"/> selector implementations.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class RemotionControlSelectorBase<TControlObject>
      : ControlSelectorBase<TControlObject>,
          IFirstControlSelector<TControlObject>,
          IPerIndexControlSelector<TControlObject>,
          ISingleControlSelector<TControlObject>
      where TControlObject : RemotionControlObject
  {
    private readonly string _rootTag;
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

    protected string RootTag
    {
      get { return _rootTag; }
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

    public TControlObject SelectPerIndex (TestObjectContext context, int index)
    {
      var scope = context.Scope.FindXPath (string.Format ("(.//{0}{1})[{2}]", _rootTag, XPathUtils.CreateContainsClassCheck (_cssClass), index));
      return CreateControlObject (context, scope);
    }
  }
}