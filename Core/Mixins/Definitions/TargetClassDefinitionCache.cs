using System;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Utilities.Singleton;
using Remotion.Collections;
using Remotion.Mixins.Context;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.Definitions
{
  public class TargetClassDefinitionCache : ThreadSafeSingletonBase<TargetClassDefinitionCache, DefaultInstanceCreator<TargetClassDefinitionCache>>
  {
    // This doesn't hold any state and can thus safely be used from multiple threads at the same time
    private static readonly TargetClassDefinitionBuilder s_definitionBuilder = new TargetClassDefinitionBuilder();

    // We manually implement a lazy, thread-safe cache here so that multiple threads can use the cache at the same time with good performance
    private readonly object _syncObject = new object();
    private readonly Cache<ClassContext, TargetClassDefinition> _cache = new Cache<ClassContext, TargetClassDefinition> ();

    public bool IsCached (ClassContext context)
    {
      lock (_syncObject)
      {
        TargetClassDefinition dummy;
        return _cache.TryGetValue (context, out dummy);
      }
    }

    public TargetClassDefinition GetTargetClassDefinition (ClassContext context)
    {
      // We could simply do the following:
      //   return _cache.GetOrCreateValue (context, s_definitionBuilder.Build);
      // from, within a lock statement. However, this would cause the whole cache to be locked while just one definition is created.
      // We therefore risk creating definition objects twice (in the rare case of two threads simultaneously asking for uncached definitions for the
      // same contexts) and optimize for the more common case (threads concurrently asking for definitions for different contexts).

      TargetClassDefinition definition;
      lock (_syncObject)
      {
        _cache.TryGetValue (context, out definition);
      }

      if (definition == null)
      {
        definition = s_definitionBuilder.Build (context);
        Validate (definition);
        lock (_syncObject)
        {
          definition = _cache.GetOrCreateValue (context, delegate { return definition; });
        }
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