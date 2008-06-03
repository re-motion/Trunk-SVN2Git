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
