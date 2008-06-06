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

namespace Remotion.Mixins
{
  // Indicates that a mixin's method initialization parameter or a type parameter should be assigned the mixin's This value at initialization or
  // the mixin's This type at configuration analysis time.
  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  public class ThisAttribute : Attribute
  {
  }

  // Indicates that a mixin's method initialization parameter or a type parameter should be assigned the mixin's Base value at initialization or
  // the mixin's Base type at configuration analysis time.
  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  public class BaseAttribute : Attribute
  {
  }
}
