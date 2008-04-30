using System;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  public class FakeDomainObjectFactory : IDomainObjectFactory
  {
    public Type GetConcreteDomainObjectType (Type baseType)
    {
      throw new NotImplementedException ();
    }

    public Type GetConcreteDomainObjectType (ClassDefinition baseTypeClassDefinition, Type concreteBaseType)
    {
      throw new NotImplementedException();
    }

    public bool WasCreatedByFactory (Type t)
    {
      throw new NotImplementedException ();
    }

    public IFuncInvoker<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type type) where TMinimal : DomainObject
    {
      throw new NotImplementedException ();
    }

    public void PrepareUnconstructedInstance (DomainObject instance)
    {
      throw new NotImplementedException ();
    }
  }
}
