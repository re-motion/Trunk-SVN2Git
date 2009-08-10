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
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue
{
  public abstract class BocAutoCompleteReferenceValuePreRendererBase : PreRendererBase<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValuePreRenderer
  {
    protected BocAutoCompleteReferenceValuePreRendererBase (IHttpContext context, IBocAutoCompleteReferenceValue control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      RegisterJavaScriptFiles(htmlHeadAppender);
      RegisterStylesheets(htmlHeadAppender);
    }

    private void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      htmlHeadAppender.RegisterJQueryBgiFramesJavaScriptInclude (Control);

      string jqueryAutocompleteScriptKey = typeof (IBocAutoCompleteReferenceValue).FullName + "_JQueryAutoCompleteScript";
      htmlHeadAppender.RegisterJavaScriptInclude (
          jqueryAutocompleteScriptKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (IBocAutoCompleteReferenceValue),
              ResourceType.Html,
              ComponentScriptFileName));

      string scriptKey = typeof (IBocAutoCompleteReferenceValue).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlResolver.GetResourceUrl (
              Control,
              Context,
              typeof (IBocAutoCompleteReferenceValue),
              ResourceType.Html,
              BindScriptFileName));
    }

    protected abstract void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender);

    protected abstract string BindScriptFileName { get; }

    protected abstract string ComponentScriptFileName { get; }

    public override void PreRender ()
    {
      string key = Control.UniqueID + "_BindScript";
      const string scriptTemplate =
          @"$(document).ready( function(){{ BocAutoCompleteReferenceValue.Bind($('#{0}'), $('#{1}'), $('#{2}'), " 
          + "'{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}'); }} );";

      string businessObjectClass = Control.DataSource!=null ? Control.DataSource.BusinessObjectClass.Identifier : "";
      string businessObjectProperty = Control.Property!=null ? Control.Property.Identifier : "";
      string businessObjectId = Control.DataSource!=null ? ((IBusinessObjectWithIdentity)Control.DataSource.BusinessObject).UniqueIdentifier : "";
      string script = string.Format (
          scriptTemplate,
          Control.TextBoxClientID,
          Control.HiddenFieldClientID,
          Control.DropDownButtonClientID,
          string.IsNullOrEmpty (Control.ServicePath)
              ? ""
              : UrlUtility.GetAbsoluteUrl (Context, Control.ServicePath, true),
          StringUtility.NullToEmpty(Control.ServiceMethod),
          Control.CompletionSetCount.HasValue ? Control.CompletionSetCount.Value : 10,
          Control.CompletionInterval,
          Control.SuggestionInterval,
          Control.NullValueString,
          businessObjectClass,
          businessObjectProperty,
          businessObjectId,
          ""
          );
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (IBocAutoCompleteReferenceValue), key, script);

      key = Control.ClientID + "_AdjustPositionScript";
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocAutoCompleteReferenceValuePreRendererBase),
          key,
          string.Format ("BocAutoCompleteReferenceValue.AdjustPosition($('#{0}'), {1});",
//              @"$(document).ready( function(){{ 
//  $(window).bind('resize', function(e){{ 
//    BocAutoCompleteReferenceValue.AdjustPosition($('#{0}'), {1}) 
//  }});
//" + "setTimeout('BocAutoCompleteReferenceValue.AdjustPosition($(\"#{0}\"), {1});', 10);" + @"
//}});",
              Control.ClientID,
              Control.EmbedInOptionsMenu ? "true" : "false"));
    }
  }
}