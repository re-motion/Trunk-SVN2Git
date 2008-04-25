using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interfaces declares advanced properties and methods for data-aware web controls.
///   <seealso cref="Remotion.Web.UI.Controls.FormGridManager"/>
/// </summary>
public interface ISmartControl: IControl
{
  /// <summary>
  ///   Specifies whether the control must be filled out by the user before submitting the form.
  /// </summary>
  bool IsRequired { get; }

  /// <summary>
  ///   Specifies the relative URL to the row's help text.
  /// </summary>
  string HelpUrl { get; }

  /// <summary>
  ///   Creates an appropriate validator for this control.
  /// </summary>
  BaseValidator[] CreateValidators(); 

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its ClientID.
  /// </summary>
  /// <remarks>
  ///   For compound controls that accept user input in text boxes, lists etc., this is the control that
  ///   actually accepts user input. For all other controls, this is the control itself.
  /// </remarks>
  Control TargetControl { get; }

  /// <summary>
  ///   If UseLabel is true, it is valid to generate HTML &lt;label&gt; tags referencing <see cref="TargetControl"/>.
  /// </summary>
  /// <remarks>
  ///   This flag is usually true, except for controls that render combo boxes or other HTML tags that do not function properly
  ///   with labels. This flag has been introduced due to a bug in Microsoft Internet Explorer.
  /// </remarks>
  bool UseLabel { get; }

//  /// <summary>
//  ///   If UseInputControlCSS is true, the control requires special formatting.
//  /// </summary>
//  /// <remarks>
//  ///   This flag should be true for controls rendering &lt;input&gt; or &lt;textarea&gt; elements.
//  ///   The reason for this is in excentric application of CSS-classes to these elements via
//  ///   the definition of global styles (input {...} and textarea {...}). The most predictable result
//  ///   is acchivied by directly assigning the class instead of using the global definitions.
//  /// </remarks>
//  bool UseInputControlCSS { get; }

  /// <summary> Gets the text to be written into the label for this control. </summary>
  string DisplayName { get; }

  /// <summary>Regsiteres stylesheet and script files with the <see cref="HtmlHeadAppender"/>.</summary>
  void RegisterHtmlHeadContents (HttpContext context);
}

}
