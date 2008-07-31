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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Mixins;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Linq
{
  /// <summary>
  /// This attribute applies the <see cref="QueryExecutorMixin"/> to the <see cref="QueryExecutor{T}"/> type.
  /// Apply this attribute to your (unit test) assembly.
  /// </summary>
  public class ApplyQueryExecutorMixinAttribute : MixAttribute
  {
    public ApplyQueryExecutorMixinAttribute ()
        : base (typeof (QueryExecutor<>), typeof (QueryExecutorMixin))
    {
    }
  }
}