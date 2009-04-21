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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  // Converts an expression to a reference by saving it as a temporary local variable at time of emitting
  public class ExpressionReference : TypeReference
  {
    private readonly Expression _expression;
    private readonly MethodEmitter _methodEmitter;
    private readonly Type _referenceType;

    public ExpressionReference (Type referenceType, Expression expression, MethodEmitter methodEmitter) : base (referenceType)
    {
      _referenceType = referenceType;
      _expression = expression;
      _methodEmitter = methodEmitter;
    }

    public ExpressionReference (Type referenceType, Expression expression, CustomMethodEmitter methodEmitter)
      : this (referenceType, expression, methodEmitter.InnerEmitter)
    {
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      LocalReference local = CreateLocal (gen);
      local.LoadAddressOfReference (gen);
    }

    public override void LoadReference (ILGenerator gen)
    {
      LocalReference local = CreateLocal(gen);
      local.LoadReference (gen);
    }

    private LocalReference CreateLocal (ILGenerator gen)
    {
      LocalReference local = _methodEmitter.CodeBuilder.DeclareLocal (_referenceType);
      local.Generate (gen);
      new AssignStatement (local, _expression).Emit (_methodEmitter, gen);
      return local;
    }

    public override void StoreReference (ILGenerator gen)
    {
      throw new NotSupportedException ("Expressions cannot be assigned to.");
    }
  }
}
