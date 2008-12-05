// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class ResourceObjectWithVarRef : ResourceObjectBase
  {
    private readonly WxeVariableReference _pathReference;

    public ResourceObjectWithVarRef (Assembly assembly, WxeVariableReference pathReference)
        : base(assembly)
    {
      ArgumentUtility.CheckNotNull ("pathReference", pathReference);
      _pathReference = pathReference;
    }

    public override string GetResourcePath (NameObjectCollection variables)
    {
      ArgumentUtility.CheckNotNull ("variables", variables);

      object pageObject =  variables[_pathReference.Name];
      if (pageObject == null)
        throw new InvalidOperationException (string.Format ("The variable '{0}' could not be found in the list of variables.", _pathReference.Name));
      
      string page = pageObject as string;
      if (page == null)
        throw new InvalidCastException (string.Format ("The variable '{0}' was of type '{1}'. Expected type is '{2}'.", _pathReference.Name, pageObject.GetType().FullName, typeof (string).FullName));

      return ResourceRoot + page;
    }

    public WxeVariableReference PathReference
    {
      get { return _pathReference; }
    }
  }
}
