using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_SpecificFolder")]
  [Instantiable]
  public abstract class SpecificFolder : Folder
  {
    public new static SpecificFolder NewObject()
    {
      return NewObject<SpecificFolder> ().With ();
    }

    public new static SpecificFolder GetObject (ObjectID id)
    {
      return DomainObject.GetObject<SpecificFolder> (id);
    }

    protected SpecificFolder ()
    {
    }
  }
}