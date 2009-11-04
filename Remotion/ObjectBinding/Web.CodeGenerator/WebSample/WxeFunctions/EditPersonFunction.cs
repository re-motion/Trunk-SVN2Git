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
