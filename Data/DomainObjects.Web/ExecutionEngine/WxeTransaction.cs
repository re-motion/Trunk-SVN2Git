using System;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.ExecutionEngine
{
  [Serializable]
  [Obsolete ("Use WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> instead.")]
  public class WxeTransaction : WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>
  {
    public WxeTransaction (WxeStepList steps, bool autoCommit, bool forceRoot)
        : base (steps, autoCommit, forceRoot)
    {
    }

    public WxeTransaction (bool autoCommit, bool forceRoot)
        : base (autoCommit, forceRoot)
    {
    }

    public WxeTransaction (bool autoCommit)
        : base (autoCommit)
    {
    }

    public WxeTransaction ()
    {
    }
  }
}