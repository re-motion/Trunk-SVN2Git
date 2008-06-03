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

#pragma warning disable 0693

namespace Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes
{
  public interface IGeneric<T>
  {
    string Generic<T> (T t);
  }

  public class MixinIntroducingGenericInterface<T> : Mixin<T>, IGeneric<T>
    where T : class
  {
    public string Generic<T> (T t)
    {
      return "Generic";
    }
  }
}
