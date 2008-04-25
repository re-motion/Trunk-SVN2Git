using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Employee : TestDomainBase
  {
    public static Employee NewObject ()
    {
      return NewObject<Employee> ().With();
    }

    public new static Employee GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Employee> (id);
    }

    public new static Employee GetObject (ObjectID id, bool includeDeleted)
    {
      return DomainObject.GetObject<Employee> (id, includeDeleted);
    }

    protected Employee ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Supervisor")]
    public abstract ObjectList<Employee> Subordinates { get; }

    [DBBidirectionalRelation ("Subordinates")]
    public abstract Employee Supervisor { get; set; }

    [DBBidirectionalRelation ("Employee")]
    public Computer Computer
    {
      get { return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"].GetValue<Computer>(); }
      set { Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"].SetValue (value); }
    }

    public void DeleteWithSubordinates ()
    {
      foreach (Employee employee in Subordinates.Clone ())
        employee.Delete ();

      this.Delete ();
    }
  }
}
