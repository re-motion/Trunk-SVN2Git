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
