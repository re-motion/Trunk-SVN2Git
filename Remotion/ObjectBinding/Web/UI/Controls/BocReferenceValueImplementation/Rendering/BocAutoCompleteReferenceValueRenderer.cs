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
using System.Text;
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
    private readonly Func<TextBox> _textBoxFactory;

    public BocAutoCompleteReferenceValueRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control, IResourceUrlFactory resourceUrlFactory)
      : this (context, control, resourceUrlFactory, () => new TextBox ())
    {
    }

    public BocAutoCompleteReferenceValueRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control, IResourceUrlFactory resourceUrlFactory, Func<TextBox> textBoxFactory)
      : base (context, control, resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("textBoxFactory", textBoxFactory);
      _textBoxFactory = textBoxFactory;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      RegisterJavaScriptFiles (htmlHeadAppender);
      RegisterStylesheets (htmlHeadAppender);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      RegisterBindScript();

      base.Render (writer);
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

    private void RegisterBindScript ()
    {
      string key = Control.UniqueID + "_BindScript";

      var dataSource = Maybe.ForValue (Control.DataSource);
      string businessObjectClass =
          dataSource.Select (ds => ds.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (
              dataSource.Select (ds => ds.BusinessObject).Select (o => o.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault (""));
      string businessObjectProperty = Maybe.ForValue (Control.Property).Select (p => p.Identifier).ValueOrDefault ("");
      string businessObjectID =
          dataSource.Select (ds => (IBusinessObjectWithIdentity) ds.BusinessObject).Select (o => o.UniqueIdentifier).ValueOrDefault ("");

      var script = new StringBuilder (1000);
      script.Append ("$(document).ready( function() { BocAutoCompleteReferenceValue.Bind(");
      script.AppendFormat ("$('#{0}'), ", Control.TextBoxClientID);
      script.AppendFormat ("$('#{0}'), ", Control.HiddenFieldClientID);
      script.AppendFormat ("$('#{0}'),", Control.DropDownButtonClientID);
      
      script.AppendFormat (
          "'{0}', ",
          string.IsNullOrEmpty (Control.ServicePath) ? "" : UrlUtility.GetAbsoluteUrl ((Page) Control.Page, Control.ServicePath, false));
      script.AppendFormat ("'{0}', ", StringUtility.NullToEmpty (Control.ServiceMethod));

      script.AppendFormat ("{0}, ", Control.CompletionSetCount);
      script.AppendFormat ("{0}, ", Control.DropDownDisplayDelay);
      script.AppendFormat ("{0}, ", Control.DropDownRefreshDelay);
      script.AppendFormat ("{0}, ", Control.SelectionUpdateDelay);

      script.AppendFormat ("'{0}', ", Control.NullValueString);
      script.AppendFormat ("'{0}', ", businessObjectClass);
      script.AppendFormat ("'{0}', ", businessObjectProperty);
      script.AppendFormat ("'{0}', ", businessObjectID);
      script.AppendFormat ("'{0}'", Control.Args);
      script.Append ("); } );");
     
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (IBocAutoCompleteReferenceValue), key, script.ToString());
    }

    protected override sealed void RenderEditModeValueWithSeparateOptionsMenu (HtmlTextWriter writer)
    {
      TextBox textBox = GetTextBox ();
      RenderEditModeValue (writer, textBox);
    }

    protected override sealed void RenderEditModeValueWithIntegratedOptionsMenu (HtmlTextWriter writer)
    {
      TextBox textBox = GetTextBox ();
      textBox.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
      RenderEditModeValue (writer, textBox);
    }

    private void RenderEditModeValue (HtmlTextWriter writer, TextBox textBox)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInput);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool autoPostBack = textBox.AutoPostBack;
      textBox.AutoPostBack = false;
      textBox.RenderControl (writer);
      textBox.AutoPostBack = autoPostBack;
      writer.RenderEndTag();

      if (Control.Enabled)
        RenderDropdownButton (writer);

      var hiddenField = GetHiddenField();
      if (autoPostBack)
      {
        PostBackOptions options = new PostBackOptions (textBox, string.Empty);
        if (textBox.CausesValidation)
        {
            options.PerformValidation = true;
            options.ValidationGroup = textBox.ValidationGroup;
        }
        if (Control.Page.Form != null)
            options.AutoPostBack = true;
        var postBackEventReference = Control.Page.ClientScript.GetPostBackEventReference (options, true);
        writer.AddAttribute (HtmlTextWriterAttribute.Onchange, postBackEventReference);
      }
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

    private HiddenField GetHiddenField ()
    {
      return new HiddenField
      {
        ID = Control.HiddenFieldUniqueID,
        Page = Control.Page.WrappedInstance,
        EnableViewState = true,
        Value = Control.BusinessObjectUniqueIdentifier ?? Control.NullValueString        
      };
    }

    private TextBox GetTextBox ()
    {
      var textBox = _textBoxFactory();
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