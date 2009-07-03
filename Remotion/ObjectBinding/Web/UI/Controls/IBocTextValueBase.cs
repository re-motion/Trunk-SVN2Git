using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocTextValueBase : IBusinessObjectBoundEditableWebControl, IBocRenderableControl
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

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is an empty <see cref="String"/>. </remarks>
    string Text { get; }

    void ApplyStyle (Style s);
    void CopyBaseAttributes (WebControl controlSrc);
    void MergeStyle (Style s);

    string GetTextBoxClientID ();
  }
}