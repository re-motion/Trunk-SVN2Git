using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Cloning
{
  [TestFixture]
  [Ignore ("TODO: Cloning")]
  public class DomainObjectClonerTest : ClientTransactionBaseTest
  {
    //[Test]
    //public void CreateValueClone_CreatesNewObject ()
    //{
    //  DomainObjectCloner cloner = new DomainObjectCloner ();

    //  ClassWithAllDataTypes original = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
    //  ClassWithAllDataTypes clone = cloner.CreateValueClone (original);

    //  Assert.That (clone, Is.Not.SameAs (original));
    //  Assert.That (clone.ID, Is.Not.EqualTo (original));
    //  Assert.That (clone.State, Is.EqualTo (StateType.New));
    //}

    //[Test]
    //public void CreateValueClone_SimpleProperties ()
    //{
    //  DomainObjectCloner cloner = new DomainObjectCloner ();

    //  ClassWithAllDataTypes original = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
    //  ClassWithAllDataTypes clone = cloner.CreateValueClone (original);

    //  Assert.That (clone.Int32Property, Is.EqualTo (original.Int32Property));
    //}

    //[Test]
    //public void CreateValueClone_OriginalValueCloned ()
    //{
    //  DomainObjectCloner cloner = new DomainObjectCloner ();

    //  ClassWithAllDataTypes original = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
    //  original.Int32Property = 2 * original.Int32Property;
    //  ClassWithAllDataTypes clone = cloner.CreateValueClone (original);

    //  Assert.That (clone.Int32Property, Is.EqualTo (original.Int32Property));
    //  Assert.That (clone.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int>(),
    //      Is.EqualTo (original.Properties[typeof (ClassWithAllDataTypes), "Int32Property"].GetOriginalValue<int> ()));
    //}

    //[Test]
    //public void Create_ValueClone_RelationProperties_NonForeignKey ()
    //{
    //  DomainObjectCloner cloner = new DomainObjectCloner ();

    //  Order order = Order.GetObject (DomainObjectIDs.Order1);
    //  Order clone = cloner.CreateValueClone (order);

    //  Assert.That (clone.OrderItems, Is.Empty);
    //  Assert.That (clone.OrderTicket, Is.Null);
    //}

    //[Test]
    //public void Create_ValueClone_RelationProperties_ForeignKey ()
    //{
    //  DomainObjectCloner cloner = new DomainObjectCloner ();

    //  Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
    //  Computer clone = cloner.CreateValueClone (computer);

    //  Assert.That (computer.Employee, Is.Not.Null);
    //  Assert.That (clone.Employee, Is.Null);
    //  Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].Value, Is.Null);
    //}

    //[Test]
    //public void Create_ValueClone_RelationProperties_ForeignKey_OriginalValue ()
    //{
    //  DomainObjectCloner cloner = new DomainObjectCloner ();

    //  Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
    //  Computer clone = cloner.CreateValueClone (computer);

    //  Assert.That (computer.Employee, Is.Not.Null);
    //  Assert.That (clone.Properties[typeof (Computer), "Employee"].GetOriginalValue<Employee>(), Is.Null);
    //  Assert.That (clone.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].OriginalValue, Is.Null);
    //}
  }
}