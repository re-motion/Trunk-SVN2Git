/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding
{
  internal class InterfaceEmitter : AbstractTypeEmitter
  {
    private static TypeBuilder CreateTypeBuilder (ModuleScope scope, string typeName)
    {
      return scope.ObtainDynamicModule (true).DefineType (typeName, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
    }

    public InterfaceEmitter (ModuleScope scope, string typeName)
        : base (CreateTypeBuilder (ArgumentUtility.CheckNotNull ("scope", scope), ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName)))
    {
    }

    protected override void EnsureBuildersAreInAValidState ()
    {
      // do not call base implementation, we don't want any method bodies or constructors
    }
  }
}
