// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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
