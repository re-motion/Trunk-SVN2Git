using System;
using Remotion.Utilities;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;

namespace WebSample.WxeFunctions
{
	// This class does not have the name "Edit Base Function" otherwise UIGen will have problems with a domain class named "Base"
	public class EditFunction : WxeTransactedFunction
	{
		public EditFunction()
		{
		}

		public EditFunction(params object[] args) : base(args)
		{
		}

		protected void EnsureBusinessObject(string objectVariable, string objectIDVariable)
		{
			DomainObject domainObject = (DomainObject)Variables[objectVariable];

			if (domainObject == null)
			{
				string objectID = (string)Variables[objectIDVariable];

				if (! StringUtility.IsNullOrEmpty(objectID))
				{
					domainObject = GetObject(objectID);
					Variables[objectVariable] = domainObject;
				}
			}
		}

		private DomainObject GetObject(string objectID)
		{
			ArgumentUtility.CheckNotNullOrEmpty("objectID", objectID);
			ObjectID id = ObjectID.Parse(objectID);
			DomainObject domainObject = ClientTransaction.Current.GetObject(id);

			if (domainObject == null)
				throw new ArgumentException("Wrong ObjectID", "objectID");

			return domainObject;
		}
	}
}
