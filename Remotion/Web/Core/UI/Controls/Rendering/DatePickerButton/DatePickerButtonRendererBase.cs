// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.DatePickerButton
{
  public abstract class DatePickerButtonRendererBase : RendererBase<IDatePickerButton>, IDatePickerButtonRenderer
  {
    private const string c_datePickerPopupForm = "DatePickerForm.aspx";
    private const int c_defaultDatePickerLengthInPoints = 150;

    protected DatePickerButtonRendererBase (IHttpContext context, HtmlTextWriter writer, IDatePickerButton control)
        : base(context, writer, control)
    {
    }

    public string CssClassBase
    {
      get { return "DatePickerButton"; }
    }

    public string CssClassDisabled
    {
      get { return "disabled"; }
    }

    public string CssClassReadOnly
    {
      get { throw new NotSupportedException (); }
    }

    protected virtual string ImageFileName
    {
      get { return "DatePicker.gif"; }
    }

    /// <summary>
    /// Renders a click-enabled image that shows a <see cref="DatePickerPage"/> on click, which puts the selected value
    /// into the control specified by <see cref="P:Control.TargetControlID"/>.
    /// </summary>
    public void Render ()
    {
      bool hasClientScript = DetermineClientScriptLevel ();
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      
      string cssClass = CssClassBase;
      if (!Control.Enabled)
        cssClass += " " + CssClassDisabled;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      
      // TODO: hyperLink.ApplyStyle (Control.DatePickerButtonStyle);

      bool canScript = (Control.EnableClientScript && Control.IsDesignMode) || hasClientScript;
      if (canScript)
      {
        string script = GetClickScript(hasClientScript);

        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
        Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      }
      if (!Control.Enabled)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");
      }

      Writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (canScript)
      {
        string imageUrl = GetResolvedImageUrl();
        if (imageUrl == null)
          imageUrl = ImageFileName;

        Writer.AddAttribute (HtmlTextWriterAttribute.Src, imageUrl);
        Writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty(Control.AlternateText));
        Writer.RenderBeginTag (HtmlTextWriterTag.Img);
        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag();
    }

    public string GetDatePickerUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (Control.Parent, Context, typeof (DatePickerPage), ResourceType.UI, c_datePickerPopupForm);
    }

    public string GetResolvedImageUrl ()
    {
      return ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (Controls.DatePickerButton), ResourceType.Image, ImageFileName);
    }

    protected abstract bool DetermineClientScriptLevel ();

    private string GetClickScript (bool hasClientScript)
    {
      string script;
      if (hasClientScript && Control.Enabled)
      {
        const string pickerActionButton = "this";
        
        string pickerActionContainer = "document.getElementById ('" + Control.ContainerControlID.Replace('$', '_') + "')";
        string pickerActionTarget = "document.getElementById ('" + Control.TargetControlID.Replace ('$', '_') + "')";

        string pickerUrl = "'" + GetDatePickerUrl() + "'";

        Unit popUpWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerWidth = "'" + popUpWidth + "'";

        Unit popUpHeight = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerHeight = "'" + popUpHeight + "'";

        script = "DatePicker_ShowDatePicker("
                 + pickerActionButton + ", "
                 + pickerActionContainer + ", "
                 + pickerActionTarget + ", "
                 + pickerUrl + ", "
                 + pickerWidth + ", "
                 + pickerHeight + ");"
                 + "return false;";
      }
      else
        script = "return false;";
      return script;
    }
  }
}