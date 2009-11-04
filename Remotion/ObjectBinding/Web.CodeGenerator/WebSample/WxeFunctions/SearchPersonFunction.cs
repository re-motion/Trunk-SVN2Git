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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using DomainSample;

namespace WebSample.WxeFunctions
{
	public class SearchPersonFunction : WxeTransactedFunction
	{
		public SearchPersonFunction()
		{
		}

		public SearchPersonFunction(params object[] args) : base(args)
		{
		}

		public SearchPersonFunction(Query query) : this(new object[] { query })
		{
		}

		#region Parameter
		[WxeParameter(1, false, WxeParameterDirection.In)]
		public IQuery Query
		{
			get { return (IQuery)Variables["Query"]; }
			set { Variables["Query"] = value; }
		}
		#endregion

		#region Steps
		private void Step1()
		{
			ExecuteQuery();
		}

		private WxePageStep Step2 = new WxePageStep("UI/SearchResultPersonForm.aspx");
		#endregion

		public DomainObjectCollection Persons
		{
			get { return (DomainObjectCollection)Variables["Persons"]; }
			protected set { Variables["Persons"] = value; }
		}

		public void ExecuteQuery()
		{
			if (Query == null)
				Query = new Query("AllPersons");

			Persons = ClientTransaction.Current.QueryManager.GetCollection(Query);
		}
	}
}
