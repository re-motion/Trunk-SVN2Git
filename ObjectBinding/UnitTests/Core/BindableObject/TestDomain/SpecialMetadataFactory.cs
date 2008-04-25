using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public class UseSpecialMetadataFactoryAttribute : UseCustomMetadataFactoryAttribute
  {
    public override IMetadataFactory GetFactoryInstance ()
    {
      return SpecialMetadataFactory.Instance;
    }
  }

  public class SpecialMetadataFactory : DefaultMetadataFactory
  {
    public static readonly new SpecialMetadataFactory Instance = new SpecialMetadataFactory ();

    private class SpecialPropertyFinder : IPropertyFinder
    {
      private readonly Type _concreteType;

      public SpecialPropertyFinder (Type concreteType)
      {
        _concreteType = concreteType;
      }

      public IEnumerable<IPropertyInformation> GetPropertyInfos ()
      {
        return PropertyInfoAdapter.AdaptCollection (Mixins.TypeUtility.GetUnderlyingTargetType (_concreteType)
            .GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
      }
    }

    public override IPropertyFinder CreatePropertyFinder (Type concreteType)
    {
      return new SpecialPropertyFinder (concreteType);
    }
  }
}