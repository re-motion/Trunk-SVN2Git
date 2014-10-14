using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Default implementation, suitable for most <see cref="HtmlControlObject"/> selector implementations.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class HtmlControlSelectorBase<TControlObject>
      : ControlSelectorBase<TControlObject>,
          IFirstControlSelector<TControlObject>,
          IPerIndexControlSelector<TControlObject>,
          ISingleControlSelector<TControlObject>
      where TControlObject : HtmlControlObject
  {
    private readonly string _rootTag;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="rootTag">The root tag of the control object.</param>
    protected HtmlControlSelectorBase ([NotNull] string rootTag)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("rootTag", rootTag);

      _rootTag = rootTag;
    }

    protected string RootTag
    {
      get { return _rootTag; }
    }

    public TControlObject SelectFirst (TestObjectContext context)
    {
      var scope = context.Scope.FindCss (_rootTag);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectSingle (TestObjectContext context)
    {
      var scope = context.Scope.FindCss (_rootTag, Options.Single);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerIndex (TestObjectContext context, int index)
    {
      var scope = context.Scope.FindXPath (string.Format ("(.//{0})[{1}]", _rootTag, index));
      return CreateControlObject (context, scope);
    }
  }
}