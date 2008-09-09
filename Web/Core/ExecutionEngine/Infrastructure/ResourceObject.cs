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
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class ResourceObject : ResourceObjectBase
  {
    private readonly string _path;

    public ResourceObject (Assembly assembly, string path)
        : base(assembly)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      _path = path;
    }

    public string Path
    {
      get { return _path; }
    }

    public override string GetResourcePath (NameObjectCollection variables)
    {
      return ResourceRoot + _path;
    }
  }
}