using System;
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

    public static FluentControlSelector<HtmlAnchorSelector, HtmlAnchorControlObject> GetHtmlAnchor (this IControlHost host)
    {
      return new FluentControlSelector<HtmlAnchorSelector, HtmlAnchorControlObject> (host, new HtmlAnchorSelector());
    }

    public static FluentControlSelector<ListMenuSelector, ListMenuControlObject> GetListMenu (this IControlHost host)
    {
      return new FluentControlSelector<ListMenuSelector, ListMenuControlObject> (host, new ListMenuSelector());
    }

    public static FluentControlSelector<CommandSelector, CommandControlObject> GetCommand (this IControlHost host)
    {
      return new FluentControlSelector<CommandSelector, CommandControlObject> (host, new CommandSelector());
    }
  }
}