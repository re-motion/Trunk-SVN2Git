using NUnit.Framework;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Cloning
{
  public abstract class CloneStrategyTestBase : StandardMappingTest
  {
    private MockRepository _mockRepository;
    private DomainObjectCloner _cloner;
    private CloneContext _contextMock;
    private ClientTransaction _sourceTransaction;
    private ClientTransaction _cloneTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _cloner = new DomainObjectCloner ();
      _contextMock = MockRepository.Stub<CloneContext> (Cloner);
      _sourceTransaction = ClientTransaction.NewBindingTransaction ();
      _cloneTransaction = ClientTransaction.NewBindingTransaction ();
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    protected DomainObjectCloner Cloner
    {
      get { return _cloner; }
    }

    protected CloneContext ContextMock
    {
      get { return _contextMock; }
    }

    protected ClientTransaction SourceTransaction
    {
      get { return _sourceTransaction; }
    }

    protected ClientTransaction CloneTransaction
    {
      get { return _cloneTransaction; }
    }

    [Test]
    public void HandleReference_OneOne_RealSide ()
    {
      Computer source = NewBoundObject<Computer>(SourceTransaction);
      Computer clone = NewBoundObject<Computer> (CloneTransaction);
      Employee sourceRelated = NewBoundObject<Employee> (SourceTransaction);
      Employee cloneRelated = NewBoundObject<Employee> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Computer), "Employee"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Computer), "Employee"];

      source.Employee = sourceRelated;

      HandleReference_OneOne_RealSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_RealSide_Checks (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneOne_VirtualSide ()
    {
      Employee source = NewBoundObject<Employee> (SourceTransaction);
      Employee clone = NewBoundObject<Employee> (CloneTransaction);
      Computer sourceRelated = NewBoundObject<Computer> (SourceTransaction);
      Computer cloneRelated = NewBoundObject<Computer> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Employee), "Computer"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Employee), "Computer"];

      source.Computer = sourceRelated;

      HandleReference_OneOne_VirtualSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_VirtualSide_Checks (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneMany_RealSide ()
    {
      OrderItem source = NewBoundObject<OrderItem> (SourceTransaction);
      OrderItem clone = NewBoundObject<OrderItem> (CloneTransaction);
      Order sourceRelated = NewBoundObject<Order> (SourceTransaction);
      Order cloneRelated = NewBoundObject<Order> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (OrderItem), "Order"];
      PropertyAccessor cloneReference = clone.Properties[typeof (OrderItem), "Order"];

      source.Order = sourceRelated;

      HandleReference_OneMany_RealSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneMany_RealSide_Checks (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneMany_VirtualSide ()
    {
      Cloner.CloneTransaction = CloneTransaction;
      
      Order source = NewBoundObject<Order> (SourceTransaction);
      Order clone = NewBoundObject<Order> (CloneTransaction);
      OrderItem sourceRelated = NewBoundObject<OrderItem> (SourceTransaction);
      OrderItem cloneRelated = NewBoundObject<OrderItem> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Order), "OrderItems"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Order), "OrderItems"];

      source.OrderItems.Add (sourceRelated);

      HandleReference_OneMany_VirtualSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneMany_VirtualSide_Checks (OrderItem sourceRelated, PropertyAccessor sourceReference, OrderItem cloneRelated, PropertyAccessor cloneReference);

    private T NewBoundObject<T> (ClientTransaction transaction)
        where T : DomainObject
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        return (T) RepositoryAccessor.NewObject (typeof (T)).With ();
      }
    }
  }
}