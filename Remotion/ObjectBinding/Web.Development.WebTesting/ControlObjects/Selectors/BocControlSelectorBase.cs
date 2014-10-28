using System;
using System.Collections.Generic;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;

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

    public TControlObject SelectPerDisplayName (TestObjectContext context, string displayName)
    {
      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributesForObjectBinding.DisplayName, displayName }
          });

      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerDomainProperty (TestObjectContext context, string domainProperty, string domainClass)
    {
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