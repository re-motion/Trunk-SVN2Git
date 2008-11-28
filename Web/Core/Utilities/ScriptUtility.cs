/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Utilities
{
  /// <summary> Utility class for client-side scripts. </summary>
  public static class ScriptUtility
  {
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

    /// <summary>
    ///   Used to register a client javascript script to be rendered  at the beginning of the HTML page.
    ///   The script is automatically surrounded by &lt;script&gt; tags.
    /// </summary>
    /// <param name="page"> 
    ///   The <see cref="Page"/> where the script file will be registered. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="key"> 
    ///   The key identifying the registered script file. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="javascript"> 
    ///   The client script that will be registered. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <seealso cref="Page.RegisterClientScriptBlock"/>
    public static void RegisterClientScriptBlock (Page page, string key, string javascript)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("javascript", javascript);

      if (!string.IsNullOrEmpty (javascript))
        javascript += "\r\n";
      
      ScriptManager.RegisterClientScriptBlock (page, typeof (Page), key, javascript, true);
    }

    /// <summary>
    ///   Used to register a client javascript script to be rendered at the end of the HTML page. 
    ///   The script is automatically surrounded by &lt;script&gt; tags.
    /// </summary>
    /// <param name="page"> 
    ///   The <see cref="Page"/> where the script file will be registered. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="key"> 
    ///   The key identifying the registered script block. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="javascript"> 
    ///   The client script that will be registered. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <seealso cref="Page.RegisterStartupScript"/>
    public static void RegisterStartupScriptBlock (Page page, string key, string javascript)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("javascript", javascript);

      if (!string.IsNullOrEmpty (javascript))
        javascript += "\r\n";
      
      ScriptManager.RegisterStartupScript (page, typeof (Page), key, javascript, true);
    }

    public static void RegisterElementForBorderSpans (Page page, string elementID)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNullOrEmpty ("elementID", elementID);

      ScriptUtility.RegisterStartupScriptBlock (
         page, "BorderSpans_" + elementID, string.Format ("StyleUtility.CreateBorderSpans (document.getElementById ('{0}'));", elementID));
    }

    /// <summary>
    /// Gets a flag that informs the caller if the <paramref name="control"/> will be part of the rendered output.
    /// </summary>
    public static bool IsPartOfRenderedOutput (Control control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      var scriptManager = ScriptManager.GetCurrent (control.Page);
      if (scriptManager != null && scriptManager.IsInAsyncPostBack)
      {
        bool isInsidePartialRenderingUpdatePanel = GetThisAndParents (control)
          .Where (c => c is UpdatePanel && ((UpdatePanel)c).IsInPartialRendering)
          .Cast<UpdatePanel>()
          .Any();

        return isInsidePartialRenderingUpdatePanel;
      }
      else
      {
        return true;
      }
    }

    //TODO MK: Move to Linq-Extensions
    private static IEnumerable<Control> GetThisAndParents (Control control)
    {
      for (var current = control; current != null; current = current.Parent)
        yield return current;
    }
  }
}
