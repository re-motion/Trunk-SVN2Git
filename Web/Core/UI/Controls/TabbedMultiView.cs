using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

  /// <include file='doc\include\UI\Controls\TabbedMultiView.xml' path='TabbedMultiView/Class/*' />
  [ToolboxData ("<{0}:TabbedMultiView id=\"MultiView\" runat=\"server\"></{0}:TabbedMultiView>")]
  [DefaultEvent ("ActiveViewChanged")]
  public class TabbedMultiView : WebControl, IControl
  {
    // constants
    private const string c_itemIDSuffix = "_Tab";
    // statics

    // types

#if NET11
  [CLSCompliant (false)]
  protected internal class MultiView: Microsoft.Web.UI.WebControls.MultiPage
#else
    protected internal class MultiView : System.Web.UI.WebControls.MultiView
#endif
    {
#if NET11
    private bool _isLoadViewStateComplete = false;
#endif

      protected internal MultiView ()
      {
      }

      protected override ControlCollection CreateControlCollection ()
      {
        return new TabViewCollection (this);
      }

      protected override void AddedControl (Control control, int index)
      {
        TabView tabView = ArgumentUtility.CheckNotNullAndType<TabView> ("control", control);

        tabView.IsLazyLoadingEnabled = Parent.EnableLazyLoading;
        if (!Parent.EnableLazyLoading)
          tabView.EnsureLazyControls ();

        base.AddedControl (control, index);
      }

      internal void OnTabViewInserted (TabView view)
      {
        Parent.OnTabViewInserted (view);
      }

      internal void OnTabViewRemove (TabView view)
      {
        Parent.OnTabViewRemove (view);
      }

      internal void OnTabViewRemoved (TabView view)
      {
        Parent.OnTabViewRemoved (view);
      }

      protected new TabbedMultiView Parent
      {
        get { return (TabbedMultiView) base.Parent; }
      }

      protected override void OnActiveViewChanged (EventArgs e)
      {
        base.OnActiveViewChanged (e);

        ISmartNavigablePage smartNavigablePage = Page as ISmartNavigablePage;
        if (smartNavigablePage != null)
          smartNavigablePage.DiscardSmartNavigationData ();
      }

#if NET11
    protected override void LoadViewState(object savedState)
    {
      base.LoadViewState (savedState);
      _isLoadViewStateComplete = true;
    }

    public TabView GetActiveView()
    {
      int selectedView = SelectedIndex;
      if (selectedView >= 0 && selectedView < Controls.Count)
        return (TabView) Controls[selectedView];
      return null;
    }

    public void SetActiveView (TabView view)
    {
      int index = Controls.IndexOf (view);
      if (index < 0)
        throw new HttpException (string.Format ("The view {0} cannot be found inside {1}, the ActiveView must be a View control directly inside a MultiView.", (view == null) ? "null" : view.ID, ID));
      if (SelectedIndex != index)
      {
        SelectedIndex = index;
        if (_isLoadViewStateComplete || (Page != null && ! Page.IsPostBack))
          OnSelectedIndexChange (EventArgs.Empty);
      }
    }

    public event EventHandler ActiveViewChanged
    {
      add { SelectedIndexChange += value; }
      remove { SelectedIndexChange -= value; }
    }

    protected override Microsoft.Web.UI.WebControls.RenderPathID RenderPath
    {
      get 
      { 
    	  if (this.IsDesignMode)
		      return Microsoft.Web.UI.WebControls.RenderPathID.DesignerPath;
        else
	  	    return Microsoft.Web.UI.WebControls.RenderPathID.DownLevelPath;
      }
    }
#else
      protected override void OnPreRender (EventArgs e)
      {
        foreach (TabView view in Views)
          view.OverrideVisible ();

        base.OnPreRender (e);
      }
#endif
    }

    protected internal class MultiViewTab : WebTab
    {
      string _target;

      /// <summary> Initalizes a new instance. </summary>
      public MultiViewTab (string itemID, string text, IconInfo icon)
        : base (itemID, text, icon)
      {
      }

      /// <summary> Initalizes a new instance. </summary>
      public MultiViewTab (string itemID, string text, string iconUrl)
        : this (itemID, text, new IconInfo (iconUrl))
      {
      }

      /// <summary> Initalizes a new instance. </summary>
      public MultiViewTab (string itemID, string text)
        : this (itemID, text, string.Empty)
      {
      }

      /// <summary> Initalizes a new instance. </summary>
      public MultiViewTab ()
        : base ()
      {
      }

      public string Target
      {
        get { return _target; }
        set { _target = value; }
      }

      protected override void OnSelectionChanged ()
      {
        base.OnSelectionChanged ();

        TabbedMultiView multiView = ((TabbedMultiView) OwnerControl);
        TabView view = null;
        //  Cannot use FindControl without a Naming Container. Only during initialization phase of aspx.
        if (multiView.NamingContainer != null)
        {
          bool isPlaceHolderTab = _target == null;
          if (isPlaceHolderTab)
            view = multiView._placeHolderTabView;
          else
            view = (TabView) multiView.MultiViewInternal.FindControl (_target);
        }
        else
        {
          foreach (TabView tabView in multiView.MultiViewInternal.Controls)
          {
            if (tabView.ID == _target)
            {
              view = tabView;
              break;
            }
          }
        }
        multiView.SetActiveView (view);
      }
    }


    // fields

    private bool _enableLazyLoading = false;
    private bool _isInitialized = false;
    private WebTabStrip _tabStrip;
    private TabbedMultiView.MultiView _multiViewInternal;
    private PlaceHolder _topControl;
    private PlaceHolder _bottomControl;

    private Style _activeViewStyle;
    private Style _topControlsStyle;
    private Style _bottomControlsStyle;

    private TabView _newActiveTabAfterRemove;
    private EmptyTabView _placeHolderTabView;

    // construction and destruction
    public TabbedMultiView ()
    {
      CreateControls ();
      _activeViewStyle = new Style ();
      _topControlsStyle = new Style ();
      _bottomControlsStyle = new Style ();
    }

    // methods and properties

    private void CreateControls ()
    {
      _tabStrip = new WebTabStrip (this);
      _multiViewInternal = new TabbedMultiView.MultiView ();
      _topControl = new PlaceHolder ();
      _bottomControl = new PlaceHolder ();
      _placeHolderTabView = new EmptyTabView ();
    }

    protected override void CreateChildControls ()
    {
      _tabStrip.ID = ID + "_TabStrip";
      Controls.Add (_tabStrip);

      _multiViewInternal.ID = ID + "_MultiView";
      Controls.Add (_multiViewInternal);

      _topControl.ID = ID + "_TopControl";
      Controls.Add (_topControl);

      _bottomControl.ID = ID + "_BottomControl";
      Controls.Add (_bottomControl);
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      _isInitialized = true;

      string key = typeof (TabbedMultiView).FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (TabbedMultiView), ResourceType.Html, "TabbedMultiView.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      TabView view = (TabView) MultiViewInternal.GetActiveView ();
      if (view != null)
        view.EnsureLazyControls ();
    }

    private void OnTabViewInserted (TabView view)
    {
      EnsureChildControls ();

      MultiViewTab tab = new MultiViewTab ();
      tab.ItemID = view.ID + c_itemIDSuffix;
      tab.Text = view.Title;
      tab.Icon = view.Icon;
      tab.Target = view.ID;
      _tabStrip.Tabs.Add (tab);

      if (Views.Count == 2 && Views.IndexOf (_placeHolderTabView) > 0)
        Views.Remove (_placeHolderTabView);
#if ! NET11
      if (Views.Count == 1)
        _multiViewInternal.ActiveViewIndex = 0;
#endif
    }

    private void OnTabViewRemove (TabView view)
    {
      EnsureChildControls ();

      TabView activeView = GetActiveView ();
      if (view != null && view == activeView)
      {
        int index = MultiViewInternal.Controls.IndexOf (view);
        bool isLastTab = index == MultiViewInternal.Controls.Count - 1;
        if (isLastTab)
        {
          if (MultiViewInternal.Controls.Count > 1)
            _newActiveTabAfterRemove = (TabView) MultiViewInternal.Controls[index - 1];
          else // No Tabs left after this tab
            _newActiveTabAfterRemove = _placeHolderTabView;
        }
        else
        {
          _newActiveTabAfterRemove = (TabView) MultiViewInternal.Controls[index + 1];
        }
      }
      else
      {
        _newActiveTabAfterRemove = null;
      }
    }

    private void OnTabViewRemoved (TabView view)
    {
      EnsureChildControls ();

      WebTab tab = _tabStrip.Tabs.Find (view.ID + c_itemIDSuffix);
      if (tab == null)
        return;

      int tabIndex = _tabStrip.Tabs.IndexOf (tab);
      _tabStrip.Tabs.RemoveAt (tabIndex);

      if (_newActiveTabAfterRemove != null)
      {
        if (_newActiveTabAfterRemove == _placeHolderTabView)
          Views.Add (_placeHolderTabView);
        SetActiveView (_newActiveTabAfterRemove);
      }
    }

#if NET11
  [CLSCompliant (false)]
#endif
    public void SetActiveView (TabView view)
    {
      ArgumentUtility.CheckNotNull ("view", view);
      MultiViewInternal.SetActiveView (view);
      TabView activeView = GetActiveView ();
      WebTab nextActiveTab = _tabStrip.Tabs.Find (activeView.ID + c_itemIDSuffix);
      nextActiveTab.IsSelected = true;
    }

#if NET11
  [CLSCompliant (false)]
#endif
    public TabView GetActiveView ()
    {
      TabView view = (TabView) MultiViewInternal.GetActiveView ();
      if (view != null && _isInitialized)
        view.EnsureLazyControls ();
      return view;
    }

    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      if (Views.Count == 0)
        Views.Add (_placeHolderTabView);

      ScriptUtility.RegisterElementForBorderSpans (Page, ActiveViewClientID);
      ScriptUtility.RegisterElementForBorderSpans (Page, _topControl.ClientID);
      ScriptUtility.RegisterElementForBorderSpans (Page, _bottomControl.ClientID);

      base.OnPreRender (e);
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      base.AddAttributesToRender (writer);
      if (ControlHelper.IsDesignMode (this, Context))
      {
        writer.AddStyleAttribute ("width", "100%");
        writer.AddStyleAttribute ("height", "75%");
      }
      if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      EnsureChildControls ();

      if (WcagHelper.Instance.IsWcagDebuggingEnabled () && WcagHelper.Instance.IsWaiConformanceLevelARequired ())
        WcagHelper.Instance.HandleError (1, this);


      if (!StringUtility.IsNullOrEmpty (CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClass);
      else if (!StringUtility.IsNullOrEmpty (Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, Attributes["class"]);
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      writer.RenderBeginTag (HtmlTextWriterTag.Table);

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTopControls (writer);
      writer.RenderEndTag ();

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTabStrip (writer);
      writer.RenderEndTag ();

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderActiveView (writer);
      writer.RenderEndTag ();
     
      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderBottomControls (writer);
      writer.RenderEndTag ();

      writer.RenderEndTag ();
    }

    protected virtual void RenderTabStrip (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabStrip);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      _tabStrip.CssClass = CssClassTabStrip;
      _tabStrip.RenderControl (writer);

      writer.RenderEndTag (); // end td
    }

    protected virtual void RenderActiveView (HtmlTextWriter writer)
    {
      if (ControlHelper.IsDesignMode (this, Context))
        writer.AddStyleAttribute ("border", "solid 1px black");
      _activeViewStyle.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (_activeViewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassActiveView);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      writer.AddAttribute (HtmlTextWriterAttribute.Id, ActiveViewClientID);
      _activeViewStyle.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (_activeViewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassActiveView);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassViewBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      Control view = _multiViewInternal.GetActiveView ();
      if (view != null)
      {
        for (int i = 0; i < view.Controls.Count; i++)
        {
          Control control = (Control) view.Controls[i];
          control.RenderControl (writer);
        }
      }

      writer.RenderEndTag (); // end content div
      writer.RenderEndTag (); // end body div
      writer.RenderEndTag (); // end outer div

      writer.RenderEndTag (); // end td
    }

    protected virtual void RenderTopControls (HtmlTextWriter writer)
    {
      Style style = _topControlsStyle;
      PlaceHolder placeHolder = _topControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (HtmlTextWriter writer)
    {
      Style style = _bottomControlsStyle;
      PlaceHolder placeHolder = _bottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (HtmlTextWriter writer, Style style, PlaceHolder placeHolder, string cssClass)
    {
      if (StringUtility.IsNullOrEmpty (style.CssClass))
      {
        if (placeHolder.Controls.Count > 0)
          writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
        else
          writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass + " " + CssClassEmpty);
      }
      else
      {
        if (placeHolder.Controls.Count > 0)
          writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass);
        else
          writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass + " " + CssClassEmpty);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      style.AddAttributesToRender (writer);
      if (StringUtility.IsNullOrEmpty (style.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      placeHolder.RenderControl (writer);

      writer.RenderEndTag (); // end content div
      writer.RenderEndTag (); // end outer div

      writer.RenderEndTag (); // end td
    }

    protected string ActiveViewClientID
    {
      get { return ClientID + "_ActiveView"; }
    }

    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls ();
        return base.Controls;
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [Browsable (false)]
    public TabViewCollection Views
    {
      get { return (TabViewCollection) MultiViewInternal.Controls; }
    }

    /// <summary>
    ///   Fired everytime the active view is changed after the <c>LoavViewState</c> phase or if it is not a post back.
    /// </summary>
    public event EventHandler ActiveViewChanged
    {
      add { MultiViewInternal.ActiveViewChanged += value; }
      remove { MultiViewInternal.ActiveViewChanged -= value; }
    }

    protected WebTabStrip TabStrip
    {
      get
      {
        EnsureChildControls ();
        return _tabStrip;
      }
    }

#if NET11
  [CLSCompliant (false)]
#endif
    protected TabbedMultiView.MultiView MultiViewInternal
    {
      get
      {
        EnsureChildControls ();
        return _multiViewInternal;
      }
    }

    public void EnsureAllLazyLoadedViews ()
    {
      foreach (TabView view in Views)
        view.EnsureLazyControls ();
    }

    [Category ("Behavior")]
    [Description ("Enables the lazy (i.e. On-Demand) loading of the individual tabs.")]
    [DefaultValue (false)]
    public bool EnableLazyLoading
    {
      get { return _enableLazyLoading; }
      set { _enableLazyLoading = value; }
    }

    [Category ("Style")]
    [Description ("The style that you want to apply to the active view.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style ActiveViewStyle
    {
      get { return _activeViewStyle; }
    }

    [Category ("Style")]
    [Description ("The style that you want to apply to a tab that is not selected.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public WebTabStyle TabStyle
    {
      get { return _tabStrip.TabStyle; }
    }

    [Category ("Style")]
    [Description ("The style that you want to apply to the selected tab.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public WebTabStyle SelectedTabStyle
    {
      get { return _tabStrip.SelectedTabStyle; }
    }

    [Category ("Style")]
    [Description ("The style that you want to the top section. The height cannot be provided in percent.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style TopControlsStyle
    {
      get { return _topControlsStyle; }
    }

    [Category ("Style")]
    [Description ("The style that you want to apply to the bottom section. The height cannot be provided in percent.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style BottomControlsStyle
    {
      get { return _bottomControlsStyle; }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Browsable (false)]
    public ControlCollection TopControls
    {
      get { return _topControl.Controls; }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Browsable (false)]
    public ControlCollection BottomControls
    {
      get { return _bottomControl.Controls; }
    }


    /// <summary>
    ///   Clears all items.
    /// </summary>
    /// <remarks>
    ///   Note that clearing <see cref="Views"/> is not sufficient, as other controls are created implicitly.
    /// </remarks>
    public void Clear ()
    {
      CreateControls ();
      Controls.Clear ();
      CreateChildControls ();
      //  Views.Clear();
      //  TabStrip.Tabs.Clear();
      //  MultiViewInternal.Controls.Clear();
    }

    #region protected virtual string CssClass...
    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiView</c>. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "tabbedMultiView"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewTabStrip</c>. </para>
    /// </remarks>
    protected virtual string CssClassTabStrip
    {
      get { return "tabbedMultiViewTabStrip"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s active view. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewActiveView</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ActiveViewStyle"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassActiveView
    {
      get { return "tabbedMultiViewActiveView"; }
    }

    /// <summary> Gets the CSS-Class applied to the top section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewTopControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="TopControlsStyle"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassTopControls
    {
      get { return "tabbedMultiViewTopControls"; }
    }

    /// <summary> Gets the CSS-Class applied to the bottom section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewBottomControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="BottomControlsStyle"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBottomControls
    {
      get { return "tabbedMultiViewBottomControls"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> wrapping the content and the border elements. </summary>
    /// <remarks> 
    ///   <para> Class: <c>body</c>. </para>
    /// </remarks>
    protected virtual string CssClassViewBody
    {
      get { return "body"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> intended for formatting the content. </summary>
    /// <remarks> 
    ///   <para> Class: <c>content</c>. </para>
    /// </remarks>
    protected virtual string CssClassContent
    {
      get { return "content"; }
    }
    
    /// <summary> Gets the CSS-Class applied when the section is empty. </summary>
    /// <remarks> 
    ///   <para> Class: <c>empty</c>. </para>
    ///   <para> 
    ///     Applied in addition to the regular CSS-Class. Use <c>td.tabbedMultiViewTopControls.emtpy</c> or 
    ///     <c>td.tabbedMultiViewBottomControls.emtpy</c>as a selector.
    ///   </para>
    /// </remarks>
    protected virtual string CssClassEmpty
    { get { return "empty"; } }
    #endregion
  }

}