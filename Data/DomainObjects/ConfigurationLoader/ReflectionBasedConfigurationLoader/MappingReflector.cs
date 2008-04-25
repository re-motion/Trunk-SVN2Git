using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using Remotion.Data.DomainObjects.Design;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  [DesignModeMappingLoader (typeof (DesignModeMappingReflector))]
  public class MappingReflector : MappingReflectorBase
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;

    //TODO: Test
    public MappingReflector ()
    {
      _typeDiscoveryService = ContextAwareTypeDiscoveryService.GetInstance();
    }

    public MappingReflector (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
    }

    protected override Type[] GetDomainObjectTypes ()
    {
      List<Type> domainObjectClasses = new List<Type>();
      foreach (Type type in _typeDiscoveryService.GetTypes (null, false))
      {
        if (typeof (DomainObject).IsAssignableFrom (type) && !domainObjectClasses.Contains (type))
          domainObjectClasses.Add (type);
      }

      return domainObjectClasses.ToArray();
    }
  }
}