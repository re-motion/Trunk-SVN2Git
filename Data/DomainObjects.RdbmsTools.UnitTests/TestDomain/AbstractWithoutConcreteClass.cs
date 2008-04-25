using System;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  public abstract class AbstractWithoutConcreteClass : Company
  {
    public new static AbstractWithoutConcreteClass NewObject()
    {
      return NewObject<AbstractWithoutConcreteClass>().With();
    }

    protected AbstractWithoutConcreteClass()
    {
    }
  }
}