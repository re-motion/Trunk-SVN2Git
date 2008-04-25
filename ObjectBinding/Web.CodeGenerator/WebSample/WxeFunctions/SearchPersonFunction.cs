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
