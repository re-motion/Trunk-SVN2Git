// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Mixins.Context;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class ConcreteMixedTypeAttributeUtility
  {
    private static readonly ConstructorInfo s_attributeCtor = 
        typeof (ConcreteMixedTypeAttribute).GetConstructor (new[] {typeof (object[]), typeof (Type[])});

    public static CustomAttributeBuilder CreateAttributeBuilder (ClassContext context, IEnumerable<Type> orderedMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("orderedMixinTypes", orderedMixinTypes);

      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixedTypeAttribute attribute = ConcreteMixedTypeAttribute.FromClassContext (context, orderedMixinTypes.ToArray());
      var builder = new CustomAttributeBuilder (s_attributeCtor, new object[] { attribute.ClassContextData, attribute.OrderedMixinTypes });
      return builder;
    }
  }
}
