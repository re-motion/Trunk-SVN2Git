using System;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using DomainSample;

namespace WebSample.WxeFunctions
{
	public class EditPersonFunction : EditFunction
	{
		public EditPersonFunction()
		{
		}

		public EditPersonFunction(params object[] args) : base(args)
		{
		}

		#region Parameter
		[WxeParameter(1, false, WxeParameterDirection.In)]
		public string PersonID
		{
			set { Variables["PersonID"] = value; }
		}
		#endregion

		#region Steps
		private void Step1 (WxeContext context)
		{
			if (Person == null)
			{
				Person = new DomainSample.Person ();

				if (StringUtility.IsNullOrEmpty (ReturnUrl))
					ReturnUrl = context.HttpContext.Request.RawUrl;
			}
		}

		private WxeStep Step2 = new WxePageStep ("UI/EditPersonForm.aspx");
		#endregion

		public Person Person
		{
			set { Variables["Person"] = value; }

			get
			{
				EnsureBusinessObject ("Person", "PersonID");
				return (Person) Variables["Person"];
			}
		}
	}
}
