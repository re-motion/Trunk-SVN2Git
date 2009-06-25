// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Remotion.Web.UI.Design;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <include file='doc\include\UI\Controls\DropDownMenu.xml' path='DropDownMenu/Class/*' />
  [Designer (typeof (WebControlDesigner))]
  public class DropDownMenu : WebControl, IDropDownMenu, IPostBackEventHandler, IControlWithDesignTimeSupport
  {
    private static readonly object s_eventCommandClickEvent = new object();
    private static readonly object s_wxeFunctionCommandClickEvent = new object();

    /// <summary> Only used by control developers. </summary>
    public static readonly string OnHeadTitleClickScript = "DropDownMenu_OnHeadControlClick();";

    private string _titleText = "";
    private IconInfo _titleIcon;
    private bool _isReadOnly;
    private bool _enableGrouping = true;
    private string _getSelectionCount = "";
    private bool _canRender;

    private readonly WebMenuItemCollection _menuItems;
    private RenderMethod _renderHeadTitleMethod;

    public DropDownMenu (IControl ownerControl, Type[] supportedMenuItemTypes)
    {
      if (ownerControl == null)
        ownerControl = this;
      _menuItems = new WebMenuItemCollection (ownerControl, supportedMenuItemTypes);
    }

    public DropDownMenu (IControl ownerControl)
        : this (ownerControl, new[] { typeof (WebMenuItem) })
    {
    }

    public DropDownMenu ()
        : this (null, new[] { typeof (WebMenuItem) })
    {
    }

    protected override void OnInit (EventArgs e)
    {
      IServiceLocator locator;
      //HACK: for unit tests that do not define a service locator, but need control initialization
      try 
      {
        locator = ServiceLocator.Current;
      }
      catch (NullReferenceException)
      {
        _canRender = true;
        return;
      }

      var factory = locator.GetInstance<IDropDownMenuRendererFactory>();
      var preRenderer = factory.CreatePreRenderer (Context != null ? new HttpContextWrapper (Context) : null, this);
      _canRender = preRenderer.CanRender();
    }

    /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
    /// <param name="eventArgument"> &lt;index&gt; </param>
    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

      //  First part: index
      int index;
      try
      {
        if (eventArgument.Length == 0)
          throw new FormatException();
        index = int.Parse (eventArgument);
      }
      catch (FormatException)
      {
        throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<index>'.");
      }

      if (index >= _menuItems.Count)
      {
        throw new ArgumentOutOfRangeException (
            eventArgument,
            "Index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed menu items.'");
      }

      WebMenuItem item = _menuItems[index];
      if (item.Command == null)
      {
        throw new ArgumentOutOfRangeException (
            eventArgument, "The DropDownMenu '" + ID + "' does not have a command associated with menu item " + index + ".");
      }

      switch (item.Command.Type)
      {
        case CommandType.Event:
        {
          OnEventCommandClick (item);
          break;
        }
        case CommandType.WxeFunction:
        {
          OnWxeFunctionCommandClick (item);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Fires the <see cref="EventCommandClick"/> event. </summary>
    protected virtual void OnEventCommandClick (WebMenuItem item)
    {
      WebMenuItemClickEventHandler clickHandler = (WebMenuItemClickEventHandler) Events[s_eventCommandClickEvent];
      if (clickHandler != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (item);
        clickHandler (this, e);
      }

      if (item != null && item.Command != null)
        item.Command.OnClick();
    }

    /// <summary> Fires the <see cref="WxeFunctionCommandClick"/> event. </summary>
    protected virtual void OnWxeFunctionCommandClick (WebMenuItem item)
    {
      WebMenuItemClickEventHandler clickHandler = (WebMenuItemClickEventHandler) Events[s_wxeFunctionCommandClickEvent];
      if (clickHandler != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (item);
        clickHandler (this, e);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      var factory = ServiceLocator.Current.GetInstance<IDropDownMenuRendererFactory>();
      var preRenderer = factory.CreatePreRenderer (Context != null ? new HttpContextWrapper (Context) : null, this);
      preRenderer.PreRender();
    }

    /// <summary> Calls <see cref="Control.OnPreRender"/> on every invocation. </summary>
    /// <remarks> Used by the <see cref="WebControlDesigner"/>. </remarks>
    void IControlWithDesignTimeSupport.PreRenderForDesignMode ()
    {
      if (!IsDesignMode)
        throw new InvalidOperationException ("PreRenderChildControlsForDesignMode may only be called during design time.");
      EnsureChildControls();
      OnPreRender (EventArgs.Empty);
    }

    public override void RenderControl (HtmlTextWriter writer)
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);

      var factory = ServiceLocator.Current.GetInstance<IDropDownMenuRendererFactory>();
      var renderer = factory.CreateRenderer (Context != null ? new HttpContextWrapper (Context) : null, writer, this);
      renderer.Render();
    }

    public string GetOpenDropDownMenuEventReference (string eventReference)
    {
      if (string.IsNullOrEmpty (eventReference))
        eventReference = "null";

      string getSelectionCount = (StringUtility.IsNullOrEmpty (_getSelectionCount) ? "null" : _getSelectionCount);
      return "DropDownMenu_OnClick (this, '" + ClientID + "', " + getSelectionCount + ", " + eventReference + ");";
    }

    /// <summary> Only used by control developers. </summary>
    public void SetRenderHeadTitleMethodDelegate (RenderMethod renderHeadTitleMethod)
    {
      _renderHeadTitleMethod = renderHeadTitleMethod;
    }

    /// <remarks>
    ///   The value will not be HTML encoded.
    /// </remarks>
    public string TitleText
    {
      get { return _titleText; }
      set { _titleText = value; }
    }

    RenderMethod IDropDownMenu.RenderHeadTitleMethod
    {
      get { return _renderHeadTitleMethod; }
    }

    public IconInfo TitleIcon
    {
      get { return _titleIcon; }
      set { _titleIcon = value; }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    [Category ("Behavior")]
    [Description ("The menu items displayed by this drop down menu.")]
    [DefaultValue ((string) null)]
    public WebMenuItemCollection MenuItems
    {
      get { return _menuItems; }
    }

    IPage IDropDownMenu.Page
    {
      get { return new PageWrapper (Page); }
    }

    [DefaultValue (false)]
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    public bool IsDesignMode
    {
      get { return ControlHelper.IsDesignMode(this, Context); }
    }

    public bool CanRender
    {
      get { return IsDesignMode || _canRender; }
    }

    [DefaultValue (true)]
    public bool EnableGrouping
    {
      get { return _enableGrouping; }
      set { _enableGrouping = value; }
    }

    [DefaultValue ("")]
    public string GetSelectionCount
    {
      get { return _getSelectionCount; }
      set { _getSelectionCount = value; }
    }

    /// <summary> Occurs when a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category ("Action")]
    [Description ("Occurs when a command of type Event is clicked.")]
    public event WebMenuItemClickEventHandler EventCommandClick
    {
      add { Events.AddHandler (s_eventCommandClickEvent, value); }
      remove { Events.RemoveHandler (s_eventCommandClickEvent, value); }
    }

    /// <summary> Occurs when a command of type <see cref="CommandType.WxeFunction"/> is clicked. </summary>
    [Category ("Action")]
    [Description ("Occurs when a command of type WxeFunction is clicked.")]
    public event WebMenuItemClickEventHandler WxeFunctionCommandClick
    {
      add { Events.AddHandler (s_wxeFunctionCommandClickEvent, value); }
      remove { Events.RemoveHandler (s_wxeFunctionCommandClickEvent, value); }
    }

    protected virtual string CssClassHead
    {
      get { return "dropDownMenuHead"; }
    }

    protected virtual string CssClassHeadFocus
    {
      get { return "dropDownMenuHeadFocus"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="DropDownMenu"/>'s title. </summary>
    /// <remarks> Class: <c></c> </remarks>
    protected virtual string CssClassHeadTitle
    {
      get { return "dropDownMenuHeadTitle"; }
    }

    protected virtual string CssClassHeadTitleFocus
    {
      get { return "dropDownMenuHeadTitleFocus"; }
    }

    protected virtual string CssClassHeadButton
    {
      get { return "dropDownMenuHeadButton"; }
    }

    protected virtual string CssClassMenuButtonFocus
    {
      get { return "dropDownMenuButtonFocus"; }
    }

    protected virtual string CssClassPopUp
    {
      get { return "dropDownMenuPopUp"; }
    }

    protected virtual string CssClassItem
    {
      get { return "dropDownMenuItem"; }
    }

    protected virtual string CssClassItemFocus
    {
      get { return "dropDownMenuItemFocus"; }
    }

    protected virtual string CssClassItemTextPane
    {
      get { return "dropDownMenuItemTextPane"; }
    }

    protected virtual string CssClassItemIconPane
    {
      get { return "dropDownMenuItemIconPane"; }
    }
  }
}