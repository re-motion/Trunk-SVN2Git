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
using System.Reflection;
using Remotion.Collections;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public abstract class ResourceObjectBase
  {
    private readonly string _resourceRoot;

    protected ResourceObjectBase (Assembly assembly)
    {
      if (assembly != null)
        _resourceRoot = ResourceUrlResolver.GetAssemblyRoot (false, assembly);
      else
        _resourceRoot = string.Empty;
    }

    public string ResourceRoot
    {
      get { return _resourceRoot; }
    }

    public abstract string GetResourcePath (NameObjectCollection variables);
  }
}