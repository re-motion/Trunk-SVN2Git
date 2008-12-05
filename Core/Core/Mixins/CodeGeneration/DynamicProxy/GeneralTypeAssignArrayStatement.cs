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
