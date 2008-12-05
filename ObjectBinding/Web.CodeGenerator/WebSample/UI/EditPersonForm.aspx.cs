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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using WebSample;
using WebSample.Classes;
using WebSample.WxeFunctions;

namespace WebSample.UI
{
	public partial class EditPersonForm : BasePage
	{
		private bool _isSaved;

		protected void Page_Load(object sender, EventArgs e)
		{
			CurrentObject.BusinessObject = Function.Person;
			CurrentObject.LoadValues(IsPostBack);

			foreach (DataEditUserControl control in DataEditUserControls)
			{
				control.DataSource.BusinessObject = Function.Person;
				control.LoadValues(IsPostBack);
			}
		}

		protected EditPersonFunction Function
		{
			get { return (EditPersonFunction)CurrentFunction; }
		}

		private List<DataEditUserControl> DataEditUserControls
		{
			get
			{
				List<DataEditUserControl> list = new List<DataEditUserControl>();

				foreach (System.Web.UI.Control control in EditPersonView.Controls)
				{
					DataEditUserControl dataEditUserControl = control as DataEditUserControl;

					if (dataEditUserControl != null)
						list.Add(dataEditUserControl);
				}

				return list;
			}
		}

		protected void SaveButton_Click(object sender, EventArgs e)
		{
			EnsurePostLoadInvoked();
			EnsureValidatableControlsInitialized();

			bool isValid = CurrentObject.Validate();

			foreach (DataEditUserControl control in DataEditUserControls)
				isValid &= control.Validate();

			if (isValid)
			{
				foreach (DataEditUserControl control in DataEditUserControls)
					control.SaveValues(false);

				CurrentObject.SaveValues(false);
				ClientTransaction.Current.Commit();
				_isSaved = true;

				this.ExecuteNextStep();
			}
		}

		protected void CancelButton_Click(object sender, EventArgs e)
		{
			this.ExecuteNextStep();
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);

			if (!_isSaved)
			{
				foreach (DataEditUserControl control in DataEditUserControls)
					control.SaveValues(true);

				CurrentObject.SaveValues(true);
			}
		}
	}
}
