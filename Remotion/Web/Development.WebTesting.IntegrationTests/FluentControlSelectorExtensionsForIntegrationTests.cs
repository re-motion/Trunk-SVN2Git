﻿using System;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentControlSelectorExtensionsForIntegrationTests
  {
    public static FluentControlSelector<FormGridSelector, FormGridControlObject> GetFormGrid (this IControlHost host)
    {
      return new FluentControlSelector<FormGridSelector, FormGridControlObject> (host, new FormGridSelector());
    }

    public static FluentControlSelector<SingleViewSelector, SingleViewControlObject> GetSingleView (this IControlHost host)
    {
      return new FluentControlSelector<SingleViewSelector, SingleViewControlObject> (host, new SingleViewSelector());
    }

    public static FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> GetTabbedMultiView (this IControlHost host)
    {
      return new FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> (host, new TabbedMultiViewSelector());
    }

    public static FluentControlSelector<TabStripSelector, TabStripControlObject> GetTabStrip (this IControlHost host)
    {
      return new FluentControlSelector<TabStripSelector, TabStripControlObject> (host, new TabStripSelector());
    }

    public static FluentControlSelector<WebButtonSelector, WebButtonControlObject> GetWebButton (this IControlHost host)
    {
      return new FluentControlSelector<WebButtonSelector, WebButtonControlObject> (host, new WebButtonSelector());
    }
  }
}