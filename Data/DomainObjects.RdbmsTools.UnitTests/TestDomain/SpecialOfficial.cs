using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class SpecialOfficial : Official
  {
    public static new SpecialOfficial NewObject ()
    {
      return NewObject<SpecialOfficial>().With();
    }

    protected SpecialOfficial()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string Speciality { get; set;}
  }
}
