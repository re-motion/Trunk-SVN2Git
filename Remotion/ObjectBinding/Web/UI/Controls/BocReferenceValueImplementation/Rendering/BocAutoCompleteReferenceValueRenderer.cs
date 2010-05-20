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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocAutoCompleteReferenceValue"/> controls in Standards Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.TextBox"/> and a pop-up element.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocAutoCompleteReferenceValueRenderer : BocReferenceValueRendererBase<IBocAutoCompleteReferenceValue>
  {
    public BocAutoCompleteReferenceValueRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control, IResourceUrlFactory resourceUrlFactory)
      : this (context, control, resourceUrlFactory, () => new TextBox ())
    {
    }

    public BocAutoCompleteReferenceValueRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control, IResourceUrlFactory resourceUrlFactory, Func<TextBox> textBoxFactory)
      : base (context, control, resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("textBoxFactory", textBoxFactory);
      TextBoxFactory = textBoxFactory;
    }

    private Func<TextBox> TextBoxFactory { get; set; }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      RegisterJavaScriptFiles (htmlHeadAppender);
      RegisterStylesheets (htmlHeadAppender);
    }

    private void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      htmlHeadAppender.RegisterJQueryBgiFramesJavaScriptInclude();

      string jqueryAutocompleteScriptKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_JQueryAutoCompleteScript";
      htmlHeadAppender.RegisterJavaScriptInclude (
          jqueryAutocompleteScriptKey,
          ResourceUrlFactory.CreateResourceUrl (
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.js"));

      string scriptKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlFactory.CreateResourceUrl (typeof (BocAutoCompleteReferenceValueRenderer), ResourceType.Html, "BocAutoCompleteReferenceValue.js"));
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_Style";
      htmlHeadAppender.RegisterStylesheetLink (
          styleKey,
          ResourceUrlFactory.CreateThemedResourceUrl (
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.css"),
          HtmlHeadAppender.Priority.Library);

      string jqueryAutocompleteStyleKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_JQueryAutoCompleteStyle";
      htmlHeadAppender.RegisterStylesheetLink (
          jqueryAutocompleteStyleKey,
          ResourceUrlFactory.CreateThemedResourceUrl (
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.css"),
          HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterBindScript();
      RegisterAdjustLayoutScript ();

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (EmbedInOptionsMenu)
        RenderContentsWithIntegratedOptionsMenu (writer);
      else
        RenderContentsWithSeparateOptionsMenu (writer);

      writer.RenderEndTag();
    }

    private void RegisterBindScript ()
    {
      string key = Control.UniqueID + "_BindScript";
      const string scriptTemplate =
          @"$(document).ready( function() {{ BocAutoCompleteReferenceValue.Bind($('#{0}'), $('#{1}'), $('#{2}'), "
          + "'{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}'); }} );";

      var dataSource = Maybe.ForValue (Control.DataSource);
      string businessObjectClass =
          dataSource.Select (ds => ds.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (
              dataSource.Select (ds => ds.BusinessObject).Select (o => o.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (""));
      string businessObjectProperty = Maybe.ForValue (Control.Property).Select (p => p.Identifier).ValueOrDefault ("");
      string businessObjectID =
          dataSource.Select (ds => (IBusinessObjectWithIdentity) ds.BusinessObject).Select (o => o.UniqueIdentifier).ValueOrDefault ("");

      string script = string.Format (
          scriptTemplate,
          Control.TextBoxClientID,
          Control.HiddenFieldClientID,
          Control.DropDownButtonClientID,
          string.IsNullOrEmpty (Control.ServicePath)
              ? ""
              : UrlUtility.GetAbsoluteUrl (Context, Control.ServicePath, true),
          StringUtility.NullToEmpty (Control.ServiceMethod),
          Control.CompletionSetCount.HasValue ? Control.CompletionSetCount.Value : 10,
          Control.CompletionInterval,
          Control.SuggestionInterval,
          Control.NullValueString,
          businessObjectClass,
          businessObjectProperty,
          businessObjectID,
          Control.Args
          );
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (IBocAutoCompleteReferenceValue), key, script);
    }

    private void RegisterAdjustLayoutScript ()
    {
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocAutoCompleteReferenceValueRenderer),
          Guid.NewGuid ().ToString (),
          string.Format ("BocBrowserCompatibility.AdjustAutoCompleteReferenceValueLayout ($('#{0}'));", Control.ClientID));
    }

    protected override void RenderEditModeValueWithSeparateOptionsMenu (HtmlTextWriter writer)
    {
      TextBox textBox = GetTextBox ();
      RenderEditModeValue (writer, textBox);
    }

    protected override void RenderEditModeValueWithIntegratedOptionsMenu (HtmlTextWriter writer)
    {
      TextBox textBox = GetTextBox ();
      textBox.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
      RenderEditModeValue (writer, textBox);
    }

    private void RenderEditModeValue (HtmlTextWriter writer, TextBox textBox)
    {
      RenderEditableControl (writer, textBox);

      RenderEditModeValueExtension (writer);
    }

    protected virtual void RenderEditModeValueExtension (HtmlTextWriter writer)
    {
    }

    protected override Label GetLabel ()
    {
      var label = new Label { ID = Control.TextBoxUniqueID, EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (Control.GetLabelText ());
      return label;
    }

    private void RenderEditableControl (HtmlTextWriter writer, TextBox textBox)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInput);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      textBox.RenderControl (writer);
      writer.RenderEndTag();

      if (Control.Enabled)
        RenderDropdownButton (writer);

      RenderHiddenField (writer);
    }

    private void RenderHiddenField (HtmlTextWriter writer)
    {
      var hiddenField = new HiddenField
                        {
                            ID = Control.HiddenFieldUniqueID,
                            Page = Control.Page.WrappedInstance,
                            EnableViewState = true,
                            Value = Control.BusinessObjectUniqueIdentifier ?? Control.NullValueString
                        };
      hiddenField.RenderControl (writer);
    }

    private void RenderDropdownButton (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.DropDownButtonClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      var imgUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocAutoCompleteReferenceValueRenderer), ResourceType.Image, "DropDownMenuArrow.gif");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundImage, string.Format ("url('{0}')", imgUrl.GetUrl()));
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (writer);
      writer.RenderEndTag();
    }

    private TextBox GetTextBox ()
    {
      var textBox = TextBoxFactory();
      textBox.ID = Control.TextBoxUniqueID;
      textBox.Text = Control.GetLabelText();
      textBox.Enabled = Control.Enabled;
      textBox.EnableViewState = false;
      textBox.Page = Control.Page.WrappedInstance;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textBox);   
      return textBox;
    }

    public override string CssClassBase
    {
      get { return "bocAutoCompleteReferenceValue"; }
    }

    private string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    private string CssClassInput
    {
      get { return "bocAutoCompleteReferenceValueInput"; }
    }
  }
}