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
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Represents an <c>import url(...)</c> rule for css inlcudes.
  /// </summary>
  public class StyleSheetImportRule : HtmlHeadElement
  {
    private readonly IResourceUrl _resourceUrl;

    public StyleSheetImportRule (IResourceUrl resourceUrl)
    {
      ArgumentUtility.CheckNotNull ("resourceUrl", resourceUrl);
      _resourceUrl = resourceUrl;
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.WriteLine (string.Format ("@import url(\"{0}\");", _resourceUrl.GetUrl()));
    }
  }

  public class StyleSheetLink : HtmlHeadElement
  {
    private readonly IResourceUrl _resourceUrl;

    public StyleSheetLink (IResourceUrl resourceUrl)
    {
      ArgumentUtility.CheckNotNull ("resourceUrl", resourceUrl);
      _resourceUrl = resourceUrl;
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.AddAttribute (HtmlTextWriterAttribute.Type, "text/css");
      writer.AddAttribute (HtmlTextWriterAttribute.Rel, "StyleSheet");
      writer.AddAttribute (HtmlTextWriterAttribute.Href, _resourceUrl.GetUrl ());
      writer.RenderBeginTag (HtmlTextWriterTag.Link);
      writer.RenderEndTag();
      writer.WriteLine ();
    }
  }
}