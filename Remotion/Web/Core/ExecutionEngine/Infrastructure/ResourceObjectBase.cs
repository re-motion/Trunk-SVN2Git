// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
