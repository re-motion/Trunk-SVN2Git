using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Cloning
{
  [TestFixture]
  public class DomainObjectClonerTest : ClientTransactionBaseTest
  {
    private DomainObjectCloner _cloner;
    private ClassWithAllDataTypes _classWithAllDataTypes;
    private Order _order1;
    private Computer _computer1;
    private ClassWithAllDataTypes _boundSource;

    public override void SetUp ()
    {
      base.SetUp();
      _cloner = new DomainObjectCloner ();
      _classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _computer1 = Computer.GetObject (DomainObjectIDs.Computer1);

      using (ClientTransaction.NewBindingTransaction ().EnterNonDiscardingScope ())
      {
        _boundSource = ClassWithAllDataTypes.NewObject ();
        _boundSource.Int32Property = 123;
      }
    }

    [Test]
    public void CreateValueClone_CreatesNewObject ()
    {
      DomainObject clone = _cloner.CreateValueClone (_classWithAllDataTypes);
      Assert.That (clone, Is.Not.SameAs (_classWithAllDataTypes));
      Assert.That (clone.ID, Is.Not.EqualTo (_classWithAllDataTypes));
      Assert.That (clone.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CreateValueClone_CreatesObjectOfSameType ()
    {
      DomainObject clone = _cloner.CreateValueClone<DomainObject> (_classWithAllDataTypes);
      Assert.That (clone.GetPublicDomainObjectType(), Is.SameAs (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    public void CreateValueClone_CallsNoCtor ()
    {
      Order clone = _cloner.CreateValueClone (_order1);
      Assert.That (clone.CtorCalled, Is.False);
    }

    [Test]
    public void CreateValueClone_RegistersDataContainer ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateValueClone (_classWithAllDataTypes);
      Assert.That (clone.InternalDataContainer.ClientTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void CreateValueClone_SimpleProperties ()
    {
      ClassWithAllDataTypes clone = _cloner.CreateValueClone (_classWithAllDataTypes);
      Assert.That (clone.Int32Property, Is.EqualTo (_classWithAllDataTypes.Int32Property));
    }

    [Test]
    public void CreateValueClone_OriginalValueNotCloned ()
    {
      _classWithAllDataTypes.Int32Property = 2 * _classWithAllDataTypes.Int32Property;
      ClassWithAllDataTypes clone = _cloner.CreateValueClone (_classWithAllDataTypes);

      Assert.That (clone.Int32Property, Is.EqualTo (_classWithAllDataTypes.Int32Property));
      Assert.That (clone.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int> (),
          Is.Not.EqualTo (_classWithAllDataTypes.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int> ()));
      Assert.That (clone.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int> (), Is.EqualTo (0));
    }

    [Test]
    public void CreateValueClone_RelationProperties_NonForeignKey ()
    {
      Order clone = _cloner.CreateValueClone (_order1);
      Assert.That (clone.OrderItems, Is.Empty);
      Assert.That (clone.OrderTicket, Is.Null);
    }

    [Test]
    public void CreateValueClone_RelationProperties_ForeignKey ()
    {
      Computer clone = _cloner.CreateValueClone (_computer1);
      Assert.That (_computer1.Employee, Is.Not.Null);
      Assert.That (clone.Employee, Is.Null);
      Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].Value, Is.Null);
    }

    [Test]
    public void CreateValueClone_RelationProperties_ForeignKey_OriginalValue ()
    {
      Computer clone = _cloner.CreateValueClone (_computer1);
      Assert.That (_computer1.Employee, Is.Not.Null);
      Assert.That (clone.Properties[typeof (Computer), "Employee"].GetOriginalValue<Employee> (), Is.Null);
      Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].OriginalValue, Is.Null);
    }

    [Test]
    public void SourceTransaction_IsRespected ()
    {
      ClassWithAllDataTypes unboundClone = _cloner.CreateValueClone (_boundSource);
      Assert.That (unboundClone.Int32Property, Is.EqualTo (123));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread or " 
        + "this object.")]
    public void NullSourceTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _cloner.CreateValueClone (_classWithAllDataTypes);
      }
    }

    [Test]
    public void CloneTransaction_IsCurrent ()
    {
      ClassWithAllDataTypes unboundClone = _cloner.CreateValueClone (_boundSource);
      Assert.That (unboundClone.ClientTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NullCloneTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _cloner.CreateValueClone (_boundSource);
      }
    }
  }
}