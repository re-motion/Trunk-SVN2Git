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

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface ICBaseType3 : IBaseType31, IBaseType32, IBaseType33, IBaseType34, IBaseType35
  { }

  public interface ICBaseType3BT3Mixin4 : ICBaseType3, IBT3Mixin4
  { }
}
