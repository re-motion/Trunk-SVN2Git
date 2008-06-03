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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using DomainSample;
using WebSample.Classes;

namespace WebSample.UI
{
	public partial class EditPersonControl : BaseControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		public override IBusinessObjectDataSourceControl DataSource
		{
			get { return CurrentObject; }
		}

		protected void PhoneNumbersField_MenuItemClick(object sender, Remotion.Web.UI.Controls.WebMenuItemClickEventArgs e)
		{
			if (e.Item.ItemID == "AddMenuItem")
			{
				PhoneNumber phoneNumber = new PhoneNumber();
				PhoneNumbersField.AddAndEditRow(phoneNumber);
			}
		}
	}
}
