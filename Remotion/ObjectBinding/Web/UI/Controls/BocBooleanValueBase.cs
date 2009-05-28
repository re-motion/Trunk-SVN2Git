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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public abstract class BocBooleanValueBase : BusinessObjectBoundEditableWebControl, IBocBooleanValueBase, IPostBackDataHandler, IFocusableControl
  {
    private const string c_defaultControlWidth = "100pt";

    private bool? _autoPostBack = null;
    private string _trueDescription = string.Empty;
    private string _falseDescription = string.Empty;
    private string _nullDescription = string.Empty;

    /// <summary> Flag that determines whether the client script will be rendered. </summary>
    private bool _hasClientScript = false;

    private static readonly object s_checkedChangedEvent = new object();

    protected BocBooleanValueBase ()
    {
    }

    /// <summary> Gets a flag that determines whether changing the checked state causes an automatic postback.</summary>
    /// <value> 
    ///   <see langword="NaBooleanEnum.True"/> to enable automatic postbacks. 
    ///   Defaults to <see langword="null"/>, which is interpreted as 
    ///   <see langword="NaBooleanEnum.False"/>.
    /// </value>
    /// <remarks>
    ///   Use <see cref="IsAutoPostBackEnabled"/> to evaluate this property.
    /// </remarks>
    [Description ("Automatically postback to the server after the checked state is modified. Undefined is interpreted as false.")]
    [Category ("Behavior")]
    [DefaultValue (typeof (bool?), "")]
    [NotifyParentProperty (true)]
    public bool? AutoPostBack
    {
      get { return _autoPostBack; }
      set { _autoPostBack = value; }
    }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="true"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="true"/>. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed when the checkbox is set to True.")]
    [Category ("Appearance")]
    [DefaultValue("")]
    public string TrueDescription
    {
      get { return _trueDescription; }
      set { _trueDescription = value; }
    }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="false"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="false"/>. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed when the checkbox is set to False.")]
    [Category ("Appearance")]
    [DefaultValue("")]
    public string FalseDescription
    {
      get { return _falseDescription; }
      set { _falseDescription = value; }
    }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="null"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed when the checkbox is set to null.")]
    [Category ("Appearance")]
    [DefaultValue("")]
    public string NullDescription
    {
      get { return _nullDescription; }
      set { _nullDescription = value; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocCheckBox</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected abstract string CssClassBase { get; }

    string IBocBooleanValueBase.CssClassReadOnly
    {
      get { return CssClassReadOnly; }
    }

    string IBocBooleanValueBase.CssClassDisabled
    {
      get { return CssClassDisabled; }
    }

    bool IBocBooleanValueBase.HasClientScript
    {
      get { return HasClientScript; }
      set { HasClientScript = value; }
    }

    string IBocBooleanValueBase.CssClassBase
    {
      get { return CssClassBase; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocCheckBox.readOnly</c> as a selector. </para>
    /// </remarks>
    protected virtual string CssClassReadOnly
    { get { return "readOnly"; } }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocCheckBox.disabled</c> as a selector.</para>
    /// </remarks>
    protected virtual string CssClassDisabled
    { get { return "disabled"; } }

    /// <summary> Gets or sets the <see cref="IBusinessObjectBooleanProperty"/> object this control is bound to. </summary>
    /// <value> An instance of type <see cref="IBusinessObjectBooleanProperty"/>. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectBooleanProperty Property
    {
      get { return (IBusinessObjectBooleanProperty) base.Property; }
      set { base.Property = ArgumentUtility.CheckType<IBusinessObjectBooleanProperty> ("value", value); }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="CheckBox"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public abstract string FocusID { get; }

    /// <summary> Flag that determines whether the client script will be rendered. </summary>
    protected bool HasClientScript
    {
      get { return _hasClientScript; }
      set { _hasClientScript = value; }
    }

    /// <summary> Gets or sets the current value. </summary>
    public new abstract bool? Value { get; set; }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = ArgumentUtility.CheckType<bool?> ("value", value); }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="BusinessObjectBoundWebControl.TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary> Gets the evaluated value for the <see cref="AutoPostBack"/> property. </summary>
    /// <value> <see langowrd="true"/> if <see cref="AutoPostBack"/> is <see langword="true"/>. </value>
    protected bool IsAutoPostBackEnabled
    {
      get { return AutoPostBack == true;}
    }

    /// <summary> Invokes the <see cref="BocBooleanValue.LoadPostData(string,System.Collections.Specialized.NameValueCollection)"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (RequiresLoadPostData)
        return LoadPostData (postDataKey, postCollection);
      else
        return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
    ///   between postbacks.
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadPostData/*' />
    protected abstract bool LoadPostData (string postDataKey, NameValueCollection postCollection);

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent()
    {
      if (! IsReadOnly && Enabled)
        OnCheckedChanged();
    }

    /// <summary> Fires the <see cref="CheckedChanged"/> event. </summary>
    protected virtual void OnCheckedChanged ()
    {
      EventHandler eventHandler = (EventHandler) Events[s_checkedChangedEvent];
      if (eventHandler != null)
        eventHandler (this, EventArgs.Empty);
    }

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
      bool isReadOnly = IsReadOnly;
      bool isDisabled = ! Enabled;

      string backUpCssClass = CssClass; // base.CssClass and base.ControlStyle.CssClass
      if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
      {
        if (isReadOnly)
          CssClass += " " + CssClassReadOnly;
        else if (isDisabled)
          CssClass += " " + CssClassDisabled;
      }
      string backUpAttributeCssClass = Attributes["class"];
      if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
      {
        if (isReadOnly)
          Attributes["class"] += " " + CssClassReadOnly;
        else if (isDisabled)
          Attributes["class"] += " " + CssClassDisabled;
      }
    
      base.AddAttributesToRender (writer);

      if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
        CssClass = backUpCssClass;
      if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
        Attributes["class"] = backUpAttributeCssClass;
    
      if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
      {
        string cssClass = CssClassBase;
        if (isReadOnly)
          cssClass += " " + CssClassReadOnly;
        else if (isDisabled)
          cssClass += " " + CssClassDisabled;
        writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
      }

      writer.AddStyleAttribute ("white-space", "nowrap");
      if (! IsReadOnly)
      {
        bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
        bool isLabelWidthEmpty = StringUtility.IsNullOrEmpty (Label.Style["width"]);
        if (isLabelWidthEmpty && isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
      }
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (! interim)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null)
        {
          bool? value = (bool?) DataSource.BusinessObject.GetProperty (Property);
          LoadValueInternal (value, interim);
        }
      }
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (bool? value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (bool? value, bool interim)
    {
      if (! interim)
      {
        Value = value;
        IsDirty = false;
      }
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/SaveValue/*' />
    public override void SaveValue (bool interim)
    {
      if (! interim && IsDirty)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        {
          DataSource.BusinessObject.SetProperty (Property, Value);
          IsDirty = false;
        }
      }
    }

    /// <summary> The <see cref="BocCheckBox"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return ! isList;
    }

    /// <summary> Occurs when the <see cref="Value"/> property changes between postbacks. </summary>
    [Category ("Action")]
    [Description ("Fires when the value of the control has changed.")]
    public event EventHandler CheckedChanged
    {
      add { Events.AddHandler (s_checkedChangedEvent, value); }
      remove { Events.RemoveHandler (s_checkedChangedEvent, value); }
    }

    public abstract Label Label { get; }

    bool IBocBooleanValueBase.IsAutoPostBackEnabled
    {
      get { return IsAutoPostBackEnabled; }
    }

    public virtual new IPage Page
    {
      get { return new PageWrapper (base.Page); }
    }

    bool IBocBooleanValueBase.IsDesignMode
    {
      get { return IsDesignMode; }
    }
  }
}