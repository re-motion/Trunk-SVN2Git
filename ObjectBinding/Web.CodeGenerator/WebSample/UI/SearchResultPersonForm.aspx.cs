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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using WebSample;
using WebSample.Classes;
using WebSample.WxeFunctions;

namespace WebSample.UI
{
	public partial class SearchResultPersonForm : BasePage
	{
		private SearchPersonFunction SearchPersonFunction
		{
			get { return (SearchPersonFunction)CurrentFunction; }
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			PersonList.LoadUnboundValue(SearchPersonFunction.Persons, IsPostBack);
		}
	}
}
