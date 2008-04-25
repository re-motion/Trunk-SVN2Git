using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Transaction
{
  public interface IClientTransactionWithID
  {
    Guid ID { get; }
    void Commit ();
    void Rollback ();
    ClientTransaction AsClientTransaction { get; }
  }

  public class ClientTransactionWithIDMixin : Mixin<ClientTransaction>, IClientTransactionWithID
  {
    private readonly Guid _id;

    public ClientTransactionWithIDMixin ()
    {
      _id = Guid.NewGuid ();
    }

    public Guid ID
    {
      get { return _id; }
    }

    public void Commit ()
    {
      This.Commit ();
    }

    public void Rollback ()
    {
      This.Rollback ();
    }

    public ClientTransaction AsClientTransaction
    {
      get { return This; }
    }

    [OverrideTarget]
    public new string ToString ()
    {
      return ID.ToString ();
    }
  }
}