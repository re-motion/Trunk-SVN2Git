using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocTextValueBase : IBusinessObjectBoundEditableWebControl
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
    Style CommonStyle { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    TextBoxStyle TextBoxStyle { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    Style LabelStyle { get; }

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
    string Text { get; }

    System.Web.UI.AttributeCollection Attributes { get; }
    string CssClass { get; set; }
    CssStyleCollection Style { get; }
    bool Enabled { get; }
    Unit Height { get; set; }
    Unit Width { get; set; }

    void ApplyStyle (Style s);
    void CopyBaseAttributes (WebControl controlSrc);
    void MergeStyle (Style s);

    bool IsDesignMode { get; }
    Style ControlStyle { get; }

    bool AutoPostBack { get; set; }

    void BaseAddAttributesToRender (HtmlTextWriter writer);

    string GetTextBoxUniqueID ();
  }
}