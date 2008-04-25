using System;

namespace Remotion.Data.DomainObjects.DataManagement
{
public interface ICollectionEndPointChangeDelegate
{
  void PerformInsert (CollectionEndPoint endPoint, DomainObject domainObject, int index);
  void PerformReplace (CollectionEndPoint endPoint, DomainObject domainObject, int index);
  void PerformSelfReplace (CollectionEndPoint endPoint, DomainObject domainObject, int index);
  void PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject);
}
}
