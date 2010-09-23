// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using System.Web;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.UI.Design;

namespace Remotion.Web.UI.Controls
{
  /// <include file='doc\include\UI\Controls\DropDownMenu.xml' path='DropDownMenu/Class/*' />
  [Designer (typeof (WebControlDesigner))]
  public class DropDownMenu : MenuBase, IDropDownMenu
  {
    /// <summary> Only used by control developers. </summary>
    public static readonly string OnHeadTitleClickScript = "DropDownMenu_OnHeadControlClick();";

    private string _titleText = "";
    private IconInfo _titleIcon;
    private bool _enableGrouping = true;
    private bool _isBrowserCapableOfScripting;

    private Action<HtmlTextWriter> _renderHeadTitleMethod;

    public DropDownMenu (IControl ownerControl, Type[] supportedMenuItemTypes)
      :base(ownerControl, supportedMenuItemTypes)
    {
      Mode = MenuMode.DropDownMenu;
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
      if (!IsDesignMode)
      {
        var factory = SafeServiceLocator.Current.GetInstance<IClientScriptBehaviorFactory>();
        var clientScriptBahavior = factory.CreateClientScriptBehavior (Page.Context, this);
        _isBrowserCapableOfScripting = clientScriptBahavior.IsBrowserCapableOfScripting;
        RegisterHtmlHeadContents (Page.Context, HtmlHeadAppender.Current);
      }
    }

    public void RegisterHtmlHeadContents (HttpContextBase httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      var factory = SafeServiceLocator.Current.GetInstance<IDropDownMenuRendererFactory> ();
      var renderer = factory.CreateRenderer (httpContext, this, SafeServiceLocator.Current);
      renderer.RegisterHtmlHeadContents (htmlHeadAppender, this);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      for (int i = 0; i < MenuItems.Count; i++)
      {
        WebMenuItem menuItem = MenuItems[i];
        if (menuItem.Command != null)
        {
          menuItem.Command.RegisterForSynchronousPostBack (
              this, i.ToString(), string.Format ("DropDownMenu '{0}', MenuItem '{1}'", ID, menuItem.ItemID));
        }
      }
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
        WcagHelper.Instance.HandleError (1, this);

      var factory = SafeServiceLocator.Current.GetInstance<IDropDownMenuRendererFactory>();
      var renderer = factory.CreateRenderer (Page.Context, this, SafeServiceLocator.Current);
      renderer.Render (writer);
    }

    public string GetBindOpenEventScript (string elementReference, string menuIDReference, bool moveToMousePosition)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("elementReference", elementReference);
      ArgumentUtility.CheckNotNullOrEmpty ("menuIDReference", menuIDReference);

      return string.Format (
          "DropDownMenu_BindOpenEvent({0}, {1}, '{2}', {3}, {4});",
          elementReference,
          menuIDReference,
          GetEventType(),
          string.IsNullOrEmpty (GetSelectionCount) ? "null" : GetSelectionCount,
          moveToMousePosition ? "true" : "false"
          );
    }

    private string GetEventType ()
    {
      switch (Mode)
      {
        case MenuMode.DropDownMenu:
          return  "click";
        case MenuMode.ContextMenu:
          return "contextmenu";
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary> Only used by control developers. </summary>
    public void SetRenderHeadTitleMethodDelegate (Action<HtmlTextWriter> renderHeadTitleMethod)
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

    Action<HtmlTextWriter> IDropDownMenu.RenderHeadTitleMethod
    {
      get { return _renderHeadTitleMethod; }
    }

    string IDropDownMenu.MenuHeadClientID
    {
      get { return ClientID + "_MenuDiv"; }
    }

    public IconInfo TitleIcon
    {
      get { return _titleIcon; }
      set { _titleIcon = value; }
    }

    public bool IsBrowserCapableOfScripting
    {
      get { return IsDesignMode || _isBrowserCapableOfScripting; }
    }

    [DefaultValue (true)]
    public bool EnableGrouping
    {
      get { return _enableGrouping; }
      set { _enableGrouping = value; }
    }

    public MenuMode Mode { get; set; }
  }
}
