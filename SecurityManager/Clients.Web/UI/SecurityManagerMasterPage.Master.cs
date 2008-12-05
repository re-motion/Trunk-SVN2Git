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
using System.Web.UI;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public partial class SecurityManagerMasterPage : MasterPage
  {
    private const string c_contentViewStyleFileUrl = "ContentViewStyle.css";
    private const string c_contentViewStyleFileKey = "SecurityManagerContentViewStyle";

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      if (!HtmlHeadAppender.Current.IsRegistered (c_contentViewStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (
            this, typeof (SecurityManagerMasterPage), ResourceType.Html, c_contentViewStyleFileUrl);
        HtmlHeadAppender.Current.RegisterStylesheetLink (c_contentViewStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}
