using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
{
  [TestFixture]
  public class AddToOneToManyRelationWithLazyLoadTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AddToOneToManyRelationWithLazyLoadTest ()
    {
    }

    // methods and properties

    [Test]
    public void Insert ()
    {
      Employee newSupervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      Employee subordinate = Employee.GetObject (DomainObjectIDs.Employee3);

      int countBeforeInsert = newSupervisor.Subordinates.Count;

      newSupervisor.Subordinates.Insert (0, subordinate);

      Assert.AreEqual (countBeforeInsert + 1, newSupervisor.Subordinates.Count);
      Assert.AreEqual (0, newSupervisor.Subordinates.IndexOf (subordinate));
      Assert.AreSame (newSupervisor, subordinate.Supervisor);

      Employee oldSupervisor = Employee.GetObject (DomainObjectIDs.Employee2);
      Assert.IsFalse (oldSupervisor.Subordinates.ContainsObject (subordinate));
    }
  }
}
