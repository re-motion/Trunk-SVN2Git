// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web;
using System.Web.UI;

namespace Remotion.Web.Utilities
{
public class HtmlUtility
{
  public static string Format (string htmlFormatString, params object[] nonHtmlParameters)
  {
    string[] htmlParameters = new string[nonHtmlParameters.Length];
    for (int i = 0; i < nonHtmlParameters.Length; ++i)
    {
      htmlParameters[i] = HtmlEncode (nonHtmlParameters[i].ToString());
    }
    return string.Format (htmlFormatString, (object[]) htmlParameters);
  }

  public static string HtmlEncode (string nonHtmlString)
  {
    string html = HttpUtility.HtmlEncode (nonHtmlString);
    if (html != null)
    {
      html = html.Replace ("\r\n", "<br />");
      html = html.Replace ("\n", "<br />");
      html = html.Replace ("\r", "<br />");
    }
    return html;
  }

  public static void HtmlEncode (string nonHtmlString, HtmlTextWriter writer)
  {
    writer.Write (HtmlUtility.HtmlEncode (nonHtmlString));
  }

  private HtmlUtility()
  {
  }
}
}
