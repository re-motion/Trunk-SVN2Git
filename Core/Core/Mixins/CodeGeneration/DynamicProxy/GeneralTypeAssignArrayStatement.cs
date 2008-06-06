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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal class GeneralTypeAssignArrayStatement : Statement
  {
    private readonly Type _elementType;
    private readonly Reference _arrayReference;
    private readonly int _elementIndex;
    private readonly Expression _elementValue;

    public GeneralTypeAssignArrayStatement (Type elementType, Reference arrayReference, int elementIndex, Expression elementValue)
    {
      _elementType = elementType;
      _arrayReference = arrayReference;
      _elementIndex = elementIndex;
      _elementValue = elementValue;
    }

    public override void Emit (IMemberEmitter member, ILGenerator il)
    {
      ArgumentsUtil.EmitLoadOwnerAndReference (_arrayReference, il);
      il.Emit (OpCodes.Ldc_I4, _elementIndex);
      _elementValue.Emit (member, il);
      il.Emit (OpCodes.Stelem, _elementType);
    }
  }
}
