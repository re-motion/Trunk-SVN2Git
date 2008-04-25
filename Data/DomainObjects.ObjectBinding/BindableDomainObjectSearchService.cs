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
    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      if (searchStatement == null || searchStatement == string.Empty)
        return new IBusinessObjectWithIdentity[] { };

      QueryDefinition definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions.GetMandatory (searchStatement);
      if (definition.QueryType != QueryType.Collection)
        throw new ArgumentException (string.Format ("The query '{0}' is not a collection query.", searchStatement), "searchStatement");

      ClientTransaction clientTransaction = ClientTransactionScope.CurrentTransaction;

      DomainObjectCollection result = clientTransaction.QueryManager.GetCollection (new Query (definition));
      IBusinessObjectWithIdentity[] availableObjects = new IBusinessObjectWithIdentity[result.Count];

      if (availableObjects.Length > 0)
        result.CopyTo (availableObjects, 0);

      return availableObjects;
    }
  }
}