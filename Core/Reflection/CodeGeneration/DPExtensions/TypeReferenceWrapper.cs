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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class TypeReferenceWrapper : TypeReference
  {
    private readonly Reference _referenceToWrap;
    private readonly Type _referenceType;

    public TypeReferenceWrapper (Reference referenceToWrap, Type referenceType)
      : base (referenceToWrap.OwnerReference, referenceType)
    {
      ArgumentUtility.CheckNotNull ("referenceToWrap", referenceToWrap);
      ArgumentUtility.CheckNotNull ("referenceType", referenceType);

      _referenceToWrap = referenceToWrap;
      _referenceType = referenceType;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      _referenceToWrap.LoadAddressOfReference (gen);
    }

    public override void LoadReference (ILGenerator gen)
    {
      _referenceToWrap.LoadReference (gen);
    }

    public override void StoreReference (ILGenerator gen)
    {
      _referenceToWrap.StoreReference (gen);
    }
  }
}
