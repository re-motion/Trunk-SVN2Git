// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectQuerySearchService : ISearchAvailableObjectsService
  {
    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      var defaultSearchArguments = searchArguments as DefaultSearchArguments;
      if (defaultSearchArguments == null || string.IsNullOrEmpty (defaultSearchArguments.SearchStatement))
        return new IBusinessObject[0];

      QueryDefinition definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions.GetMandatory (defaultSearchArguments.SearchStatement);
      if (definition.QueryType != QueryType.Collection)
        throw new ArgumentException (string.Format ("The query '{0}' is not a collection query.", defaultSearchArguments.SearchStatement));

      var referencingDomainObject = referencingObject as DomainObject;

      var clientTransaction = referencingDomainObject != null ? DomainObjectCheckUtility.GetNonNullClientTransaction (referencingDomainObject) : ClientTransaction.Current;
      if (clientTransaction == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread or the referencing object.");

      var result = clientTransaction.QueryManager.GetCollection (QueryFactory.CreateQuery (definition));
      var availableObjects = new IBusinessObjectWithIdentity[result.Count];

      if (availableObjects.Length > 0)
        result.ToArray().CopyTo (availableObjects, 0);

      return availableObjects;
    }
  }
}