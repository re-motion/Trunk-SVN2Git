using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;

namespace DomainSample
{
public class PersonCollection : DomainObjectCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PersonCollection () : base (typeof (Person))
  {
  }

  // methods and properties

  public new Person this[int index]
  {
    get { return (Person) base[index]; }
    set { base[index] = value; }
  }

  public new Person this[ObjectID id]
  {
    get { return (Person) base[id]; }
  }

}
}
