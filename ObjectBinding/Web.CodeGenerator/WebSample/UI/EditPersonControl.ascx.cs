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
