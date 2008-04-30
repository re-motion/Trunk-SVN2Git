using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
{
  public class InvertingClientTransactionMixin : Mixin<ClientTransaction, InvertingClientTransactionMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      void Commit ();
      void Rollback ();
    }

    [OverrideTarget]
    public void Commit ()
    {
      Base.Rollback (); // okay, this is not really realistic
    }

    [OverrideTarget]
    public void Rollback ()
    {
      Base.Commit ();
    }
  }
}