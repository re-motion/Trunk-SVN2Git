// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.UnitTests.Core
{
  public static class ClassContextObjectMother
  {
    public static ClassContext Create (Type type)
    {
      return new ClassContext (type, Enumerable.Empty<MixinContext>(), Enumerable.Empty<Type>());
    }

    public static ClassContext Create (Type type, params MixinContext[] mixins)
    {
      return new ClassContext (type, mixins, Enumerable.Empty<Type>());
    }

    public static ClassContext Create (Type type, params Type[] mixinTypes)
    {
      var mixinContexts = GetMixinContexts (mixinTypes);
      var completeInterfaces = Enumerable.Empty<Type>();
      return new ClassContext (type, mixinContexts, completeInterfaces);
    }

    private static IEnumerable<MixinContext> GetMixinContexts (Type[] mixinTypes)
    {
      var mixins = new Dictionary<Type, MixinContext> (mixinTypes.Length);
      foreach (Type mixinType in mixinTypes)
      {
        if (!mixins.ContainsKey (mixinType))
        {
          var context = MixinContextObjectMother.Create (mixinType: mixinType);
          mixins.Add (context.MixinType, context);
        }
        else
        {
          string message = string.Format ("The mixin type {0} was tried to be added twice.", mixinType.FullName);
          throw new ArgumentException (message, "mixinTypes");
        }
      }
      return mixins.Values;
    }

  }
}