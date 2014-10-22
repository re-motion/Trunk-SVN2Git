using System;
using System.Collections.Generic;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;

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
      var scope = context.Scope.FindDMA (RootTag, DiagnosticMetadataAttributesForObjectBinding.DisplayName, displayName);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerDomainProperty (TestObjectContext context, string domainProperty, string domainClass)
    {
      ElementScope scope;

      if (domainClass != null)
      {
        scope = context.Scope.FindDMA (
            new[] { RootTag },
            new Dictionary<string, string>
            {
                { DiagnosticMetadataAttributesForObjectBinding.BoundType, domainClass },
                { DiagnosticMetadataAttributesForObjectBinding.BoundProperty, domainProperty }
            });
      }
      else
      {
        scope = context.Scope.FindDMA (RootTag, DiagnosticMetadataAttributesForObjectBinding.BoundProperty, domainProperty);
      }

      return CreateControlObject (context, scope);
    }
  }
}