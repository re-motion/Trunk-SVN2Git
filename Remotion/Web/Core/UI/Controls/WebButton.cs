// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Globalization;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.WebButtonImplementation;
using Remotion.Web.UI.Controls.WebButtonImplementation.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A <c>Button</c> using <c>&amp;</c> as access key prefix in <see cref="Button.Text"/>. </summary>
  /// <include file='doc\include\UI\Controls\WebButton.xml' path='WebButton/Class/*' />
  [ToolboxData ("<{0}:WebButton runat=server></{0}:WebButton>")]
  public class WebButton
      :
          Button,
          IWebButton,
          // Required because Page.ProcessPostData always registers the last IPostBackEventHandler in the controls 
          // collection for controls (buttons) having PostData but no IPostBackDataHandler. 
          IPostBackDataHandler
  {
    private static readonly object s_clickEvent = new object();

    private IconInfo _icon;
    private PostBackOptions _options;

    private bool _useLegacyButton;
    private bool _isDefaultButton;

    private ISecurableObject _securableObject;
    private MissingPermissionBehavior _missingPermissionBehavior = MissingPermissionBehavior.Invisible;
    private bool _requiresSynchronousPostBack;

    private TextWithHotkey _textWithHotkey;

    public WebButton ()
    {
      _icon = new IconInfo();
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents (HtmlHeadAppender.Current);

      ScriptUtility.Instance.RegisterJavaScriptInclude (this, HtmlHeadAppender.Current);
    }

    protected virtual IWebButtonRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IWebButtonRenderer> ();
    }

    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
    }

    /// <remarks>
    ///   This method is never called if the button is rendered as a legacy button.
    /// </remarks>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      ArgumentUtility.CheckNotNull ("postCollection", postCollection);

      string eventTarget = postCollection[ControlHelper.PostEventSourceID];
      bool isScriptedPostBack = !StringUtility.IsNullOrEmpty (eventTarget);
      if (!isScriptedPostBack && IsLegacyButtonEnabled)
      {
        // The button can only fire a click event if client script is active or the button is used in legacy mode
        // A more general fallback is not possible becasue of compatibility issues with ExecuteFunctionNoRepost
        bool isSuccessfulControl = !StringUtility.IsNullOrEmpty (postCollection[postDataKey]);
        if (isSuccessfulControl)
          Page.RegisterRequiresRaiseEvent (this);
      }
      return false;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this, true) ?? NullResourceManager.Instance;
      LoadResources (resourceManager);

      if (_isDefaultButton && string.IsNullOrEmpty (Page.Form.DefaultButton))
        Page.Form.DefaultButton = UniqueID;

      if (_requiresSynchronousPostBack)
      {
        var scriptManager = ScriptManager.GetCurrent (base.Page);
        if (scriptManager != null)
          scriptManager.RegisterPostBackControl (this);
      }
    }

    protected override void Render (HtmlTextWriter writer)
    {
      _textWithHotkey = TextWithHotkey.Parse (Text);

      base.Render (writer);
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      if (string.IsNullOrEmpty (AccessKey))
        writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, HotkeyFormatter.FormatHotkey (_textWithHotkey));

      if (IsLegacyButtonEnabled)
      {
        if (!string.IsNullOrEmpty (_textWithHotkey.Text))
          Text = _textWithHotkey.Text;
        else if (_icon != null && _icon.HasRenderingInformation)
          Text = _icon.AlternateText;
      }

      if (Page != null)
        Page.VerifyRenderingInServerForm (this);

      if (IsEnabled)
      {
        string onClick = EnsureEndWithSemiColon (OnClientClick);
        if (HasAttributes)
        {
          string onClickAttribute = Attributes["onclick"];
          if (onClickAttribute != null)
          {
            onClick = onClick + EnsureEndWithSemiColon (onClickAttribute);
            Attributes.Remove ("onclick");
          }
        }

        if (Page != null)
        {
          PostBackOptions options = GetPostBackOptions();
          options.ClientSubmit = true;

          string postBackScript;
          if (options.PerformValidation)
            postBackScript = "this.disabled = true;";
          else
            postBackScript = "this.disabled = (typeof (Page_IsValid) == 'undefined' || Page_IsValid == null || Page_IsValid == true);";

          string postBackEventReference = Page.ClientScript.GetPostBackEventReference (options, false);
          if (StringUtility.IsNullOrEmpty (postBackEventReference))
            postBackEventReference = Page.ClientScript.GetPostBackEventReference (this, null);
          postBackScript += EnsureEndWithSemiColon (postBackEventReference);

          if (options.PerformValidation)
            postBackScript += "this.disabled = (typeof (Page_IsValid) == 'undefined' || Page_IsValid == null || Page_IsValid == true);";
          postBackScript += "return false;";

          if (postBackScript != null)
            onClick = MergeScript (onClick, postBackScript);
        }

        if (!StringUtility.IsNullOrEmpty (onClick))
          writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);

        writer.AddAttribute ("onmousedown", "WebButton_MouseDown (this, '" + CssClassMouseDown + "');");
        writer.AddAttribute ("onmouseup", "WebButton_MouseUp (this, '" + CssClassMouseDown + "');");
        writer.AddAttribute ("onmouseout", "WebButton_MouseOut (this, '" + CssClassMouseDown + "');");
      }


      _options = base.GetPostBackOptions();
      _options.ClientSubmit = false;
      _options.PerformValidation = false;
      _options.AutoPostBack = false;

      string backUpOnClientClick = OnClientClick;
      OnClientClick = null;

      base.AddAttributesToRender (writer);

      OnClientClick = backUpOnClientClick;

      _options = null;

      if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected override PostBackOptions GetPostBackOptions ()
    {
      if (_options == null)
        return base.GetPostBackOptions();
      else
        return _options;
    }

    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        if (IsLegacyButtonEnabled)
          return HtmlTextWriterTag.Input;
        //For new styles
        //if (ControlHelper.IsDesignMode (this))
        //  return HtmlTextWriterTag.Button;
        return HtmlTextWriterTag.Button;
      }
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      {
        if (!_useLegacyButton)
          WcagHelper.Instance.HandleError (1, this, "UseLegacyButton");
      }
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      EvaluateWaiConformity();

      if (IsLegacyButtonEnabled)
        return;

      //For new styles
      //if (ControlHelper.IsDesignMode (this))
      //  return;

      ScriptUtility.Instance.RegisterElementForBorderSpans (this, "#" + ClientID + " > *:first");

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButtonBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      var text = HotkeyFormatter.FormatText (_textWithHotkey, false);

      if (HasControls())
        base.RenderContents (writer);
      else
      {
        bool hasIcon = _icon != null && _icon.HasRenderingInformation;
        bool hasText = !StringUtility.IsNullOrEmpty (text);
        if (hasIcon)
        {
          writer.AddAttribute (HtmlTextWriterAttribute.Src, _icon.Url);
          if (!_icon.Height.IsEmpty)
            writer.AddAttribute (HtmlTextWriterAttribute.Height, _icon.Height.ToString());
          if (!_icon.Width.IsEmpty)
            writer.AddAttribute (HtmlTextWriterAttribute.Width, _icon.Width.ToString());
          writer.AddStyleAttribute ("vertical-align", "middle");
          writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
          writer.AddAttribute (HtmlTextWriterAttribute.Alt, _icon.AlternateText);
          writer.RenderBeginTag (HtmlTextWriterTag.Img);
          writer.RenderEndTag();
        }
        if (hasIcon && hasText)
          writer.Write ("&nbsp;");
        if (hasText)
          writer.Write (text); // Do not HTML enocde
      }

      writer.RenderEndTag(); // End acnhorBody span
    }

    /// <summary> Gets or sets the icon displayed in this menu item. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The icon displayed.")]
    [NotifyParentProperty (true)]
    public IconInfo Icon
    {
      get { return _icon; }
      set { _icon = value; }
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      if (ControlHelper.IsDesignMode (this))
        return;

      //  Dispatch simple properties
      string key = ResourceManagerUtility.GetGlobalResourceKey (Text);
      if (!StringUtility.IsNullOrEmpty (key))
        Text = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (AccessKey);
      if (!StringUtility.IsNullOrEmpty (key))
        AccessKey = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
      if (!StringUtility.IsNullOrEmpty (key))
        ToolTip = resourceManager.GetString (key);

      if (Icon != null)
        Icon.LoadResources (resourceManager);
    }

    /// <summary> 
    ///   Gets or sets the flag that determines whether to use a legacy (i.e. input) element for the button or the modern form (i.e. button). 
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> to enable the legacy version. Defaults to <see langword="false"/>.
    /// </value>
    [Description ("Determines whether to use a legacy (i.e. input) element for the button or the modern form (i.e. button).")]
    [Category ("Behavior")]
    [DefaultValue (false)]
    public bool UseLegacyButton
    {
      get { return _useLegacyButton; }
      set { _useLegacyButton = value; }
    }

    protected bool IsLegacyButtonEnabled
    {
      get { return WcagHelper.Instance.IsWaiConformanceLevelARequired() || _useLegacyButton; }
    }

    [Category ("Behavior")]
    [DefaultValue (false)]
    public bool IsDefaultButton
    {
      get { return _isDefaultButton; }
      set { _isDefaultButton = value; }
    }

    private string EnsureEndWithSemiColon (string value)
    {
      if (!StringUtility.IsNullOrEmpty (value))
      {
        value = value.Trim();

        if (!value.EndsWith (";"))
          value += ";";
      }

      return value;
    }

    private string MergeScript (string firstScript, string secondScript)
    {
      if (!StringUtility.IsNullOrEmpty (firstScript))
        return (firstScript + secondScript);
      if (secondScript.TrimStart (new char[0]).StartsWith ("javascript:"))
        return secondScript;
      return ("javascript:" + secondScript);
    }

    protected bool HasAccess ()
    {
      IWebSecurityAdapter securityAdapter = AdapterRegistry.Instance.GetAdapter<IWebSecurityAdapter>();
      if (securityAdapter == null)
        return true;

      EventHandler clickHandler = (EventHandler) Events[s_clickEvent];
      if (clickHandler == null)
        return true;

      return securityAdapter.HasAccess (_securableObject, Events[s_clickEvent]);
    }

    public override bool Visible
    {
      get
      {
        if (!base.Visible)
          return false;

        if (_missingPermissionBehavior == MissingPermissionBehavior.Invisible)
          return HasAccess();

        return true;
      }
      set { base.Visible = value; }
    }

    public override bool Enabled
    {
      get
      {
        if (!base.Enabled)
          return false;

        if (_missingPermissionBehavior == MissingPermissionBehavior.Disabled)
          return HasAccess();

        return true;
      }
      set { base.Enabled = value; }
    }

    [Category ("Action")]
    public new event EventHandler Click
    {
      add { Events.AddHandler (s_clickEvent, value); }
      remove { Events.RemoveHandler (s_clickEvent, value); }
    }

    protected override void OnClick (EventArgs e)
    {
      base.OnClick (e);

      EventHandler clickHandler = (EventHandler) Events[s_clickEvent];
      if (clickHandler != null)
        clickHandler (this, e);
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public ISecurableObject SecurableObject
    {
      get { return _securableObject; }
      set { _securableObject = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [NotifyParentProperty (true)]
    [DefaultValue (MissingPermissionBehavior.Invisible)]
    public MissingPermissionBehavior MissingPermissionBehavior
    {
      get { return _missingPermissionBehavior; }
      set { _missingPermissionBehavior = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("True to require a synchronous postback within Ajax Update Panels.")]
    [DefaultValue (false)]
    public bool RequiresSynchronousPostBack
    {
      get { return _requiresSynchronousPostBack; }
      set { _requiresSynchronousPostBack = value; }
    }

    public new IPage Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }


    protected virtual IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    private IHotkeyFormatter HotkeyFormatter
    {
      get { return ServiceLocator.GetInstance<IHotkeyFormatter>(); }
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebButton"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>webButton</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "webButton"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> intended for formatting the content. </summary>
    /// <remarks> 
    ///   <para> Class: <c>content</c>. </para>
    /// </remarks>
    protected virtual string CssClassButtonBody
    {
      get { return "buttonBody"; }
    }

    /// <summary> Gets the CSS-Class applied when the section is empty. </summary>
    /// <remarks> 
    ///   <para> Class: <c>mouseDown</c>. </para>
    ///   <para> 
    ///     Applied in addition to the regular CSS-Class. Use <c>a.webButton.mouseDown</c>as a selector.
    ///   </para>
    /// </remarks>
    protected virtual string CssClassMouseDown
    {
      get { return "mouseDown"; }
    }

    #endregion
  }
}
