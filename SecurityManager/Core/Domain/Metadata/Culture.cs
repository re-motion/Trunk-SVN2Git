using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Culture : BaseSecurityManagerObject
  {
    public static Culture NewObject (string cultureName)
    {
      return NewObject<Culture> ().With (cultureName);
    }

    public static Culture Find (string name)
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Metadata.Culture.Find");
      query.Parameters.Add ("@cultureName", name);

      DomainObjectCollection result = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      if (result.Count == 0)
        return null;

      return (Culture) result[0];
    }

    protected Culture (string cultureName)
    {
      ArgumentUtility.CheckNotNull ("cultureName", cultureName);
      
      CultureName = cultureName;
    }

    [StringProperty (IsNullable = false, MaximumLength = 10)]
    public abstract string CultureName { get; protected set; }
  }
}
