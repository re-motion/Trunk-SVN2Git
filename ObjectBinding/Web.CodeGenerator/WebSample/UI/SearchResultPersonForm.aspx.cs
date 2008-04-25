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
