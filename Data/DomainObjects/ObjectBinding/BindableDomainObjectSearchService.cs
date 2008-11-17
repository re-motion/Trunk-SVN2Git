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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectSearchService : ISearchAvailableObjectsService
  {
    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      DefaultSearchArguments defaultSearchArguments = searchArguments as DefaultSearchArguments;
      if (defaultSearchArguments == null || string.IsNullOrEmpty (defaultSearchArguments.SearchStatement))
        return new IBusinessObjectWithIdentity[] { };

      QueryDefinition definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions.GetMandatory (defaultSearchArguments.SearchStatement);
      if (definition.QueryType != QueryType.Collection)
        throw new ArgumentException (string.Format ("The query '{0}' is not a collection query.", defaultSearchArguments.SearchStatement));

      ClientTransaction clientTransaction = ClientTransactionScope.CurrentTransaction;

      DomainObjectCollection result = clientTransaction.QueryManager.GetCollection (QueryFactory.CreateQuery (definition));
      IBusinessObjectWithIdentity[] availableObjects = new IBusinessObjectWithIdentity[result.Count];

      if (availableObjects.Length > 0)
        result.CopyTo (availableObjects, 0);

      return availableObjects;
    }
  }
}
