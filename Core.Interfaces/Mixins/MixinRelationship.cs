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
  /// <summary>
  /// Describes how a mixin influences its target class.
  /// </summary>
  public enum MixinKind
  {
    /// <summary>
    /// The mixin extends the target class from the outside, the target class might not know about being mixed. The mixin therefore has the
    /// possibility to override attributes (with <see cref="AttributeUsageAttribute.AllowMultiple"/> set to false) and interfaces declared
    /// or implemented by the target class.
    /// </summary>
    Extending,
    /// <summary>
    /// The mixin is explicitly used by the target class. The mixin therefore behaves more like a base class, eg. attributes (with 
    /// <see cref="AttributeUsageAttribute.AllowMultiple"/> set to false) and interfaces introduced by the mixin can be overridden by the 
    /// target class.
    /// </summary>
    Used
  }
}
