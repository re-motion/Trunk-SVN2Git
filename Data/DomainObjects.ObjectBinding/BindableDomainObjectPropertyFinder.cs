using System;
using System.Reflection;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  internal class BindableDomainObjectPropertyFinder : ReflectionBasedPropertyFinder
  {
    public BindableDomainObjectPropertyFinder (Type targetType)
        : base (targetType)
    {
    }

    protected override bool IsInfrastructureProperty (PropertyInfo propertyInfo, MethodInfo accessorDeclaration)
    {
      return base.IsInfrastructureProperty (propertyInfo, accessorDeclaration) || accessorDeclaration.DeclaringType == typeof (DomainObject);
    }
  }
}