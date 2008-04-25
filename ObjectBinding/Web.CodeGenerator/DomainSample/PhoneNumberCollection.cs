using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;

namespace DomainSample
{
public class PhoneNumberCollection : DomainObjectCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PhoneNumberCollection () : base (typeof (PhoneNumber))
  {
  }

  // methods and properties

  public new PhoneNumber this[int index]
  {
    get { return (PhoneNumber) base[index]; }
    set { base[index] = value; }
  }

  public new PhoneNumber this[ObjectID id]
  {
    get { return (PhoneNumber) base[id]; }
  }

}
}
