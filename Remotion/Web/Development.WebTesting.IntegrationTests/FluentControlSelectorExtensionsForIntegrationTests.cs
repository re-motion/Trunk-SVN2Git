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
    public static FluentControlSelector<CommandSelector, CommandControlObject> GetCommand (this IControlHost host)
    {
      return new FluentControlSelector<CommandSelector, CommandControlObject> (host, new CommandSelector());
    }

    public static FluentControlSelector<DropDownMenuSelector, DropDownMenuControlObject> GetDropDownMenu (this IControlHost host)
    {
      return new FluentControlSelector<DropDownMenuSelector, DropDownMenuControlObject> (host, new DropDownMenuSelector());
    }

    public static FluentControlSelector<FormGridSelector, FormGridControlObject> GetFormGrid (this IControlHost host)
    {
      return new FluentControlSelector<FormGridSelector, FormGridControlObject> (host, new FormGridSelector());
    }

    public static FluentControlSelector<AnchorSelector, AnchorControlObject> GetAnchor (this IControlHost host)
    {
      return new FluentControlSelector<AnchorSelector, AnchorControlObject> (host, new AnchorSelector());
    }

    public static FluentControlSelector<LabelSelector, LabelControlObject> GetLabel (this IControlHost host)
    {
      return new FluentControlSelector<LabelSelector, LabelControlObject> (host, new LabelSelector());
    }

    public static FluentControlSelector<ListMenuSelector, ListMenuControlObject> GetListMenu (this IControlHost host)
    {
      return new FluentControlSelector<ListMenuSelector, ListMenuControlObject> (host, new ListMenuSelector());
    }

    public static FluentControlSelector<SingleViewSelector, SingleViewControlObject> GetSingleView (this IControlHost host)
    {
      return new FluentControlSelector<SingleViewSelector, SingleViewControlObject> (host, new SingleViewSelector());
    }

    public static FluentControlSelector<TabbedMenuSelector, TabbedMenuControlObject> GetTabbedMenu (this IControlHost host)
    {
      return new FluentControlSelector<TabbedMenuSelector, TabbedMenuControlObject> (host, new TabbedMenuSelector());
    }

    public static FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> GetTabbedMultiView (this IControlHost host)
    {
      return new FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> (host, new TabbedMultiViewSelector());
    }

    public static FluentControlSelector<TabStripSelector, TabStripControlObject> GetTabStrip (this IControlHost host)
    {
      return new FluentControlSelector<TabStripSelector, TabStripControlObject> (host, new TabStripSelector());
    }

    public static FluentControlSelector<TextBoxSelector, TextBoxControlObject> GetTextBox (this IControlHost host)
    {
      return new FluentControlSelector<TextBoxSelector, TextBoxControlObject> (host, new TextBoxSelector());
    }

    public static FluentControlSelector<WebButtonSelector, WebButtonControlObject> GetWebButton (this IControlHost host)
    {
      return new FluentControlSelector<WebButtonSelector, WebButtonControlObject> (host, new WebButtonSelector());
    }
  }
}