/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.UI.Design;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

  /// <include file='doc\include\UI\Controls\DropDownMenu.xml' path='DropDownMenu/Class/*' />
  [Designer (typeof (WebControlDesigner))]
  public class DropDownMenu : WebControl, IControl, IPostBackEventHandler, IControlWithDesignTimeSupport
  {
    private static readonly object s_eventCommandClickEvent = new object ();
    private static readonly object s_wxeFunctionCommandClickEvent = new object ();

    private const string c_dropDownIcon = "DropDownMenuArrow.gif";

    /// <summary> Only used by control developers. </summary>
    public static readonly string OnHeadTitleClickScript = "DropDownMenu_OnHeadControlClick();";

    private string _titleText = "";
    private IconInfo _titleIcon = null;
    private bool _isReadOnly = false;
    private bool _enableGrouping = true;
    private string _getSelectionCount = "";

    private WebMenuItemCollection _menuItems;
    private RenderMethod _renderHeadTitleMethod;

    public DropDownMenu (Control ownerControl, Type[] supportedMenuItemTypes)
    {
      if (ownerControl == null)
        ownerControl = this;
      _menuItems = new WebMenuItemCollection (ownerControl, supportedMenuItemTypes);
    }

    public DropDownMenu (Control ownerControl)
      : this (ownerControl, new Type[] { typeof (WebMenuItem) })
    {
    }

    public DropDownMenu ()
      : this (null, new Type[] { typeof (WebMenuItem) })
    {
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
          throw new FormatException ();
        index = int.Parse (eventArgument);
      }
      catch (FormatException)
      {
        throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: '<index>'.");
      }

      if (index >= _menuItems.Count)
        throw new ArgumentOutOfRangeException (eventArgument, "Index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed menu items.'");

      WebMenuItem item = _menuItems[index];
      if (item.Command == null)
        throw new ArgumentOutOfRangeException (eventArgument, "The DropDownMenu '" + ID + "' does not have a command associated with menu item " + index + ".");

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
      if (item != null && item.Command != null)
        item.Command.OnClick ();

      WebMenuItemClickEventHandler clickHandler = (WebMenuItemClickEventHandler) Events[s_eventCommandClickEvent];
      if (clickHandler != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (item);
        clickHandler (this, e);
      }
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

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      string key = typeof (DropDownMenu).FullName + "_Script";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        string url = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.js");
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, url);
      }

      key = typeof (DropDownMenu).FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      //  Startup script initalizing the global values of the script.
      string key = typeof (DropDownMenu).FullName + "_Startup";
      if (!Page.ClientScript.IsStartupScriptRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (this, Context, typeof (DropDownMenu), ResourceType.Html, "DropDownMenu.css");
        string script = string.Format ("DropDownMenu_InitializeGlobals ('{0}');", styleSheetUrl);
        ScriptUtility.RegisterStartupScriptBlock (Page, key, script);
      }

      key = UniqueID;
      if (Enabled
          && !Page.ClientScript.IsStartupScriptRegistered (key))
      {
        StringBuilder script = new StringBuilder ();
        script.Append ("DropDownMenu_AddMenuInfo (\r\n\t");
        script.AppendFormat ("new DropDownMenu_MenuInfo ('{0}', new Array (\r\n", ClientID);
        bool isFirstItem = true;

        WebMenuItem[] menuItems;
        if (_enableGrouping)
          menuItems = _menuItems.GroupMenuItems (true);
        else
          menuItems = _menuItems.ToArray ();

        string category = null;
        bool isCategoryVisible = false;
        for (int i = 0; i < menuItems.Length; i++)
        {
          WebMenuItem menuItem = menuItems[i];
          if (_enableGrouping && category != menuItem.Category)
          {
            category = menuItem.Category;
            isCategoryVisible = false;
          }
          if (!menuItem.EvaluateVisible ())
            continue;
          if (_enableGrouping && menuItem.IsSeparator && !isCategoryVisible)
            continue;
          if (_enableGrouping)
            isCategoryVisible = true;
          if (isFirstItem)
            isFirstItem = false;
          else
            script.AppendFormat (",\r\n");
          AppendMenuItem (script, menuItem, _menuItems.IndexOf (menuItem));

        }
        script.Append (" )"); // Close Array
        script.Append (" )"); // Close new MenuInfo
        script.Append (" );"); // Close AddMenuInfo
        ScriptUtility.RegisterStartupScriptBlock (Page, key, script.ToString ());
      }
    }

    private void AppendMenuItem (StringBuilder stringBuilder, WebMenuItem menuItem, int menuItemIndex)
    {
      string href = "null";
      string target = "null";

      bool isCommandEnabled = true;
      if (menuItem.Command != null)
      {
        bool isActive = menuItem.Command.Show == CommandShow.Always
                        || _isReadOnly && menuItem.Command.Show == CommandShow.ReadOnly
                        || !_isReadOnly && menuItem.Command.Show == CommandShow.EditMode;

        isCommandEnabled = isActive && menuItem.Command.Type != CommandType.None;
        if (isCommandEnabled)
        {
          bool isPostBackCommand = menuItem.Command.Type == CommandType.Event
                                  || menuItem.Command.Type == CommandType.WxeFunction;
          if (isPostBackCommand)
          {
            // Clientside script creates an anchor with href="#" and onclick=function
            string argument = menuItemIndex.ToString ();
            href = Page.ClientScript.GetPostBackClientHyperlink (this, argument);
            href = ScriptUtility.EscapeClientScript (href);
            href = "'" + href + "'";

            menuItem.Command.RegisterForSynchronousPostBack (this, argument, string.Format ("DropDownMenu '{0}', MenuItem '{1}'", ID, menuItem.ItemID));
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString (), menuItem.ItemID);
            if (!ControlHelper.IsDesignMode (this, Context))
              href = UrlUtility.GetAbsoluteUrl (Page, href);
            href = "'" + href + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;
      string icon = "null";
      if (showIcon && menuItem.Icon.HasRenderingInformation)
        icon = "'" + UrlUtility.ResolveUrl (menuItem.Icon.Url) + "'";
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
        disabledIcon = "'" + UrlUtility.ResolveUrl (menuItem.DisabledIcon.Url) + "'";
      string text = showText ? "'" + menuItem.Text + "'" : "null";

      bool isDisabled = !menuItem.EvaluateEnabled () || !isCommandEnabled;
      stringBuilder.AppendFormat (
          "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
          menuItemIndex.ToString (),
          menuItem.Category,
          text,
          icon,
          disabledIcon,
          (int) menuItem.RequiredSelection,
          isDisabled ? "true" : "false",
          href,
          target);
    }

    /// <summary> Calls <see cref="Control.OnPreRender"/> on every invocation. </summary>
    /// <remarks> Used by the <see cref="WebControlDesigner"/>. </remarks>
    void IControlWithDesignTimeSupport.PreRenderForDesignMode ()
    {
      if (!ControlHelper.IsDesignMode (this, Context))
        throw new InvalidOperationException ("PreRenderChildControlsForDesignMode may only be called during design time.");
      EnsureChildControls ();
      OnPreRender (EventArgs.Empty);
    }

    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      this.Style.Add ("display", "inline-block");
      base.AddAttributesToRender (writer);
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled () && WcagHelper.Instance.IsWaiConformanceLevelARequired ())
        WcagHelper.Instance.HandleError (1, this);

      //  Menu-Div filling the control's div is required to apply internal css attributes
      //  for position, width and height. This allows the Head and th popup-div to align themselves
      writer.AddStyleAttribute ("position", "relative");
      if (Enabled)
      {
        string script = GetOpenDropDownMenuEventReference (null);
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
      writer.AddAttribute ("id", ClientID + "_MenuDiv");
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Menu-Div

      RenderHead (writer);

      writer.RenderEndTag (); // Close Menu-Div
    }

    public string GetOpenDropDownMenuEventReference (string eventReference)
    {
      if (string.IsNullOrEmpty (eventReference))
        eventReference = "null";

      string getSelectionCount = (StringUtility.IsNullOrEmpty (_getSelectionCount) ? "null" : _getSelectionCount);
      return "DropDownMenu_OnClick (this, '" + ClientID + "', " + getSelectionCount + ", " + eventReference + ");";
    }

    private void RenderHead (HtmlTextWriter writer)
    {
      //  Head-Div is used to group the title and the button, providing a single point of reference
      //  for the popup-div.
      writer.AddStyleAttribute ("position", "relative");
      writer.AddAttribute ("id", ClientID + "_HeadDiv");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
      writer.AddAttribute ("OnMouseOver", "DropDownMenu_OnHeadMouseOver (this)");
      writer.AddAttribute ("OnMouseOut", "DropDownMenu_OnHeadMouseOut (this)");
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Drop Down Head-Div

      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);  // Begin Drop Down Button table
      writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      RenderHeadTitle (writer);
      RenderHeadButton (writer);

      writer.RenderEndTag ();
      writer.RenderEndTag (); // Close Drop Down Button table

      ////  Options Drop Down Button 
      //writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      //writer.AddStyleAttribute ("min-height", "1em");
      //writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
      //writer.AddStyleAttribute ("float", "right");
      //writer.AddStyleAttribute ("text-align", "center");
      //writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
      //writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin Drop Down Button-San
      //string url = ResourceUrlResolver.GetResourceUrl (
      //    this, Context, typeof (DropDownMenu), ResourceType.Image, c_dropDownIcon);
      //writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
      //writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
      //writer.AddStyleAttribute ("vertical-align", "middle");
      //writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
      //writer.RenderBeginTag (HtmlTextWriterTag.Input);
      //writer.RenderEndTag();
      //writer.RenderEndTag();  // Close Drop Down Button-Span
      //
      ////  TODO: IE 5.01 has trouble with height
      ////  Options Drop Down Titel
      //writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
      //writer.RenderBeginTag (HtmlTextWriterTag.Span);
      //writer.Write (_titleText);
      //writer.RenderEndTag();

      writer.RenderEndTag ();  // Close Drop Down Head-Div
    }

    /// <summary> Only used by control developers. </summary>
    public void SetRenderHeadTitleMethodDelegate (RenderMethod renderHeadTitleMethod)
    {
      _renderHeadTitleMethod = renderHeadTitleMethod;
    }

    private void RenderHeadTitle (HtmlTextWriter writer)
    {
      bool hasHeadTitleContents = true;
      if (_renderHeadTitleMethod == null)
      {
        bool hasTitleText = !StringUtility.IsNullOrEmpty (_titleText);
        bool hasTitleIcon = _titleIcon != null && !StringUtility.IsNullOrEmpty (_titleIcon.Url);
        hasHeadTitleContents = hasTitleText || hasTitleIcon;

        if (hasHeadTitleContents)
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");//"100%");
          writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
          writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

          if (Enabled)
          {
            writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin title tag
          }
          else
          {
            writer.AddStyleAttribute (HtmlTextWriterStyle.Color, "GrayText");
            writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin title tag
          }
          RenderIcon (writer, _titleIcon);
          writer.Write (_titleText);
          writer.RenderEndTag (); // Close title tag

          writer.RenderEndTag (); // Close td
        }
      }
      else
      {
        _renderHeadTitleMethod (writer, this);
      }

      if (hasHeadTitleContents)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.AddStyleAttribute ("padding-right", "0.3em");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        writer.RenderEndTag ();
      }
    }

    private void RenderIcon (HtmlTextWriter writer, IconInfo icon)
    {
      if (icon == null || StringUtility.IsNullOrEmpty (icon.Url))
        return;

      writer.AddAttribute (HtmlTextWriterAttribute.Src, icon.Url);
      if (!icon.Width.IsEmpty && !icon.Height.IsEmpty)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Width, icon.Width.ToString ());
        writer.AddAttribute (HtmlTextWriterAttribute.Height, icon.Height.ToString ());
      }
      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      writer.AddStyleAttribute ("margin-right", "0.3em");
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag ();
    }

    private void RenderHeadButton (HtmlTextWriter writer)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("text-align", "center");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.RenderBeginTag (HtmlTextWriterTag.A);  // Begin anchor

      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (DropDownMenu), ResourceType.Image, c_dropDownIcon);
      writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, string.Empty);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag (); // Close img

      writer.RenderEndTag (); // Close anchor

      writer.RenderEndTag (); // Close td
    }

    /// <remarks>
    ///   The value will not be HTML encoded.
    /// </remarks>
    public string TitleText
    {
      get { return _titleText; }
      set { _titleText = value; }
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

    [DefaultValue (false)]
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
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
    { get { return "dropDownMenuHead"; } }

    protected virtual string CssClassHeadFocus
    { get { return "dropDownMenuHeadFocus"; } }

    /// <summary> Gets the CSS-Class applied to the <see cref="DropDownMenu"/>'s title. </summary>
    /// <remarks> Class: <c></c> </remarks>
    protected virtual string CssClassHeadTitle
    { get { return "dropDownMenuHeadTitle"; } }

    protected virtual string CssClassHeadTitleFocus
    { get { return "dropDownMenuHeadTitleFocus"; } }

    protected virtual string CssClassHeadButton
    { get { return "dropDownMenuHeadButton"; } }

    protected virtual string CssClassMenuButtonFocus
    { get { return "dropDownMenuButtonFocus"; } }

    protected virtual string CssClassPopUp
    { get { return "dropDownMenuPopUp"; } }

    protected virtual string CssClassItem
    { get { return "dropDownMenuItem"; } }

    protected virtual string CssClassItemFocus
    { get { return "dropDownMenuItemFocus"; } }

    protected virtual string CssClassItemTextPane
    { get { return "dropDownMenuItemTextPane"; } }

    protected virtual string CssClassItemIconPane
    { get { return "dropDownMenuItemIconPane"; } }
  }

}
