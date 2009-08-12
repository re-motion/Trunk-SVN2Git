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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Serializes instances of <see cref="ConcreteMixinTypeIdentifier"/> into a <see cref="SerializationInfo"/> object. The serialization is
  /// completely flat, using only primitive types, so the returned object is always guaranteed to be complete even in the face of the order of 
  /// deserialization of objects not being deterministic.
  /// </summary>
  public class ConcreteMixinTypeIdentifierSerializer
  {
    public static void Serialize (ConcreteMixinTypeIdentifier identifier, SerializationInfo serializationInfo, string key)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("serializationInfo", serializationInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      serializationInfo.AddValue (key + ".MixinType", identifier.MixinType.AssemblyQualifiedName);

      SerializeMethods (serializationInfo, key + ".ExternalOverriders", (ICollection<MethodInfo>) identifier.ExternalOverriders, true);
      SerializeMethods (serializationInfo, key + ".WrappedProtectedMembers", (ICollection<MethodInfo>) identifier.WrappedProtectedMembers, true);
    }

    public static ConcreteMixinTypeIdentifier Deserialize (SerializationInfo serializationInfo, string key)
    {
      ArgumentUtility.CheckNotNull ("serializationInfo", serializationInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      var mixinType = Type.GetType (serializationInfo.GetString (key + ".MixinType"));
      HashSet<MethodInfo> externalOverriders = DeserializeMethods (serializationInfo, key + ".ExternalOverriders", null);
      HashSet<MethodInfo> wrappedProtectedMembers = DeserializeMethods (serializationInfo, key + ".WrappedProtectedMembers", null);

      return new ConcreteMixinTypeIdentifier (mixinType, externalOverriders, wrappedProtectedMembers);
    }

    private static void SerializeMethods (SerializationInfo serializationInfo, string key, ICollection<MethodInfo> collection, bool includeDeclaringType)
    {
      serializationInfo.AddValue (key + ".Count", collection.Count);

      var index = 0;
      foreach (var methodInfo in collection)
      {
        if (includeDeclaringType)
          serializationInfo.AddValue (key + "[" + index + "].DeclaringType", methodInfo.DeclaringType.AssemblyQualifiedName);

        serializationInfo.AddValue (key + "[" + index + "].MetadataToken", methodInfo.MetadataToken);

        ++index;
      }
    }

    private static HashSet<MethodInfo> DeserializeMethods (SerializationInfo serializationInfo, string key, Type declaringType)
    {
      var methods = new HashSet<MethodInfo> ();
      var count = serializationInfo.GetInt32 (key + ".Count");

      for (int i = 0; i < count; ++i)
      {
        var methodDeclaringType = declaringType ?? Type.GetType (serializationInfo.GetString (key + "[" + i + "].DeclaringType"));
        
        var method = (MethodInfo) methodDeclaringType.Module.ResolveMethod (serializationInfo.GetInt32 (key + "[" + i + "].MetadataToken"));
        if (methodDeclaringType.IsGenericType)
          method = (MethodInfo) MethodInfo.GetMethodFromHandle (method.MethodHandle, methodDeclaringType.TypeHandle);

        methods.Add (method);
      }
      return methods;
    }
  }
}