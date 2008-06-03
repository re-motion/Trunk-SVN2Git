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
using Castle.DynamicProxy;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public static class AssemblySaver
  {
    public static string[] SaveAssemblies (ModuleScope scope)
    {
      List<string> paths = new List<string> ();

      if (scope.StrongNamedModule != null)
      {
        scope.SaveAssembly (true);
        paths.Add (scope.StrongNamedModule.FullyQualifiedName);
      }

      if (scope.WeakNamedModule != null)
      {
        scope.SaveAssembly (false);
        paths.Add (scope.WeakNamedModule.FullyQualifiedName);
      }
      return paths.ToArray ();
    }
  }
}
