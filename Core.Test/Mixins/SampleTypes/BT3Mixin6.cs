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

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT3Mixin6ThisDependencies : IBaseType31, IBaseType32, IBaseType33, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6BaseDependencies : IBaseType34, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6 { }

  [Extends(typeof(BaseType3))]
  [Serializable]
  public class BT3Mixin6<TThis, TBase> : Mixin<TThis, TBase>, IBT3Mixin6
      where TThis : class, IBT3Mixin6ThisDependencies
      where TBase : class, IBT3Mixin6BaseDependencies
  {
  }
}
