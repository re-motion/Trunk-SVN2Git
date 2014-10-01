using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector base class for all business object controls.
  /// </summary>
  public abstract class BocSelectorBase<TControlObject>
      : RemotionControlSelectorBase<TControlObject>,
          IPerDisplayNameControlSelector<TControlObject>,
          IPerDomainPropertyControlSelector<TControlObject>
      where TControlObject : BocControlObject
  {
    protected BocSelectorBase ([NotNull] string rootTag, [NotNull] string cssClass)
        : base (rootTag, cssClass)
    {
    }

    public TControlObject SelectPerDisplayName (TestObjectContext context, string displayName)
    {
      var scope = context.Scope.FindCss (string.Format ("{0}[{1}='{2}']", _rootTag, DiagnosticMetadataAttributes.DisplayName, displayName));
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerDomainProperty (TestObjectContext context, string domainProperty, string domainClass)
    {
      ElementScope scope;

      if (domainClass != null)
      {
        scope = context.Scope.FindCss (
            string.Format (
                "{0}[{1}='{2}'][{3}='{4}']",
                _rootTag,
                DiagnosticMetadataAttributes.BoundType,
                domainClass,
                DiagnosticMetadataAttributes.BoundProperty,
                domainProperty));
      }
      else
        scope = context.Scope.FindCss (string.Format ("{0}[{1}='{2}']", _rootTag, DiagnosticMetadataAttributes.BoundProperty, domainProperty));

      return CreateControlObject (context, scope);
    }
  }
}