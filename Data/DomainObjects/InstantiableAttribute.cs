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
  /// When the <see cref="InstantiableAttribute"/> is defined on a type, it signals that this type can be instantiated by the
  /// <see cref="DomainObject"/> infrastructure even though it declared as <see langword="abstract"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class InstantiableAttribute: Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InstantiableAttribute"/> class.
    /// </summary>
    public InstantiableAttribute()
    {
    }
  }
}
