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

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class GenericClassWithGenericMethod<TConcreteInterfaceConstraint, TConcreteClassConstraint, TStructConstraint, TClassNewConstraint,
      TSelfConstraint, TComplexSelfConstraint>
      where TConcreteInterfaceConstraint : IConvertible
      where TConcreteClassConstraint : List<string>
      where TStructConstraint : struct
      where TClassNewConstraint : class, new ()
      where TSelfConstraint : TConcreteInterfaceConstraint
      where TComplexSelfConstraint : List<List<TSelfConstraint[]>>
  {
    public virtual string GenericMethod<T1, T2> (T1 t1, T2 t2, TComplexSelfConstraint tSelf)
        where T1 : TConcreteInterfaceConstraint
        where T2 : List<TStructConstraint[]>
    {
      return string.Format ("{0}, {1}, {2}", t1, t2, tSelf);
    }
  }
}
