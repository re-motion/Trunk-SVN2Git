using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
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
  }
}
