﻿using System;
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

    public static FluentControlSelector<BocBooleanValueSelector, BocBooleanValueControlObject> GetBooleanValue (this IControlHost host)
    {
      return new FluentControlSelector<BocBooleanValueSelector, BocBooleanValueControlObject> (host, new BocBooleanValueSelector());
    }

    public static FluentControlSelector<BocListSelector, BocListControlObject> GetList (this IControlHost host)
    {
      return new FluentControlSelector<BocListSelector, BocListControlObject> (host, new BocListSelector());
    }

    public static FluentControlSelector<BocReferenceValueSelector, BocReferenceValueControlObject> GetReferenceValue (this IControlHost host)
    {
      return new FluentControlSelector<BocReferenceValueSelector, BocReferenceValueControlObject> (host, new BocReferenceValueSelector());
    }

    public static FluentControlSelector<BocTextValueSelector, BocTextValueControlObject> GetTextValue (this IControlHost host)
    {
      return new FluentControlSelector<BocTextValueSelector, BocTextValueControlObject> (host, new BocTextValueSelector());
    }

    public static FluentControlSelector<BocTreeViewSelector, BocTreeViewControlObject> GetTreeView (this IControlHost host)
    {
      return new FluentControlSelector<BocTreeViewSelector, BocTreeViewControlObject> (host, new BocTreeViewSelector());
    }
  }
}