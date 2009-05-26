using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocTextValueBase : IPostBackDataHandler, IFocusableControl, IBusinessObjectBoundEditableWebControl, IUrlResolutionService
  {
    /// <summary>
    ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) 
    ///   and the <see cref="Label"/> (read-only mode).
    /// </summary>
    /// <remarks>
    ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual style settings for
    ///   the respective modes. Note that if you set one of the <b>Font</b> attributes (Bold, Italic etc.) to 
    ///   <see langword="true"/>, this cannot be overridden using <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> 
    ///   properties.
    /// </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    Style CommonStyle { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    TextBoxStyle TextBoxStyle { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the Label (read-only mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    Style LabelStyle { get; }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("Validation message displayed if there is an error.")]
    [Category ("Validator")]
    [DefaultValue ("")]
    string ErrorMessage { get; set; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTextValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    string CssClassBase { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
    /// </remarks>
    string CssClassReadOnly { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    string CssClassDisabled { get; }

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is an empty <see cref="String"/>. </remarks>
    [Description ("The string representation of the current value.")]
    [Category ("Data")]
    [DefaultValue ("")]
    string Text { get; set; }

    /// <summary> Gets or sets a flag that specifies whether the value of the control is required. </summary>
    /// <remarks>
    ///   Set this property to <see langword="null"/> in order to use the default value 
    ///   (see <see cref="ISmartControl.IsRequired"/>).
    /// </remarks>
    [Description ("Explicitly specifies whether the control is required.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    bool? Required { get; set; }

    /// <summary> Gets or sets a flag that specifies whether the control should be displayed in read-only mode. </summary>
    /// <remarks>
    ///   Set this property to <see langword="null"/> in order to use the default value 
    ///   (see <see cref="IBusinessObjectBoundEditableControl.IsReadOnly"/>). Note that if the data source is in read-only mode, the
    ///   control is read-only too, even if this property is set to <c>false</c>.
    /// </remarks>
    [Description ("Explicitly specifies whether the control should be displayed in read-only mode.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    bool? ReadOnly { get; set; }

    /// <summary>Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this <see cref="BusinessObjectBoundWebControl"/>.</summary>
    /// <value> The <see cref="BusinessObjectBinding"/> instance used to manage this control's binding. </value>
    [Browsable (false)]
    BusinessObjectBinding Binding { get; }

    System.Web.UI.AttributeCollection Attributes { get; }
    string CssClass { get; set; }
    CssStyleCollection Style { get; }
    bool Enabled { get; set; }
    Unit Height { get; set; }
    Unit Width { get; set; }
    char ClientIDSeparator { get; }


    /// <summary> This event is fired when the text is changed between postbacks. </summary>
    [Category ("Action")]
    [Description ("Fires when the value of the control has changed.")]
    event EventHandler TextChanged;

    void ApplyStyle (Style s);
    void CopyBaseAttributes (WebControl controlSrc);
    void MergeStyle (Style s);

    bool IsDesignMode { get; }
    Style ControlStyle { get; }
    void BaseAddAttributesToRender (HtmlTextWriter writer);
  }
}