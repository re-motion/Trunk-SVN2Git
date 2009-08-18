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
using System.Linq;
using System.Text;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Utilities
{
  /// <summary> Utility class for client-side scripts. </summary>
  public class ScriptUtility : IScriptUtility
  {
    #region Obsolete

    /// <summary>
    ///   Used to register a client javascript script to be rendered  at the beginning of the HTML page.
    ///   The script is automatically surrounded by &lt;script&gt; tags.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="Control"/> which the script file will be registered. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="key"> 
    ///   The key identifying the registered script file. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="javascript"> 
    ///   The client script that will be registered. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <seealso cref="Page.RegisterClientScriptBlock"/>
    [Obsolete ("Use IPage.ClientScript.RegisterClientScriptBlock (IControl, Type, string, string) instead.")]
    public static void RegisterClientScriptBlock (Control control, string key, string javascript)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("javascript", javascript);

      if (!string.IsNullOrEmpty (javascript))
        javascript += "\r\n";

      ScriptManager.RegisterClientScriptBlock (control, typeof (Page), key, javascript, true);
    }

    /// <summary>
    ///   Used to register a client javascript script to be rendered at the end of the HTML page. 
    ///   The script is automatically surrounded by &lt;script&gt; tags.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="Control"/> for which the script file will be registered. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="key"> 
    ///   The key identifying the registered script block. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="javascript"> 
    ///   The client script that will be registered. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <seealso cref="ScriptManager.RegisterStartupScript"/>
    [Obsolete ("Use IPage.ClientScript.RegisterStartupScriptBlock (IControl, Type, string, string) instead.")]
    public static void RegisterStartupScriptBlock (Control control, string key, string javascript)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("javascript", javascript);

      if (!string.IsNullOrEmpty (javascript))
        javascript += "\r\n";

      ScriptManager.RegisterStartupScript (control, typeof (Page), key, javascript, true);
    }

    [Obsolete ("Use ScriptUtility.Instance.RegisterElementForBorderSpans", true)]
    public static void RegisterElementForBorderSpans ()
    {
      throw new NotImplementedException ("Use ScriptUtility.Instance.RegisterElementForBorderSpans");
    }

    /// <summary>
    /// Gets a flag that informs the caller if the <paramref name="control"/> will be part of the rendered output. This method only works during the
    /// Render cycle.
    /// </summary>
    [Obsolete ("The various methods for registering scripts now accept controls instead of the page, thus allowing filtering of the output by the surrounding UpdatePanel.")]
    public static bool IsPartOfRenderedOutput (Control control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      var scriptManager = ScriptManager.GetCurrent (control.Page);
      if (scriptManager != null && scriptManager.IsInAsyncPostBack)
      {
        bool isInsidePartialRenderingUpdatePanel = control.CreateSequence (c => c.Parent)
            .Where (c => c is UpdatePanel && ((UpdatePanel) c).IsInPartialRendering)
            .Cast<UpdatePanel> ()
            .Any ();

        return isInsidePartialRenderingUpdatePanel;
      }
      else
      {
        return true;
      }
    }

    #endregion

    public enum Event
    {
      Resize
    }

    public static IScriptUtility Instance
    {
      get { return ServiceLocator.Current.GetInstance<IScriptUtility>(); }
    }

    /// <summary> Escapes special characters (e.g. <c>\n</c>) in the passed string. </summary>
    /// <param name="input"> The unescaped string. Must not be <see langword="null"/>. </param>
    /// <returns> The string with special characters escaped. </returns>
    /// <remarks>
    ///   This is required when adding client script to the page containing special characters. ASP.NET automatically 
    ///   escapes client scripts created by <see cref="Page.GetPostBackEventReference">Page.GetPostBackEventReference</see>.
    /// </remarks>
    public static string EscapeClientScript (string input)
    {
      ArgumentUtility.CheckNotNull ("input", input);

      StringBuilder output = new StringBuilder (input.Length + 5);
      for (int idxChars = 0; idxChars < input.Length; idxChars++)
      {
        char c = input[idxChars];
        switch (c)
        {
          case '\t':
            {
              output.Append (@"\t");
              break;
            }
          case '\n':
            {
              output.Append (@"\n");
              break;
            }
          case '\r':
            {
              output.Append (@"\r");
              break;
            }
          case '"':
            {
              output.Append ("\\\"");
              break;
            }
          case '\'':
            {
              output.Append (@"\'");
              break;
            }
          case '\\':
            {
              if (idxChars > 0 && idxChars + 1 < input.Length)
              {
                char prevChar = input[idxChars - 1];
                char nextChar = input[idxChars + 1];
                if (prevChar == '<' && nextChar == '/')
                {
                  output.Append (c);
                  break;
                }
              }
              output.Append (@"\\");
              break;
            }
          case '\v':
            {
              output.Append (c);
              break;
            }
          case '\f':
            {
              output.Append (c);
              break;
            }
          default:
            {
              output.Append (c);
              break;
            }
        }
      }
      return output.ToString ();
    }

    public ScriptUtility ()
    {
    }

    public void RegisterElementForBorderSpans (HtmlHeadAppender htmlHeadAppender, IControl control, string jQuerySelectorForBorderSpanTarget)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      ArgumentUtility.CheckNotNullAndType<Control> ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("jQuerySelectorForBorderSpanTarget", jQuerySelectorForBorderSpanTarget);

      string key = typeof (ScriptUtility).FullName + "_StyleUtility";
      string url = ResourceUrlResolver.GetResourceUrl (control, typeof (ScriptUtility), ResourceType.Html, ResourceTheme, "StyleUtility.js");

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (control);
      htmlHeadAppender.RegisterJavaScriptInclude (key, url);

      control.Page.ClientScript.RegisterStartupScriptBlock (
          control,
          typeof (Page),
          "BorderSpans_" + jQuerySelectorForBorderSpanTarget,
          string.Format (
              "StyleUtility.CreateBorderSpans ('{0}');", jQuerySelectorForBorderSpanTarget));
    }

    protected ResourceTheme ResourceTheme
    {
      get { return ServiceLocator.Current.GetInstance<ResourceTheme> (); }
    }

    public void RegisterResizeOnElement (HtmlHeadAppender htmlHeadAppender, IControl control, string jquerySelector, string eventHandler)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      ArgumentUtility.CheckNotNullAndType<Control> ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("jquerySelector", jquerySelector);
      ArgumentUtility.CheckNotNullOrEmpty ("eventHandler", eventHandler);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (control);

      string key = control.ClientID + "_ResizeEventHandler";
      string script = string.Format ("PageUtility.Instance.RegisterResizeHandler({0}, {1});", jquerySelector, eventHandler);
      control.Page.ClientScript.RegisterStartupScriptBlock (control, typeof (ScriptUtility), key, script);
    }
  }
}
