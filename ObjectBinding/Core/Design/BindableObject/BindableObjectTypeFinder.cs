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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public class BindableObjectTypeFinder
  {
    private readonly IServiceProvider _serviceProvider;

    public BindableObjectTypeFinder (IServiceProvider serviceProvider)
    {
      ArgumentUtility.CheckNotNull ("serviceProvider", serviceProvider);
      _serviceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider
    {
      get { return _serviceProvider; }
    }

    public List<Type> GetTypes (bool includeGac)
    {
      ICollection types = GetAllDesignerTypes (includeGac);
      MixinConfiguration applicationContext = GetMixinConfiguration (types);

      List<Type> bindableTypes = new List<Type>();
      using (applicationContext.EnterScope ())
      {
        foreach (Type type in types)
        {
          if (IsBindableObjectImplementation (type))
            bindableTypes.Add (type);
        }
      }
      return bindableTypes;
    }

    public static bool IsBindableObjectImplementation(Type type)
    {
      return !MixinTypeUtility.IsGeneratedByMixinEngine (type) && (HasBindableObjectMixin(type) || IsDerivedFromBindableObjectBase(type));
    }

    private static bool IsDerivedFromBindableObjectBase(Type type)
    {
      return AttributeUtility.IsDefined (type, typeof (BindableObjectBaseClassAttribute), true);
    }

    private static bool HasBindableObjectMixin(Type type)
    {
      return MixinTypeUtility.HasAscribableMixin (type, typeof (BindableObjectMixinBase<>));
    }

    public MixinConfiguration GetMixinConfiguration (bool includeGac)
    {
      ICollection typesToBeAnalyzed = GetAllDesignerTypes (includeGac);
      return GetMixinConfiguration(typesToBeAnalyzed);
    }

    private MixinConfiguration GetMixinConfiguration (ICollection typesToBeAnalyzed)
    {
      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (null);
      foreach (Type type in typesToBeAnalyzed)
        builder.AddType (type);

      return builder.BuildConfiguration ();
    }

    private ICollection GetAllDesignerTypes (bool includeGac)
    {
      ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService) _serviceProvider.GetService (typeof (ITypeDiscoveryService));
      if (typeDiscoveryService == null)
        return Type.EmptyTypes;
      else
        return typeDiscoveryService.GetTypes (typeof (object), !includeGac);
    }
  }
}
