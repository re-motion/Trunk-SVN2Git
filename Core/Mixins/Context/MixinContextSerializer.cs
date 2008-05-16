using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Mixins.Utilities.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public static class MixinContextSerializer
  {
    public static void SerializeIntoFlatStructure (MixinContext mixinContext, string key, SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddValue (key + ".MixinKind", mixinContext.MixinKind);
      ReflectionObjectSerializer.SerializeType (mixinContext.MixinType, key + ".MixinType", info);

      info.AddValue (key + ".ExplicitDependencyCount", mixinContext.ExplicitDependencies.Count);
      IEnumerator<Type> dependencyEnumerator = mixinContext.ExplicitDependencies.GetEnumerator ();
      for (int i = 0; dependencyEnumerator.MoveNext (); ++i)
        ReflectionObjectSerializer.SerializeType (dependencyEnumerator.Current, key + ".ExplicitDependencies[" + i + "]", info);
    }

    public static MixinContext DeserializeFromFlatStructure (string key, SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("info", info);

      MixinKind mixinKind = (MixinKind) info.GetValue (key + ".MixinKind", typeof (MixinKind));
      Type mixinType = ReflectionObjectSerializer.DeserializeType (key + ".MixinType", info);

      int dependencyCount = info.GetInt32 (key + ".ExplicitDependencyCount");
      List<Type> explicitDependencies = new List<Type>();
      for (int i = 0; i < dependencyCount; ++i)
        explicitDependencies.Add (ReflectionObjectSerializer.DeserializeType (key + ".ExplicitDependencies[" + i + "]", info));

      MixinContext newContext = new MixinContext (mixinKind, mixinType, explicitDependencies);
      return newContext;
    }
  }
}