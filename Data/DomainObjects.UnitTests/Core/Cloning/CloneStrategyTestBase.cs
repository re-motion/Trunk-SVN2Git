using NUnit.Framework;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Cloning
{
  public abstract class CloneStrategyTestBase : StandardMappingTest
  {
    protected MockRepository _mockRepository;
    protected DomainObjectCloner _cloner;
    protected CloneContext _contextMock;
    protected ClientTransaction _sourceTransaction;
    protected ClientTransaction _cloneTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _cloner = new DomainObjectCloner ();
      _contextMock = _mockRepository.Stub<CloneContext> (_cloner);
      _sourceTransaction = ClientTransaction.NewBindingTransaction ();
      _cloneTransaction = ClientTransaction.NewBindingTransaction ();
    }

    [Test]
    public void HandleReference_OneOne_RealSide ()
    {
      Computer source = NewBoundObject<Computer>(_sourceTransaction);
      Computer clone = NewBoundObject<Computer> (_cloneTransaction);
      Employee sourceRelated = NewBoundObject<Employee> (_sourceTransaction);
      Employee cloneRelated = NewBoundObject<Employee> (_cloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Computer), "Employee"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Computer), "Employee"];

      source.Employee = sourceRelated;

      HandleReference_OneOne_RealSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_RealSide_Checks (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneOne_VirtualSide ()
    {
      Employee source = NewBoundObject<Employee> (_sourceTransaction);
      Employee clone = NewBoundObject<Employee> (_cloneTransaction);
      Computer sourceRelated = NewBoundObject<Computer> (_sourceTransaction);
      Computer cloneRelated = NewBoundObject<Computer> (_cloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Employee), "Computer"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Employee), "Computer"];

      source.Computer = sourceRelated;

      HandleReference_OneOne_VirtualSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_VirtualSide_Checks (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneMany_RealSide ()
    {
      OrderItem source = NewBoundObject<OrderItem> (_sourceTransaction);
      OrderItem clone = NewBoundObject<OrderItem> (_cloneTransaction);
      Order sourceRelated = NewBoundObject<Order> (_sourceTransaction);
      Order cloneRelated = NewBoundObject<Order> (_cloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (OrderItem), "Order"];
      PropertyAccessor cloneReference = clone.Properties[typeof (OrderItem), "Order"];

      source.Order = sourceRelated;

      HandleReference_OneMany_RealSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneMany_RealSide_Checks (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneMany_VirtualSide ()
    {
      _cloner.CloneTransaction = _cloneTransaction;
      
      Order source = NewBoundObject<Order> (_sourceTransaction);
      Order clone = NewBoundObject<Order> (_cloneTransaction);
      OrderItem sourceRelated = NewBoundObject<OrderItem> (_sourceTransaction);
      OrderItem cloneRelated = NewBoundObject<OrderItem> (_cloneTransaction);

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