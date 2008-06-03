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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

  //  TODO: Command: Move long comment blocks to xml-file
  /// <summary> A <see cref="Command"/> defines an action the user can invoke. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class Command : IControlItem
  {
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class EventCommandInfo
    {
    //  EventPermissionProvider; //None, EventHandler, Properties
    //  public class Permission
    //  {
    //    WxeFunctionType;
    //    Method;
    //    AccessTypes;
    //  }

      private bool _requiresSynchronousPostBack;

      public EventCommandInfo ()
      {
      }

      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("True to require a synchronous postback within Ajax Update Panels.")]
      [DefaultValue (false)]
      [NotifyParentProperty (true)]
      public bool RequiresSynchronousPostBack
      {
        get { return _requiresSynchronousPostBack; }
        set { _requiresSynchronousPostBack = value; }
      }

      /// <summary> Returns a string representation of this <see cref="EventCommandInfo"/>. </summary>
      /// <returns> A <see cref="string"/>. </returns>
      public override string ToString ()
      {
        StringBuilder stringBuilder = new StringBuilder (50);

        if (_requiresSynchronousPostBack)
          return "Synchronous Postback required";
        else
          return string.Empty;
      }
    }

    /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class HrefCommandInfo
    {
      private string _href = string.Empty;
      private string _target = string.Empty;

      /// <summary> Simple constructor. </summary>
      public HrefCommandInfo ()
      {
      }

      /// <summary> Returns a string representation of this <see cref="HrefCommandInfo"/>. </summary>
      /// <remarks> Format: Href, Target </remarks>
      /// <returns> A <see cref="string"/>. </returns>
      public override string ToString ()
      {
        StringBuilder stringBuilder = new StringBuilder (50);

        if (_href == string.Empty || _target == string.Empty)
          return _href;
        else
          return _href + ", " + _target;
      }

      public virtual string FormatHref (params string[] parameters)
      {
        string[] encodedParameters = new string[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
          if (HttpContext.Current != null)
            encodedParameters[i] = HttpUtility.UrlEncode (parameters[i], HttpContext.Current.Response.ContentEncoding);
          else
            encodedParameters[i] = "";
        }
        return string.Format (Href, encodedParameters);
      }

      /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
      /// <value> 
      ///   The URL to link to when the rendered command is clicked. The default value is 
      ///   an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The hyperlink reference of the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string Href
      {
        get
        {
          return _href;
        }
        set
        {
          _href = StringUtility.NullToEmpty (value);
          _href = _href.Trim ();
        }
      }

      /// <summary> 
      ///   Gets or sets the target window or frame to display the web page specified by <see cref="Href"/> 
      ///   when  the rendered command is clicked.
      /// </summary>
      /// <value> 
      ///   The target window or frame to load the web page specified by <see cref="Href"/> when the rendered command
      ///   is clicked.  The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The target window or frame of the command. Leave it blank for no target.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string Target
      {
        get
        {
          return _target;
        }
        set
        {
          _target = StringUtility.NullToEmpty (value);
          _target = _target.Trim ();
        }
      }
    }

    /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class WxeFunctionCommandInfo
    {
      private string _mappingID = string.Empty;
      private string _typeName = string.Empty;
      private string _parameters = string.Empty;
      private string _target = string.Empty;

      /// <summary> Simple constructor. </summary>
      public WxeFunctionCommandInfo ()
      {
      }

      /// <summary>
      ///   Returns a string representation of this <see cref="WxeFunctionCommandInfo"/>.
      /// </summary>
      /// <remarks> Format: Href, Target </remarks>
      /// <returns> A <see cref="string"/>. </returns>
      public override string ToString ()
      {
        if (_typeName == string.Empty)
          return string.Empty;
        else
          return _typeName + " (" + _parameters + ")";
      }

      /// <summary> 
      ///   Gets or sets the complete type name of the <see cref="WxeFunction"/> to call when the rendered 
      ///   command is clicked. Either the <see cref="TypeName"/> or the <see cref="MappingID"/> is required.
      /// </summary>
      /// <value> 
      ///   The complete type name of the <see cref="WxeFunction"/> to call when the rendered command is clicked. 
      ///   The default value is an empty <see cref="String"/>. 
      /// </value>
      /// <remarks>
      ///   Valid type names include the classic .net syntax and typenames using the abbreviated form as defined by the
      ///   <see cref="TypeUtility.ParseAbbreviatedTypeName">TypeUtility.ParseAbbreviatedTypeName</see> method.
      ///   In ASP.NET 2.0, it is possible to use functions located in the <b>App_Code</b> assembly by not specifying an
      ///   assembly name.
      /// </remarks>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The complete type name (type[, assembly]) of the WxeFunction used for the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string TypeName
      {
        get
        {
          return _typeName;
        }
        set
        {
          _typeName = StringUtility.NullToEmpty (value);
          _typeName = _typeName.Trim ();
        }
      }

      /// <summary> 
      ///   Gets or sets the ID of the function as defined in the <see cref="UrlMappingEntry"/>.
      ///   Either the <see cref="TypeName"/> or the <see cref="MappingID"/> is required.
      /// </summary>
      /// <value> 
      ///   The <see cref="UrlMappingEntry.ID"/> associated with the <see cref="WxeFunction"/> to call when the 
      ///   rendered command is clicked. The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The Url-Mapping ID associated with the WxeFunction used for the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public string MappingID
      {
        get
        {
          return _mappingID;
        }
        set
        {
          _mappingID = StringUtility.NullToEmpty (value);
          _mappingID = _mappingID.Trim ();
        }
      }

      /// <summary> 
      ///   Gets or sets the comma separated list of parameters passed to the <see cref="WxeFunction"/> when the 
      ///   rendered command is clicked.
      /// </summary>
      /// <value> 
      ///   The comma separated list of parameters passed to the <see cref="WxeFunction"/> when the rendered command 
      ///   is clicked. The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("A comma separated list of parameters for the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string Parameters
      {
        get
        {
          return _parameters;
        }
        set
        {
          _parameters = StringUtility.NullToEmpty (value);
          _parameters = _parameters.Trim ();
        }
      }

      /// <summary> 
      ///   Gets or sets the target window or frame to open the <see cref="WxeFunction"/> when the rendered command is 
      ///   clicked.
      /// </summary>
      /// <value> 
      ///   The target window or frame to open the Wxe <see cref="WxeFunction"/> when the rendered command is clicked. 
      ///   The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behaviour")]
      [Description ("The target window or frame of the command. Leave it blank for no target.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public string Target
      {
        get { return _target; }
        set
        {
          _target = StringUtility.NullToEmpty (value);
          _target = _target.Trim ();
        }
      }


      public virtual WxeFunction InitializeFunction (NameObjectCollection additionalWxeParameters)
      {
        Type functionType = ResolveFunctionType ();
        WxeFunction function = (WxeFunction) Activator.CreateInstance (functionType);

        function.InitializeParameters (_parameters, additionalWxeParameters);

        return function;
      }

      public virtual Type ResolveFunctionType ()
      {
        UrlMappingEntry mapping = UrlMappingConfiguration.Current.Mappings.FindByID (_mappingID);

        bool hasMapping = mapping != null;
        bool hasTypeName = !StringUtility.IsNullOrEmpty (_typeName);

        Type functionType = null;
        if (hasTypeName)
          functionType = WebTypeUtility.GetType (_typeName, true, false);

        if (hasMapping)
        {
          if (functionType == null)
          {
            functionType = mapping.FunctionType;
          }
          else if (mapping.FunctionType != functionType)
          {
            throw new InvalidOperationException (string.Format (
                "The WxeFunctionCommand in has both a MappingID ('{0}') and a TypeName ('{1}') defined, but they resolve to different WxeFunctions.",
                _mappingID, _typeName));
          }
        }
        else if (!hasTypeName)
        {
          throw new InvalidOperationException ("The WxeFunctionCommand has no valid MappingID or FunctionTypeName specified.");
        }

        return functionType;
      }
    }

    private string _toolTip = string.Empty;
    private CommandType _type;
    private readonly CommandType _defaultType = CommandType.None;
    private CommandShow _show = CommandShow.Always;
    private EventCommandInfo _eventCommand = new EventCommandInfo ();
    private HrefCommandInfo _hrefCommand = new HrefCommandInfo ();
    private WxeFunctionCommandInfo _wxeFunctionCommand = new WxeFunctionCommandInfo ();
    //private ScriptCommandInfo _scriptCommand = null;
    private bool _hasClickFired = false;

    private Control _ownerControl = null;

    [Browsable (false)]
    public CommandClickEventHandler Click;

    public Command ()
      : this (CommandType.None)
    {
    }

    public Command (CommandType defaultType)
    {
      _defaultType = defaultType;
      _type = _defaultType;
    }

    /// <summary> Fires the <see cref="Click"/> event. </summary>
    public virtual void OnClick ()
    {
      if (_hasClickFired)
        return;
      _hasClickFired = true;
      if (Click != null)
        Click (OwnerControl, new CommandClickEventArgs (this));
    }

    /// <summary> Renders the opening tag for the command. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back.
    /// </param>
    /// <param name="parameters">
    ///   The strings inserted into the href attribute using <c>string.Format</c>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="securableObject">
    ///   The <see cref="ISecurableObject"/> for which security is evaluated. Use <see landword="null"/> if security is stateless or not evaluated.
    /// </param>
    /// <param name="additionalUrlParameters">
    ///   The <see cref="NameValueCollection"/> containing additional url parameters.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="includeNavigationUrlParameters"> 
    ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
    /// </param>
    /// <param name="style"> The style applied to the opening tag. </param>
    public virtual void RenderBegin (
        HtmlTextWriter writer,
        string postBackEvent,
        string[] parameters,
        string onClick,
        ISecurableObject securableObject,
        NameValueCollection additionalUrlParameters,
        bool includeNavigationUrlParameters,
        Style style)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("style", style);

      if (HasAccess (securableObject))
      {
        switch (_type)
        {
          case CommandType.Href:
            AddAttributesToRenderForHrefCommand (writer, parameters, onClick, additionalUrlParameters, includeNavigationUrlParameters);
            break;
          case CommandType.Event:
            AddAttributesToRenderForEventCommand (writer, postBackEvent, onClick);
            break;
          case CommandType.WxeFunction:
            AddAttributesToRenderForWxeFunctionCommand (writer, postBackEvent, onClick, additionalUrlParameters, includeNavigationUrlParameters);
            break;
          case CommandType.None:
            break;
          default:
            throw new InvalidOperationException (string.Format ("The CommandType '{0}' is not supported by the '{1}'.", _type, typeof (Command).FullName));
        }
      }
      style.AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
    }

    /// <summary> Renders the opening tag for the command. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back.
    /// </param>
    /// <param name="parameters">
    ///   The strings inserted into the href attribute using <c>string.Format</c>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="securableObject">
    ///   The <see cref="ISecurableObject"/> for which security is evaluated. Use <see landword="null"/> if security is stateless or not evaluated.
    /// </param>
    public void RenderBegin (HtmlTextWriter writer, string postBackEvent, string[] parameters, string onClick, ISecurableObject securableObject)
    {
      RenderBegin (writer, postBackEvent, parameters, onClick, securableObject, new NameValueCollection (0), true, new Style ());
    }

    /// <summary> Adds the attributes for the Href command to the anchor tag. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
    /// <param name="parameters">
    ///   The strings inserted into the href attribute using <c>string.Format</c>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="additionalUrlParameters">
    ///   The <see cref="NameValueCollection"/> containing additional url parameters.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="includeNavigationUrlParameters"> 
    ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
    ///   Defaults to <see langword="true"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.Href"/>.
    /// </exception> 
    protected virtual void AddAttributesToRenderForHrefCommand (
        HtmlTextWriter writer,
        string[] parameters,
      string onClick,
      NameValueCollection additionalUrlParameters,
      bool includeNavigationUrlParameters)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters);
      if (Type != CommandType.Href)
        throw new InvalidOperationException ("Call to AddAttributesToRenderForHrefCommand not allowed unless Type is set to CommandType.Href.");

      string href = HrefCommand.FormatHref (parameters);
      if (HttpContext.Current != null)
      {
        if (includeNavigationUrlParameters)
        {
          ISmartNavigablePage page = null;
          if (OwnerControl != null)
            page = OwnerControl.Page as ISmartNavigablePage;

          if (page != null)
          {
            additionalUrlParameters = NameValueCollectionUtility.Clone (additionalUrlParameters);
            NameValueCollectionUtility.Append (additionalUrlParameters, page.GetNavigationUrlParameters ());
          }
        }
        href = UrlUtility.AddParameters (href, additionalUrlParameters);
        href = UrlUtility.GetAbsoluteUrl (HttpContext.Current, href);
      }
      writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
      if (!StringUtility.IsNullOrEmpty (HrefCommand.Target))
        writer.AddAttribute (HtmlTextWriterAttribute.Target, HrefCommand.Target);
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
      if (!StringUtility.IsNullOrEmpty (_toolTip))
        writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);
    }

    /// <summary> Adds the attributes for the Event command to the anchor tag. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.Event"/>.
    /// </exception> 
    protected virtual void AddAttributesToRenderForEventCommand (
        HtmlTextWriter writer,
        string postBackEvent,
      string onClick)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent);
      if (Type != CommandType.Event)
        throw new InvalidOperationException ("Call to AddAttributesToRenderForEventCommand not allowed unless Type is set to CommandType.Event.");

      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + StringUtility.NullToEmpty (onClick));
      if (!StringUtility.IsNullOrEmpty (_toolTip))
        writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);
    }

    /// <summary> Adds the attributes for the Wxe Function command to the anchor tag. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="additionalUrlParameters">
    ///   The <see cref="NameValueCollection"/> containing additional url parameters.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="includeNavigationUrlParameters"> 
    ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
    /// </exception> 
    protected virtual void AddAttributesToRenderForWxeFunctionCommand (
        HtmlTextWriter writer,
        string postBackEvent,
      string onClick,
      NameValueCollection additionalUrlParameters,
      bool includeNavigationUrlParameters)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent);
      if (Type != CommandType.WxeFunction)
        throw new InvalidOperationException ("Call to AddAttributesToRenderForWxeFunctionCommand not allowed unless Type is set to CommandType.WxeFunction.");

      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + StringUtility.NullToEmpty (onClick));
      if (!StringUtility.IsNullOrEmpty (_toolTip))
        writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);
    }

    /// <summary> Renders the closing tag for the command. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
    public virtual void RenderEnd (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      writer.RenderEndTag ();
    }

    /// <summary>
    ///   Returns a string representation of this <see cref="Command"/>.
    /// </summary>
    /// <remarks>
    ///   <list type="table">
    ///     <listheader>
    ///     <term>Type</term> 
    ///     <description>Format</description>
    ///     </listheader>
    ///     <item>
    ///       <term>Href</term>
    ///       <description> Href: &lt;HrefCommand.ToString()&gt; </description>
    ///     </item>
    ///     <item>
    ///       <term>WxeFunction</term>
    ///       <description> WxeFunction: &lt;WxeFunctionCommand.ToString()&gt; </description>
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <returns> A <see cref="string"/>. </returns>
    public override string ToString ()
    {
      StringBuilder stringBuilder = new StringBuilder (50);

      stringBuilder.Append (Type.ToString ());

      switch (Type)
      {
        case CommandType.Href:
          if (HrefCommand != null)
            stringBuilder.AppendFormat (": {0}", HrefCommand.ToString ());
          break;
        case CommandType.WxeFunction:
          if (WxeFunctionCommand != null)
            stringBuilder.AppendFormat (": {0}", WxeFunctionCommand.ToString ());
          break;
        default:
          break;
      }

      return stringBuilder.ToString ();
    }

    /// <summary> Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. </summary>
    /// <param name="wxePage"> 
    ///   The <see cref="IWxePage"/> where this command is rendered on. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="additionalWxeParameters"> 
    ///   The parameters passed to the <see cref="WxeFunction"/> in addition to the executing function's variables.
    ///   Use <see langword="null"/> or an empty collection if all parameters are supplied by the 
    ///   <see cref="WxeFunctionCommandInfo.Parameters"/> string and the function stack.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   <para>
    ///     Thrown if called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
    ///   </para><para>
    ///     Thrown if neither the <see cref="WxeFunctionCommandInfo.MappingID"/> nor the 
    ///     <see cref="WxeFunctionCommandInfo.TypeName"/> are set.
    ///   </para><para>
    ///     Thrown if the <see cref="WxeFunctionCommandInfo.MappingID"/> and <see cref="WxeFunctionCommandInfo.TypeName"/>
    ///     specify different functions.
    ///   </para>
    /// </exception> 
    public virtual void ExecuteWxeFunction (IWxePage wxePage, NameObjectCollection additionalWxeParameters)
    {
      ArgumentUtility.CheckNotNull ("wxePage", wxePage);

      if (Type != CommandType.WxeFunction)
        throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

      if (!wxePage.IsReturningPostBack)
      {
        string target = WxeFunctionCommand.Target;
        bool hasTarget = !StringUtility.IsNullOrEmpty (target);
        WxeFunction function = WxeFunctionCommand.InitializeFunction (additionalWxeParameters);

        if (hasTarget)
          wxePage.ExecuteFunctionExternal (function, target, null, false);
        else
          wxePage.ExecuteFunction (function);
      }
    }

    /// <summary> The <see cref="CommandType"/> represented by this instance of <see cref="Command"/>. </summary>
    /// <value> One of the <see cref="CommandType"/> enumeration values. The default is <see cref="CommandType.None"/>. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The type of command generated.")]
    [NotifyParentProperty (true)]
    public CommandType Type
    {
      get { return _type; }
      set { _type = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The ToolTip/Title rendered in the anchor tag.")]
    [NotifyParentProperty (true)]
    [DefaultValue ("")]
    public string ToolTip
    {
      get { return _toolTip; }
      set
      {
        _toolTip = StringUtility.NullToEmpty (value);
        _toolTip = _toolTip.Trim ();
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsDefaultType
    {
      get { return _type == _defaultType; }
    }

    /// <summary> Controls the persisting of the <see cref="Type"/>. </summary>
    protected bool ShouldSerializeType ()
    {
      return !IsDefaultType;
    }

    /// <summary> Sets the <see cref="Type"/> to its default value. </summary>
    protected void ResetType ()
    {
      _type = _defaultType;
    }

    /// <summary>
    ///   Determines when the item command is shown to the user in regard of the parent control's read-only setting.
    /// </summary>
    /// <value> 
    ///   One of the <see cref="CommandShow"/> enumeration values. The default is <see cref="CommandShow.Always"/>.
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("Determines when to show the item command to the user in regard to the parent control's read-only setting.")]
    [DefaultValue (CommandShow.Always)]
    [NotifyParentProperty (true)]
    public CommandShow Show
    {
      get { return _show; }
      set { _show = value; }
    }

    /// <summary>
    ///   The <see cref="EventCommandInfo"/> used when rendering the command as an event.
    /// </summary>
    /// <remarks> 
    ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Event"/>.
    /// </remarks>
    /// <value> A <see cref="EventCommandInfo"/> object. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The properties of the event. Interpreted if Type is set to Event.")]
    [DefaultValue ((string) null)]
    [NotifyParentProperty (true)]
    public virtual EventCommandInfo EventCommand
    {
      get { return _eventCommand; }
      set { _eventCommand = value; }
    }

    /// <summary>
    ///   The <see cref="HrefCommandInfo"/> used when rendering the command as a hyperlink.
    /// </summary>
    /// <remarks> 
    ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Href"/>.
    /// </remarks>
    /// <value> A <see cref="HrefCommandInfo"/> object. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The properties of the hyperlink. Interpreted if Type is set to Href.")]
    [DefaultValue ((string) null)]
    [NotifyParentProperty (true)]
    public virtual HrefCommandInfo HrefCommand
    {
      get { return _hrefCommand; }
      set { _hrefCommand = value; }
    }

    /// <summary>
    ///   The <see cref="WxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
    /// </summary>
    /// <remarks> 
    ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>.
    /// </remarks>
    /// <value> A <see cref="WxeFunctionCommandInfo"/> object. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The properties of the WxeFunction. Interpreted if Type is set to WxeFunction.")]
    [DefaultValue ((string) null)]
    [NotifyParentProperty (true)]
    public virtual WxeFunctionCommandInfo WxeFunctionCommand
    {
      get { return _wxeFunctionCommand; }
      set { _wxeFunctionCommand = value; }
    }

    /// <summary> Gets or sets the control to which this object belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public Control OwnerControl
    {
      get { return OwnerControlImplementation; }
      set { OwnerControlImplementation = value; }
    }

    protected virtual Control OwnerControlImplementation
    {
      get { return _ownerControl; }
      set
      {
        if (_ownerControl != value)
          _ownerControl = value;
      }
    }

    string IControlItem.ItemID
    {
      get { return null; }
    }

    public virtual void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;

      string key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
      if (!StringUtility.IsNullOrEmpty (key))
        ToolTip = resourceManager.GetString (key);
    }

    public void RegisterForSynchronousPostBack (Control control, string argument, string commandID)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("commandID", commandID);

      ScriptManager scriptManager = ScriptManager.GetCurrent (control.Page);
      if (scriptManager != null)
      {
        bool hasUpdatePanelAsParent = false;
        for (Control current = control; current != null && !(current is Page); current = current.Parent)
        {
          if (current is UpdatePanel)
          {
            hasUpdatePanelAsParent = true;
            break;
          }
        }

        if (hasUpdatePanelAsParent)
        {
          if (Type == CommandType.Event && EventCommand.RequiresSynchronousPostBack)
          {
            ISmartPage smartPage = control.Page as ISmartPage;
            if (smartPage == null)
            {
              throw new InvalidOperationException (
                  string.Format (
                      "{0}: EventCommands with RequiresSynchronousPostBack set to true are only supported on pages implementing ISmartPage when used within an UpdatePanel.",
                      commandID));
            }
            smartPage.RegisterCommandForSynchronousPostBack (control, argument);
          }
          else if (Type == CommandType.WxeFunction && StringUtility.IsNullOrEmpty (WxeFunctionCommand.Target))
          {
            ISmartPage smartPage = control.Page as ISmartPage;
            if (smartPage == null)
            {
              throw new InvalidOperationException (
                  string.Format (
                      "{0}: WxeCommands are only supported on pages implementing ISmartPage when used within an UpdatePanel.",
                      commandID));
            }
            smartPage.RegisterCommandForSynchronousPostBack (control, argument);
          }
        }          
      }
    }

    public virtual bool HasAccess (ISecurableObject securableObject)
    {
      switch (_type)
      {
        case CommandType.Href:
          return true;
        case CommandType.Event:
          return HasAccessForEventCommand (securableObject);
        case CommandType.WxeFunction:
          return HasAccessForWxeFunctionCommand ();
        case CommandType.None:
          return true;
        default:
          throw new InvalidOperationException (string.Format ("The CommandType '{0}' is not supported by the '{1}'.", _type, typeof (Command).FullName));
      }
    }

    private bool HasAccessForEventCommand (ISecurableObject securableObject)
    {
      IWebSecurityAdapter webSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWebSecurityAdapter> ();
      if (webSecurityAdapter == null)
        return true;
      return webSecurityAdapter.HasAccess (securableObject, Click);
    }

    private bool HasAccessForWxeFunctionCommand ()
    {
      IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter> ();
      if (wxeSecurityAdapter == null)
        return true;
      return wxeSecurityAdapter.HasStatelessAccess (WxeFunctionCommand.ResolveFunctionType ());
    }
  }

  /// <summary> The possible command types of a <see cref="Command"/>. </summary>
  public enum CommandType
  {
    /// <summary> No command will be generated. </summary>
    None,
    /// <summary> A server side event will be raised upon a command click. </summary>
    Event,
    /// <summary> A hyperlink will be rendered on the page. </summary>
    Href,
    /// <summary> A <see cref="WxeFunction"/> will be called upon a command click. </summary>
    WxeFunction
  }

  /// <summary> Defines when the command will be active on the page. </summary>
  public enum CommandShow
  {
    /// <summary> The command is always active. </summary>
    Always,
    /// <summary> The command is only active if the containing element is read-only. </summary>
    ReadOnly,
    /// <summary> The command is only active if the containing element is in edit-mode. </summary>
    EditMode
  }

  /// <summary>
  ///   Represents the method that handles the <see cref="Command.Click"/> event
  ///   raised when clicking on a <see cref="Command"/> of type <see cref="CommandType.Event"/>.
  /// </summary>
  public delegate void CommandClickEventHandler (object sender, CommandClickEventArgs e);

  /// <summary> Provides data for the <see cref="Remotion.Web.UI.Controls.Command.Click"/> event. </summary>
  public class CommandClickEventArgs : EventArgs
  {
    private Command _command;

    /// <summary> Initializes a new instance. </summary>
    public CommandClickEventArgs (Command command)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      _command = command;
    }

    /// <summary> The <see cref="Command"/> that caused the event. </summary>
    public Command Command
    {
      get { return _command; }
    }
  }

}
