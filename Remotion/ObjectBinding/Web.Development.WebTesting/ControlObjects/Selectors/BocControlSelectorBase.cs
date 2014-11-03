using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector base class for all business object controls.
  /// </summary>
  public abstract class BocControlSelectorBase<TControlObject>
      : TypedControlSelectorBase<TControlObject>,
          IPerDisplayNameControlSelector<TControlObject>,
          IPerDomainPropertyControlSelector<TControlObject>
      where TControlObject : BocControlObject
  {
    protected BocControlSelectorBase ([NotNull] string controlType)
        : base (controlType)
    {
    }

    public TControlObject SelectPerDisplayName (ControlSelectionContext context, string displayName)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("displayName", displayName);

      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributesForObjectBinding.DisplayName, displayName }
          });

      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerDomainProperty (ControlSelectionContext context, string domainProperty, string domainClass)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("domainProperty", domainProperty);

      var diagnosticMetadata = new Dictionary<string, string>
            {
                { DiagnosticMetadataAttributes.ControlType, ControlType },
                { DiagnosticMetadataAttributesForObjectBinding.BoundProperty, domainProperty }
            };
      
      if (domainClass != null)
        diagnosticMetadata.Add (DiagnosticMetadataAttributesForObjectBinding.BoundType, domainClass);

      var scope = context.Scope.FindDMA ("*", diagnosticMetadata);
      return CreateControlObject (context, scope);
    }
  }
}