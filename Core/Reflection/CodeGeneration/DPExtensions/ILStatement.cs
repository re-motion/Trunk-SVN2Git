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

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class ILStatement : Statement
  {
    private readonly Proc<IMemberEmitter, ILGenerator> _ilSource;

    public ILStatement (Proc<IMemberEmitter, ILGenerator> ilSource)
    {
      _ilSource = ilSource;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _ilSource (member, gen);
    }
  }
}
