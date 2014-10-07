using System;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentControlSelectorExtensionsForIntegrationTests
  {
    public static FluentControlSelector<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject> GetAutoComplete (
        this IControlHost host)
    {
      return new FluentControlSelector<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject> (
          host,
          new BocAutoCompleteReferenceValueSelector());
    }

    public static FluentControlSelector<BocListSelector, BocListControlObject> GetList (this IControlHost host)
    {
      return new FluentControlSelector<BocListSelector, BocListControlObject> (host, new BocListSelector());
    }

    public static FluentControlSelector<BocTextSelector, BocTextControlObject> GetText (this IControlHost host)
    {
      return new FluentControlSelector<BocTextSelector, BocTextControlObject> (host, new BocTextSelector());
    }
  }
}