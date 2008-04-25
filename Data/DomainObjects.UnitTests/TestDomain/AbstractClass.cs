using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class AbstractClass: DomainObject
  {
    public static AbstractClass NewObject()
    {
      return NewObject<AbstractClass>().With();
    }

    protected AbstractClass()
    {
    }
  }
}