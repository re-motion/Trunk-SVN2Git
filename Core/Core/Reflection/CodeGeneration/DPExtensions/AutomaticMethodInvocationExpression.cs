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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class AutomaticMethodInvocationExpression : TypedMethodInvocationExpression
  {
    private readonly TypeReference _owner;

    public AutomaticMethodInvocationExpression (TypeReference owner, MethodInfo method, params Expression[] arguments)
        : base (owner, method, arguments)
    {
      _owner = owner;
    }

    protected override void EmitCall (IMemberEmitter member, ILGenerator gen)
    {
      if (_owner.Type.IsValueType || Method.IsStatic)
        gen.Emit (OpCodes.Call, Method);
      else
        gen.Emit (OpCodes.Callvirt, Method);
    }
  }
}
