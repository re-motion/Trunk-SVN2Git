using System;
using System.Collections.Generic;
using Coypu;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocEnumValueControlObject"/>.
  /// </summary>
  public class BocEnumValueSelector
      : ControlSelectorBase<BocEnumValueControlObject>,
          IFirstControlSelector<BocEnumValueControlObject>,
          IPerIndexControlSelector<BocEnumValueControlObject>,
          ISingleControlSelector<BocEnumValueControlObject>,
          IPerDisplayNameControlSelector<BocEnumValueControlObject>,
          IPerDomainPropertyControlSelector<BocEnumValueControlObject>
  {
    // Todo RM-6297: due to the fact that BocEnumValue may have different root tags (uaghs), we have to re-implement some stuff here...

    private const string c_cssClass = "bocEnumValue";
    private const string c_mainRootTag = "span";
    private const string c_alternativeRootTag = "div";

    public BocEnumValueControlObject SelectFirst (TestObjectContext context)
    {
      var scope = context.Scope.FindCss ("." + c_cssClass);
      return CreateControlObject (context, scope);
    }

    public BocEnumValueControlObject SelectSingle (TestObjectContext context)
    {
      var scope = context.Scope.FindCss ("." + c_cssClass, Options.Single);
      return CreateControlObject (context, scope);
    }

    public BocEnumValueControlObject SelectPerIndex (TestObjectContext context, int index)
    {
      var scope = context.Scope.FindXPath (
          string.Format (
              "(.//{0}{2}|.//{1}{2})[{3}]",
              c_mainRootTag,
              c_alternativeRootTag,
              XPathUtils.CreateContainsClassCheck (c_cssClass),
              index));

      return CreateControlObject (context, scope);
    }

    public BocEnumValueControlObject SelectPerDisplayName (TestObjectContext context, string displayName)
    {
      var scope = context.Scope.FindDMA (
          new[] { c_mainRootTag, c_alternativeRootTag },
          new Dictionary<string, string> { { DiagnosticMetadataAttributesForObjectBinding.DisplayName, displayName } });

      return CreateControlObject (context, scope);
    }

    public BocEnumValueControlObject SelectPerDomainProperty (TestObjectContext context, string domainProperty, string domainClass)
    {
      ElementScope scope;

      if (domainClass != null)
      {
        scope = context.Scope.FindDMA (
            new[] { c_mainRootTag, c_alternativeRootTag },
            new Dictionary<string, string>
            {
                { DiagnosticMetadataAttributesForObjectBinding.BoundType, domainClass },
                { DiagnosticMetadataAttributesForObjectBinding.BoundProperty, domainProperty }
            });
      }
      else
      {
        scope = context.Scope.FindDMA (
            new[] { c_mainRootTag, c_alternativeRootTag },
            new Dictionary<string, string> { { DiagnosticMetadataAttributesForObjectBinding.BoundProperty, domainProperty } });
      }

      return CreateControlObject (context, scope);
    }
  }
}