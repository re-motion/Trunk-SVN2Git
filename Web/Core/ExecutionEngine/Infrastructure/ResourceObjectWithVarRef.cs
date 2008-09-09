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