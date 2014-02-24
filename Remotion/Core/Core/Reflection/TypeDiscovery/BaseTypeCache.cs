using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery
{
  public class BaseTypeCache
  {
    private readonly Dictionary<Type, List<Type>> _classCache =
        new Dictionary<Type, List<Type>> (MemberInfoEqualityComparer<Type>.Instance);

    private readonly Dictionary<Type, List<Type>> _interfaceCache =
        new Dictionary<Type, List<Type>> (MemberInfoEqualityComparer<Type>.Instance);

    public BaseTypeCache ()
    {
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
      if (baseType == typeof (object))
        return GetAllTypesFromCache();

      var cache = baseType.IsInterface ? _interfaceCache : _classCache;

      List<Type> types;
      if (cache.TryGetValue (baseType, out types))
        return types;

      return new Type[0];
    }

    public BaseTypeCache BuildCaches (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);

      foreach (var type in types)
      {
        if (type.IsInterface)
        {
          if (!_interfaceCache.ContainsKey (type))
            _interfaceCache.Add (type, new List<Type>());
        }
        else
        {
          foreach (var baseType in type.CreateSequence (t => t.BaseType))
          {
            if (IsGacAssembly (baseType))
              break;

            List<Type> derivedTypes;
            if (!_classCache.TryGetValue (baseType, out derivedTypes))
            {
              derivedTypes = new List<Type>();
              _classCache.Add (baseType, derivedTypes);
            }
            derivedTypes.Add (type);
          }
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
          if (IsGacAssembly (interfaceType))
            break;

          List<Type> interfaceImplementations;
          if (!_interfaceCache.TryGetValue (interfaceType, out interfaceImplementations))
          {
            interfaceImplementations = new List<Type>();
            _interfaceCache.Add (interfaceType, interfaceImplementations);
          }
          interfaceImplementations.Add (type);
        }
      }

      return this;
    }

    private bool IsGacAssembly (Type type)
    {
      return AssemblyTypeCache.IsGacAssembly (type.Assembly);
    }
  }
}