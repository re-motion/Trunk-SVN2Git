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
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class InitObjectExpression : Expression
  {
    private readonly Reference _objectToBeInitialized;
    private readonly Type _type;

    public InitObjectExpression (Reference objectToBeInitialized, Type type)
    {
      ArgumentUtility.CheckNotNull ("objectToBeInitialized", objectToBeInitialized);
      ArgumentUtility.CheckNotNull ("type", type);

      _objectToBeInitialized = objectToBeInitialized;
      _type = type;
    }

    public InitObjectExpression (CustomMethodEmitter method, Type type)
        : this (ArgumentUtility.CheckNotNull ("method", method).DeclareLocal (type), type)
    {
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _objectToBeInitialized.LoadAddressOfReference (gen);
      gen.Emit (OpCodes.Initobj, _type);
      _objectToBeInitialized.LoadReference (gen);
    }
  }
}
