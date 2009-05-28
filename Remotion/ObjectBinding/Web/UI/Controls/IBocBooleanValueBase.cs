using System.ComponentModel;
using System.Web.UI.WebControls;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocBooleanValueBase : IBusinessObjectBoundEditableWebControl
  {
    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="true"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="true"/>. The default value is an empty <see cref="string"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("The description displayed when the checkbox is set to True.")]
    [Category ("Appearance")]
    [DefaultValue ("")]
    string TrueDescription { get; set; }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="false"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="false"/>. The default value is an empty <see cref="string"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("The description displayed when the checkbox is set to False.")]
    [Category ("Appearance")]
    [DefaultValue ("")]
    string FalseDescription { get; set; }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="null"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="string"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("The description displayed when the checkbox is set to null.")]
    [Category ("Appearance")]
    [DefaultValue ("")]
    string NullDescription { get; set; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocCheckBox</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    string CssClassBase { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocCheckBox.readOnly</c> as a selector. </para>
    /// </remarks>
    string CssClassReadOnly { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocCheckBox.disabled</c> as a selector.</para>
    /// </remarks>
    string CssClassDisabled { get; }

    /// <summary> Flag that determines whether the client script will be rendered. </summary>
    bool HasClientScript { get; set; }

    /// <summary> Gets or sets the current value. </summary>
    new bool? Value { get; set; }

    bool IsAutoPostBackEnabled { get; }

    new IPage Page { get; }

    /// <summary> Evalutes whether this control is in <b>Design Mode</b>. </summary>
    [Browsable (false)]
    bool IsDesignMode { get; }

    bool Enabled { get; }
  }
}