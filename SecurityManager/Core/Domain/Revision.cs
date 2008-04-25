using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  public static class Revision
  {
    public static int GetRevision ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Revision.GetRevision");
      return (int) ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query);
    }

    public static void IncrementRevision ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Revision.IncrementRevision");
      ClientTransactionScope.CurrentTransaction.QueryManager.GetScalar (query);
    }
  }
}