using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
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
      return GetObject<SpecificFolder> (id);
    }

    protected SpecificFolder ()
    {
    }
  }
}