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
using Remotion.Reflection;

namespace Remotion.Implementation
{
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
  public class ConcreteImplementationAttribute : Attribute
  {
    private readonly string _partialTypeNameTemplate;

    public ConcreteImplementationAttribute (string partialTypeNameTemplate)
    {
      _partialTypeNameTemplate = ArgumentUtility.CheckNotNull ("partialTypeNameTemplate", partialTypeNameTemplate);
    }

    public string PartialTypeNameTemplate
    {
      get { return _partialTypeNameTemplate; }
    }

    public string GetPartialTypeName()
    {
      return _partialTypeNameTemplate.Replace ("<version>", FrameworkVersion.Value.ToString());
    }

    public Type ResolveType ()
    {
      return ContextAwareTypeDiscoveryUtility.GetType (GetPartialTypeName (), true);
    }

    public object InstantiateType ()
    {
      return Activator.CreateInstance (ResolveType());
    }
  }
}
