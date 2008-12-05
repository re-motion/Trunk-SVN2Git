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
using System.Threading;
using System.Globalization;
using Remotion.Globalization;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;

namespace WebSample.Classes
{
	[WebMultiLingualResources("WebSample.Globalization.Global")]
	public class BasePage : WxePage, IObjectWithResources
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Request.UserLanguages.Length > 0)
			{
				try
				{
					string[] cultureInfo = Request.UserLanguages[0].Split(';');

					if (cultureInfo.Length > 0)
					{
						Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo[0]);
						Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo[0]);
					}
				}
				catch (ArgumentException)
				{
					// if cultureInfo contains a invalid value we just ignore it
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			// register CSS
			string url = Remotion.Web.ResourceUrlResolver.GetResourceUrl(
					this, typeof(Remotion.Web.ResourceUrlResolver), Remotion.Web.ResourceType.Html, "style.css");

			Remotion.Web.UI.HtmlHeadAppender.Current.RegisterStylesheetLink(GetType() + "style", url);

			// globalization
			IResourceManager rm = ResourceManagerUtility.GetResourceManager(this);

			if (rm != null)
				ResourceDispatcher.Dispatch(this, rm);

			base.OnPreRender(e);
		}

		IResourceManager IObjectWithResources.GetResourceManager()
		{
			return this.GetResourceManager();
		}

		protected virtual IResourceManager GetResourceManager()
		{
			Type type = this.GetType();

			if (MultiLingualResourcesAttribute.ExistsResource(type))
				return MultiLingualResourcesAttribute.GetResourceManager(type, true);
			else
				return null;
		}
	}
}
