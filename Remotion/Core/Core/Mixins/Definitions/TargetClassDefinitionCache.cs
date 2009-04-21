// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Utilities.Singleton;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.Definitions
{
  public class TargetClassDefinitionCache : ThreadSafeSingletonBase<TargetClassDefinitionCache, DefaultInstanceCreator<TargetClassDefinitionCache>>, ITargetClassDefinitionCache
  {
    // This doesn't hold any state and can thus safely be used from multiple threads at the same time
    private static readonly TargetClassDefinitionBuilder s_definitionBuilder = new TargetClassDefinitionBuilder();

    private readonly InterlockedCache<ClassContext, TargetClassDefinition> _cache = new InterlockedCache<ClassContext, TargetClassDefinition> ();

    public bool IsCached (ClassContext context)
    {
      TargetClassDefinition dummy;
      return _cache.TryGetValue (context, out dummy);
    }

    public TargetClassDefinition GetTargetClassDefinition (ClassContext context)
    {
      // Because the cache is interlocked, we could simply do the following:
      //   return _cache.GetOrCreateValue (context, s_definitionBuilder.Build);
      // However, this would cause the whole cache to be locked while just one definition is created.
      // We therefore risk creating definition objects twice (in the rare case of two threads simultaneously asking for uncached definitions for the
      // same contexts) and optimize for the more common case (threads concurrently asking for definitions for different contexts).

      TargetClassDefinition definition;
      if (!_cache.TryGetValue (context, out definition))
      {
        var newDefinition = s_definitionBuilder.Build (context);
        Validate (newDefinition);
        definition = _cache.GetOrCreateValue (context, delegate { return newDefinition; });
      }
      return definition;
    }

    private void Validate (TargetClassDefinition definition)
    {
      DefaultValidationLog log = Validator.Validate (definition);
      if (log.GetNumberOfFailures () > 0 || log.GetNumberOfUnexpectedExceptions () > 0)
        throw new ValidationException (log);
    }
  }
}
