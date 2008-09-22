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
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.TestDomain
{
  public interface IVeryGenericMixin
  {
    string GetMessage<T>(T t);
  }

  public interface IVeryGenericMixin<T1, T2>
  {
    string GenericIfcMethod<T3> (T1 t1, T2 t2, T3 t3);
  }

  public class VeryGenericMixin<TThis, TBase> : Mixin<TThis, TBase>, IVeryGenericMixin<TThis, TBase>, IVeryGenericMixin
    where TThis : class
    where TBase : class
  {
    public string GenericIfcMethod<T3> (TThis t1, TBase t2, T3 t3)
    {
      return "IVeryGenericMixin.GenericIfcMethod-" + t3;
    }

    public string GetMessage<T> (T t)
    {
      return GenericIfcMethod (This, Base, t);
    }
  }

  public interface IUltraGenericMixin
  {
    string GetMessage<T> (T t);
  }

  public interface IADUGMThisDependencies : IBaseType31, IBaseType32, IBT3Mixin4 {}
  public interface IADUGMBaseDependencies : IBaseType31, IBaseType32, IBT3Mixin4 {}

  public abstract class AbstractDerivedUltraGenericMixin<TThis, TBase> : VeryGenericMixin<TThis, TBase>, IUltraGenericMixin
    where TThis : class, IADUGMThisDependencies
    where TBase : class, IADUGMBaseDependencies
  {
    protected abstract string AbstractGenericMethod<T>();

    public new string GetMessage<T> (T t)
    {
      return AbstractGenericMethod<T>() + "-" + base.GenericIfcMethod (This, Base, t);
    }
  }

  [Uses (typeof (AbstractDerivedUltraGenericMixin<,>), AdditionalDependencies = new Type[] { typeof (IBT3Mixin6) })]
  [Uses (typeof (BT3Mixin4))]
  public class ClassOverridingUltraGenericStuff : BaseType3
  {
    [OverrideMixin]
    public string AbstractGenericMethod<T> ()
    {
      return typeof (T).Name;
    }
  }
}
