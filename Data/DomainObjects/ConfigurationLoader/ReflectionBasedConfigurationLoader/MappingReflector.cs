/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
