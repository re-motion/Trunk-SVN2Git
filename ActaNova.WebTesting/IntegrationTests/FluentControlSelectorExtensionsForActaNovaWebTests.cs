using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace ActaNova.WebTesting.IntegrationTests
{
  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentControlSelectorExtensionsForIntegrationTests
  {
    public static FluentControlSelector<AnchorSelector, AnchorControlObject> GetAnchor (this IControlHost host)
    {
      return new FluentControlSelector<AnchorSelector, AnchorControlObject> (host, new AnchorSelector());
    }

    public static AnchorControlObject GetAnchor (this IControlHost host, string localID)
    {
      return host.GetAnchor().ByLocalID (localID);
    }

    public static FluentControlSelector<CommandSelector, CommandControlObject> GetCommand (this IControlHost host)
    {
      return new FluentControlSelector<CommandSelector, CommandControlObject> (host, new CommandSelector());
    }

    public static CommandControlObject GetCommand (this IControlHost host, string localID)
    {
      return host.GetCommand().ByLocalID (localID);
    }

    public static FluentControlSelector<DropDownListSelector, DropDownListControlObject> GetDropDownList (this IControlHost host)
    {
      return new FluentControlSelector<DropDownListSelector, DropDownListControlObject> (host, new DropDownListSelector());
    }

    public static DropDownListControlObject GetDropDownList (this IControlHost host, string localID)
    {
      return host.GetDropDownList().ByLocalID (localID);
    }

    public static FluentControlSelector<DropDownMenuSelector, DropDownMenuControlObject> GetDropDownMenu (this IControlHost host)
    {
      return new FluentControlSelector<DropDownMenuSelector, DropDownMenuControlObject> (host, new DropDownMenuSelector());
    }

    public static DropDownMenuControlObject GetDropDownMenu (this IControlHost host, string localID)
    {
      return host.GetDropDownMenu().ByLocalID (localID);
    }

    public static FluentControlSelector<ImageButtonSelector, ImageButtonControlObject> GetImageButton (this IControlHost host)
    {
      return new FluentControlSelector<ImageButtonSelector, ImageButtonControlObject> (host, new ImageButtonSelector());
    }

    public static ImageButtonControlObject GetImageButton (this IControlHost host, string localID)
    {
      return host.GetImageButton().ByLocalID (localID);
    }

    public static FluentControlSelector<FormGridSelector, FormGridControlObject> GetFormGrid (this IControlHost host)
    {
      return new FluentControlSelector<FormGridSelector, FormGridControlObject> (host, new FormGridSelector());
    }

    public static FormGridControlObject GetFormGrid (this IControlHost host, string localID)
    {
      return host.GetFormGrid().ByLocalID (localID);
    }

    public static FormGridControlObject GetOnlyFormGrid (this IControlHost host)
    {
      return host.GetFormGrid().Single();
    }

    public static FluentControlSelector<LabelSelector, LabelControlObject> GetLabel (this IControlHost host)
    {
      return new FluentControlSelector<LabelSelector, LabelControlObject> (host, new LabelSelector());
    }

    public static LabelControlObject GetLabel (this IControlHost host, string localID)
    {
      return host.GetLabel().ByLocalID (localID);
    }

    public static FluentControlSelector<ListMenuSelector, ListMenuControlObject> GetListMenu (this IControlHost host)
    {
      return new FluentControlSelector<ListMenuSelector, ListMenuControlObject> (host, new ListMenuSelector());
    }

    public static ListMenuControlObject GetListMenu (this IControlHost host, string localID)
    {
      return host.GetListMenu().ByLocalID (localID);
    }

    public static FluentControlSelector<ScopeSelector, ScopeControlObject> GetScope (this IControlHost host)
    {
      return new FluentControlSelector<ScopeSelector, ScopeControlObject> (host, new ScopeSelector());
    }

    public static ScopeControlObject GetScope (this IControlHost host, string localID)
    {
      return host.GetScope().ByLocalID (localID);
    }

    public static FluentControlSelector<SingleViewSelector, SingleViewControlObject> GetSingleView (this IControlHost host)
    {
      return new FluentControlSelector<SingleViewSelector, SingleViewControlObject> (host, new SingleViewSelector());
    }

    public static SingleViewControlObject GetSingleView (this IControlHost host, string localID)
    {
      return host.GetSingleView().ByLocalID (localID);
    }

    public static SingleViewControlObject GetOnlySingleView (this IControlHost host)
    {
      return host.GetSingleView().Single();
    }

    public static FluentControlSelector<TabbedMenuSelector, TabbedMenuControlObject> GetTabbedMenu (this IControlHost host)
    {
      return new FluentControlSelector<TabbedMenuSelector, TabbedMenuControlObject> (host, new TabbedMenuSelector());
    }

    public static TabbedMenuControlObject GetTabbedMenu (this IControlHost host, string localID)
    {
      return host.GetTabbedMenu().ByLocalID (localID);
    }

    public static TabbedMenuControlObject GetOnlyTabbedMenu (this IControlHost host)
    {
      return host.GetTabbedMenu().Single();
    }

    public static FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> GetTabbedMultiView (this IControlHost host)
    {
      return new FluentControlSelector<TabbedMultiViewSelector, TabbedMultiViewControlObject> (host, new TabbedMultiViewSelector());
    }

    public static TabbedMultiViewControlObject GetTabbedMultiView (this IControlHost host, string localID)
    {
      return host.GetTabbedMultiView().ByLocalID (localID);
    }

    public static TabbedMultiViewControlObject GetOnlyTabbedMultiView (this IControlHost host)
    {
      return host.GetTabbedMultiView().Single();
    }

    public static FluentControlSelector<TextBoxSelector, TextBoxControlObject> GetTextBox (this IControlHost host)
    {
      return new FluentControlSelector<TextBoxSelector, TextBoxControlObject> (host, new TextBoxSelector());
    }

    public static TextBoxControlObject GetTextBox (this IControlHost host, string localID)
    {
      return host.GetTextBox().ByLocalID (localID);
    }

    public static FluentControlSelector<WebButtonSelector, WebButtonControlObject> GetWebButton (this IControlHost host)
    {
      return new FluentControlSelector<WebButtonSelector, WebButtonControlObject> (host, new WebButtonSelector());
    }

    public static WebButtonControlObject GetWebButton (this IControlHost host, string localID)
    {
      return host.GetWebButton().ByLocalID (localID);
    }

    public static FluentControlSelector<WebTabStripSelector, WebTabStripControlObject> GetWebTabStrip (this IControlHost host)
    {
      return new FluentControlSelector<WebTabStripSelector, WebTabStripControlObject> (host, new WebTabStripSelector());
    }

    public static WebTabStripControlObject GetWebTabStrip (this IControlHost host, string localID)
    {
      return host.GetWebTabStrip().ByLocalID (localID);
    }

    public static WebTabStripControlObject GetOnlyWebTabStrip (this IControlHost host)
    {
      return host.GetWebTabStrip().Single();
    }

    public static FluentControlSelector<WebTreeViewSelector, WebTreeViewControlObject> GetWebTreeView (this IControlHost host)
    {
      return new FluentControlSelector<WebTreeViewSelector, WebTreeViewControlObject> (host, new WebTreeViewSelector());
    }

    public static WebTreeViewControlObject GetWebTreeView (this IControlHost host, string localID)
    {
      return host.GetWebTreeView().ByLocalID (localID);
    }

    public static FluentControlSelector<ActaNovaAutoCompleteReferenceValueSelector, ActaNovaAutoCompleteReferenceValueControlObject> GetAutoComplete (
        this IControlHost host)
    {
      return new FluentControlSelector<ActaNovaAutoCompleteReferenceValueSelector, ActaNovaAutoCompleteReferenceValueControlObject> (
          host,
          new ActaNovaAutoCompleteReferenceValueSelector());
    }

    public static ActaNovaAutoCompleteReferenceValueControlObject GetAutoComplete (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetAutoComplete().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocBooleanValueSelector, BocBooleanValueControlObject> GetBooleanValue (this IControlHost host)
    {
      return new FluentControlSelector<BocBooleanValueSelector, BocBooleanValueControlObject> (host, new BocBooleanValueSelector());
    }

    public static BocBooleanValueControlObject GetBooleanValue (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetBooleanValue().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocCheckBoxSelector, BocCheckBoxControlObject> GetCheckBox (this IControlHost host)
    {
      return new FluentControlSelector<BocCheckBoxSelector, BocCheckBoxControlObject> (host, new BocCheckBoxSelector());
    }

    public static BocCheckBoxControlObject GetCheckBox (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetCheckBox().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocDateTimeValueSelector, BocDateTimeValueControlObject> GetDateTimeValue (this IControlHost host)
    {
      return new FluentControlSelector<BocDateTimeValueSelector, BocDateTimeValueControlObject> (host, new BocDateTimeValueSelector());
    }

    public static BocDateTimeValueControlObject GetDateTimeValue (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetDateTimeValue().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocEnumValueSelector, BocEnumValueControlObject> GetEnumValue (this IControlHost host)
    {
      return new FluentControlSelector<BocEnumValueSelector, BocEnumValueControlObject> (host, new BocEnumValueSelector());
    }

    public static BocEnumValueControlObject GetEnumValue (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetEnumValue().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocListSelector, BocListControlObject> GetList (this IControlHost host)
    {
      return new FluentControlSelector<BocListSelector, BocListControlObject> (host, new BocListSelector());
    }

    public static BocListControlObject GetList (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetList().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocListAsGridSelector, BocListAsGridControlObject> GetListAsGrid (this IControlHost host)
    {
      return new FluentControlSelector<BocListAsGridSelector, BocListAsGridControlObject> (host, new BocListAsGridSelector());
    }

    public static BocListAsGridControlObject GetListAsGrid (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetListAsGrid().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocMultilineTextValueSelector, BocMultilineTextValueControlObject> GetMultilineTextValue (
        this IControlHost host)
    {
      return new FluentControlSelector<BocMultilineTextValueSelector, BocMultilineTextValueControlObject> (host, new BocMultilineTextValueSelector());
    }

    public static BocMultilineTextValueControlObject GetMultilineTextValue (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetMultilineTextValue().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocReferenceValueSelector, BocReferenceValueControlObject> GetReferenceValue (this IControlHost host)
    {
      return new FluentControlSelector<BocReferenceValueSelector, BocReferenceValueControlObject> (host, new BocReferenceValueSelector());
    }

    public static BocReferenceValueControlObject GetReferenceValue (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetReferenceValue().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocTextValueSelector, BocTextValueControlObject> GetTextValue (this IControlHost host)
    {
      return new FluentControlSelector<BocTextValueSelector, BocTextValueControlObject> (host, new BocTextValueSelector());
    }

    public static BocTextValueControlObject GetTextValue (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetTextValue().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<BocTreeViewSelector, BocTreeViewControlObject> GetTreeView (this IControlHost host)
    {
      return new FluentControlSelector<BocTreeViewSelector, BocTreeViewControlObject> (host, new BocTreeViewSelector());
    }

    public static BocTreeViewControlObject GetTreeView (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetTreeView().ByDomainProperty (domainProperty, domainClass);
    }

    public static FluentControlSelector<ActaNovaTreeViewAutoCompleteReferenceValueSelector, ControlObjects.ActaNovaTreeViewAutoCompleteReferenceValueControlObject>
        GetTreeViewAutoComplete (this IControlHost host)
    {
      return new FluentControlSelector<ActaNovaTreeViewAutoCompleteReferenceValueSelector, ControlObjects.ActaNovaTreeViewAutoCompleteReferenceValueControlObject> (
          host,
          new ActaNovaTreeViewAutoCompleteReferenceValueSelector());
    }

    public static ControlObjects.ActaNovaTreeViewAutoCompleteReferenceValueControlObject GetTreeViewAutoComplete (
        this IControlHost host,
        string domainProperty,
        string domainClass = null)
    {
      return host.GetTreeViewAutoComplete().ByDomainProperty (domainProperty, domainClass);
    }

    public static ActaNovaEditObjectPanelControlObject GetDialog (this IControlHost host)
    {
      return host.GetControl (new SingleControlSelectionCommand<ActaNovaEditObjectPanelControlObject> (new ActaNovaEditObjectPanelSelector()));
    }

    public static ActaNovaDownLevelDmsControlObject GetOnlyDownLevelDms (this IControlHost host)
    {
      return host.GetControl (new SingleControlSelectionCommand<ActaNovaDownLevelDmsControlObject> (new ActaNovaDownLevelDmsSelector()));
    }
  }
}