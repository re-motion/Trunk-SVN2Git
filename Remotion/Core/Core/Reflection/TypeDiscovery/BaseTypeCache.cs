using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery
{
  public sealed class BaseTypeCache
  {
    public static BaseTypeCache Create (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      var classCache = new Dictionary<Type, List<Type>> (MemberInfoEqualityComparer<Type>.Instance);
      var interfaceCache = new Dictionary<Type, List<Type>> (MemberInfoEqualityComparer<Type>.Instance);

      foreach (var type in types)
      {
        if (type.IsInterface)
        {
          if (!interfaceCache.ContainsKey (type))
            interfaceCache.Add (type, new List<Type>());
        }
        else
        {
          foreach (var baseType in type.CreateSequence (t => t.BaseType))
          {
            if (AssemblyTypeCache.IsGacAssembly (baseType.Assembly))
              break;

            List<Type> derivedTypes;
            if (!classCache.TryGetValue (baseType, out derivedTypes))
            {
              derivedTypes = new List<Type>();
              classCache.Add (baseType, derivedTypes);
            }
            derivedTypes.Add (type);
          }
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
          if (AssemblyTypeCache.IsGacAssembly (interfaceType.Assembly))
            break;

          List<Type> interfaceImplementations;
          if (!interfaceCache.TryGetValue (interfaceType, out interfaceImplementations))
          {
            interfaceImplementations = new List<Type>();
            interfaceCache.Add (interfaceType, interfaceImplementations);
          }
          interfaceImplementations.Add (type);
        }
      }

      return new BaseTypeCache (classCache, interfaceCache);
    }

    private readonly Dictionary<Type, List<Type>> _classCache = new Dictionary<Type, List<Type>> (MemberInfoEqualityComparer<Type>.Instance);

    private readonly Dictionary<Type, List<Type>> _interfaceCache = new Dictionary<Type, List<Type>> (MemberInfoEqualityComparer<Type>.Instance);

    private BaseTypeCache (Dictionary<Type, List<Type>> classCache, Dictionary<Type, List<Type>> interfaceCache)
    {
      _classCache = classCache;
      _interfaceCache = interfaceCache;
    }

    public bool IsCachePopulated ()
    {
      return _classCache.Any() || _interfaceCache.Any();
    }

    public ICollection GetAllTypesFromCache ()
    {
      return _classCache.Keys.Concat (_interfaceCache.Keys).ToArray();
    }

    public ICollection GetFromCache (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      if (baseType == typeof (object))
        return GetAllTypesFromCache();

      var cache = baseType.IsInterface ? _interfaceCache : _classCache;

      List<Type> types;
      if (cache.TryGetValue (baseType, out types))
        return types;

      return new Type[0];
    }
  }
}