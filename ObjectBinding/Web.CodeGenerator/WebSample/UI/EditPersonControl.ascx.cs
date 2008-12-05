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
