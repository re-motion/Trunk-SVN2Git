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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// An value indicating whether the original or current value of a <see cref="PropertyValue"/> is being accessed.
  /// </summary>
  public enum ValueAccess
  {
    /// <summary>
    /// The original value is being accessed.
    /// </summary>
    Original = 0,

    /// <summary>
    /// The current value is being accessed.
    /// </summary>
    Current = 1
  }
}
