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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
public class WebTab: IControlItem, IControlStateManager
{
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;
  private string _itemID = "";
  private string _text = "";
  private IconInfo _icon;
  private bool _isVisible = true;
  private bool _isDisabled = false;
  private WebTabStrip _tabStrip;
  private bool _isSelected = false;
  private int _selectDesired = 0;
  private bool _isControlStateRestored;

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text, IconInfo icon)
  {
    ItemID = itemID;
    Text = text;
    _icon = icon;
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text, string iconUrl)
    : this (itemID, text, new IconInfo (iconUrl))
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text)
    : this (itemID, text, new IconInfo (string.Empty))
  {
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public WebTab()
  {
    _icon = new IconInfo();
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  private void OwnerControl_PreRender(object sender, EventArgs e)
  {
    if (Remotion.Web.Utilities.ControlHelper.IsDesignMode (_ownerControl))
      return;
    PreRender();
  }

  /// <summary> Is called when the <see cref="OwnerControl"/> is Pre-Rendered. </summary>
  protected virtual void PreRender()
  {
  }

  /// <summary> Sets this tab's <see cref="WebTabStrip"/>. </summary>
  protected internal virtual void SetTabStrip (WebTabStrip tabStrip)
  {
    _tabStrip = tabStrip; 
    if (_selectDesired == 1)
    {
      _selectDesired = 0;
      IsSelected = true;
    }
    else if (_selectDesired == -1)
    {
      _selectDesired = 0;
      IsSelected = false;
    }
  }

  /// <summary> Sets the tab's selection state. </summary>
  /// <exception cref="InvalidOperationException"> 
  ///   Thrown if <see cref="IsVisible"/> is <see langword="false"/> but <paramref name="value"/> is <see langword="true"/>.
  /// </exception>
  protected internal void SetSelected (bool value)
  {
    if (value && ! _isVisible)
      throw new InvalidOperationException (string.Format ("Cannot select tab '{0}' because it is invisible.", _itemID));
    if (value && _isDisabled)
      throw new InvalidOperationException (string.Format ("Cannot select tab '{0}' because it is disabled.", _itemID));
    _isSelected = value;
    if (_tabStrip == null)
      _selectDesired = value ? 1 : -1;
  }

  internal void OnSelectionChangedInternal ()
  {
    OnSelectionChanged ();
  }

  protected virtual void OnSelectionChanged ()
  {
  }

  public override string ToString()
  {
    string displayName = ItemID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Text;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets or sets the ID of this tab. </summary>
  /// <remarks> Must be unique within the collection of tabs. Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this tab.")]
  //No Default value
  [NotifyParentProperty (true)]
  [ParenthesizePropertyName (true)]
  public virtual string ItemID
  {
    get { return _itemID; }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      if (! StringUtility.IsNullOrEmpty (value))
      {
        WebTabCollection tabs = null;
        if (_tabStrip != null)
          tabs = _tabStrip.Tabs;
        if (tabs != null)
        {
          if (tabs.Find (value) != null)
            throw new ArgumentException (string.Format ("The collection already contains a tab with ItemID '{0}'.", value), "value");
        }
      }
      _itemID = value; 
    }
  }

  // TODO: Test if still required in VS 2005. Workaround for Designer bug: Get Accessor does not evalute.
  internal bool HasItemID()
  {
    return ! StringUtility.IsNullOrEmpty (_itemID);
  }

  /// <summary> Gets or sets the text displayed in this tab. </summary>
  /// <remarks> Must not be <see langword="null"/> or emtpy. The value will not be HTML encoded. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text displayed in this tab. Use '-' for a separator tab. The value will not be HTML encoded.")]
  //No Default value
  [NotifyParentProperty (true)]
  public virtual string Text
  {
    get { return _text; }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      _text = value; 
    }
  }

  /// <summary> Gets or sets the icon displayed in this tab. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The icon displayed in this tab.")]
  [NotifyParentProperty (true)]
  public virtual IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("False to hide the tab.")]
  [NotifyParentProperty (true)]
  [DefaultValue (true)]
  public bool IsVisible
  {
    get 
    { 
      return _isVisible; 
    }
    set 
    {
      _isVisible = value; 
      if (! _isVisible && _tabStrip != null)
        _tabStrip.Tabs.DeselectTabInternal (this);
    }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("True to manually disable the tab.")]
  [NotifyParentProperty (true)]
  [DefaultValue (false)]
  public bool IsDisabled
  {
    get 
    {
      return _isDisabled; 
    }
    set
    {
      _isDisabled = value; 
      if (_isDisabled && _tabStrip != null)
        _tabStrip.Tabs.DeselectTabInternal (this);
    }
  }

  private bool ShouldSerializeIcon()
  {
    return IconInfo.ShouldSerialize (_icon);
  }

  private void ResetIcon()
  {
    _icon.Reset();
  }

  /// <summary> Gets the <see cref="WebTabStrip"/> to which this tab belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTabStrip TabStrip
  {
    get { return _tabStrip; }
  }

  /// <summary> Gets or sets a flag that determines whether this node is the selected node of the tree view. </summary>
  /// <exception cref="InvalidOperationException"> 
  ///   Thrown if <see cref="IsVisible"/> is <see langword="false"/> but <paramref name="value"/> is <see langword="true"/>.
  /// </exception>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsSelected
  {
    get { return _isSelected; }
    set 
    {
      SetSelected (value);
      if (_tabStrip != null)
      {
        if (value)
          _tabStrip.SetSelectedTabInternal (this);
        else if (this == _tabStrip.SelectedTab)
          _tabStrip.SetSelectedTabInternal (null);
      }
    }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "Tab"; }
  }

  /// <summary> Gets or sets the control to which this object belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public Control OwnerControl
  {
    get { return OwnerControlImplementation;  }
    set { OwnerControlImplementation = value; }
  }

  protected virtual Control OwnerControlImplementation
  {
    get { return _ownerControl;  }
    set
    { 
      if (_ownerControl != value)
      {
        if (OwnerControl != null)
          OwnerControl.PreRender -= new EventHandler (OwnerControl_PreRender);
        _ownerControl = value;
        if (OwnerControl != null)
          OwnerControl.PreRender += new EventHandler (OwnerControl_PreRender);
        OnOwnerControlChanged();
      }
    }
  }

  public virtual bool EvaluateVisible ()
  {
    return IsVisible;
  }

  public virtual bool EvaluateEnabled ()
  {
    return !IsDisabled;
  }

  protected string GetPostBackClientEvent ()
  {
    try
    {
      //  VS.NET Designer Bug: VS does is not able to determine whether _tabStrip is null.
      if (ControlHelper.IsDesignMode (_tabStrip, System.Web.HttpContext.Current))
        return string.Empty;
      if (_tabStrip == null) 
        throw new InvalidOperationException ("The WebTab is not part of a WebTabStrip.");
      if (_tabStrip.Page == null) 
        throw new InvalidOperationException (string.Format ("WebTabStrip '{0}' is not part of a page.", _tabStrip.ID));
    }
    catch (NullReferenceException)
    {
      return string.Empty;
    }
    return _tabStrip.Page.ClientScript.GetPostBackClientHyperlink (_tabStrip, ItemID);
  }

  public virtual void RenderBeginTagForCommand (HtmlTextWriter writer, bool isEnabled, WebTabStyle style)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("style", style);
    if (isEnabled && ! _isDisabled)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, GetPostBackClientEvent ());
    }
    style.AddAttributesToRender (writer);
    writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor
  }

  public virtual void RenderEndTagForCommand (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    writer.RenderEndTag();
  }

  public virtual void RenderContents (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    bool hasIcon = _icon != null && ! StringUtility.IsNullOrEmpty (_icon.Url);
    bool hasText = ! StringUtility.IsNullOrEmpty (_text);
    if (hasIcon)
      _icon.Render (writer);
    else
      IconInfo.RenderInvisibleSpacer (writer);
    if (hasIcon && hasText)
      writer.Write ("&nbsp;");
    if (hasText)
      writer.Write (_text); // Do not HTML encode
    if (!hasIcon && !hasText)
      writer.Write ("&nbsp;");
  }

  public virtual void OnClick()
  {
  }

  public virtual void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    string key = ResourceManagerUtility.GetGlobalResourceKey (Text);
    if (! StringUtility.IsNullOrEmpty (key))
      Text = resourceManager.GetString (key);
    
    if (Icon != null)
      Icon.LoadResources (resourceManager);
  }

  void IControlStateManager.LoadControlState (object state)
  {
    if (_isControlStateRestored)
      return;
    _isControlStateRestored = true;
    LoadControlState (state);
  }

  protected virtual void LoadControlState (object state)
  {
    if (state == null)
      return;

    IsSelected = (bool) state;
  }

  object IControlStateManager.SaveControlState ()
  {
    return SaveControlState();
  }

  protected virtual object SaveControlState()
  {
    if (! IsSelected)
      return null;
    return IsSelected;
  }
}

/// <summary>
///   Represents the method that handles the <c>Click</c> event raised when clicking on a web tab.
/// </summary>
public delegate void WebTabClickEventHandler (object sender, WebTabClickEventArgs e);

/// <summary>
///   Provides data for the <c>Click</c> event.
/// </summary>
public class WebTabClickEventArgs: EventArgs
{
  /// <summary> The <see cref="WebTab"/> that was clicked. </summary>
  private WebTab _tab;

  /// <summary> Initializes an instance. </summary>
  public WebTabClickEventArgs (WebTab tab)
  {
    ArgumentUtility.CheckNotNull ("tab", tab);
    _tab = tab;
  }

  /// <summary> The <see cref="WebTab"/> that was clicked. </summary>
  public WebTab Tab
  {
    get { return _tab; }
  }
}

public class WebTabStyle: Style
{
  /// <exclude />
  [EditorBrowsable (EditorBrowsableState.Never)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new Color BorderColor 
  {
    get { return base.BorderColor; }
    set { base.BorderColor = value; }
  }

  /// <exclude />
  [EditorBrowsable (EditorBrowsableState.Never)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new BorderStyle BorderStyle
  {
    get { return base.BorderStyle; }
    set { base.BorderStyle = value; }
  }

  /// <exclude />
  [EditorBrowsable (EditorBrowsableState.Never)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new Unit BorderWidth
  {
    get { return base.BorderWidth; }
    set { base.BorderWidth = value; }
  }

  /// <exclude />
  [EditorBrowsable (EditorBrowsableState.Never)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new Unit Width
  {
    get { return base.Width; }
    set { base.Width = value; }
  }

  /// <exclude />
  [EditorBrowsable (EditorBrowsableState.Never)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new Unit Height
  {
    get { return base.Height; }
    set { base.Height = value; }
  }
}
}
