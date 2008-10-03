/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
