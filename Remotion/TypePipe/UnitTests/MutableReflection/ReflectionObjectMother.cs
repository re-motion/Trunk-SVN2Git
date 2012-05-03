// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  public static class ReflectionObjectMother
  {
    private static readonly Random s_random = new Random();

    private static readonly Type[] s_types = EnsureNoNulls (new[] { typeof (DateTime), typeof (string) });
    private static readonly Type[] s_otherTypes = EnsureNoNulls (new[] { typeof (Random), typeof (int) });
    private static readonly Type[] s_unsealedTypes = EnsureNoNulls (new[] { typeof (object), typeof (List<int>) });
    private static readonly Type[] s_interfaceTypes = EnsureNoNulls (new[] { typeof (IDisposable), typeof (IServiceProvider) });
    private static readonly Type[] s_otherInterfaceTypes = EnsureNoNulls (new[] { typeof (IComparable), typeof (ICloneable) });
    private static readonly MemberInfo[] s_members = EnsureNoNulls (new MemberInfo[] { typeof (DateTime).GetProperty ("Now"), typeof (string).GetMethod ("get_Length") });
    private static readonly FieldInfo[] s_fields = EnsureNoNulls (new[] { typeof (string).GetField ("Empty"), typeof (Type).GetField ("EmptyTypes") });
    private static readonly ConstructorInfo[] s_defaultCtors = EnsureNoNulls (new[] { typeof (object).GetConstructor (Type.EmptyTypes), typeof (List<int>).GetConstructor (Type.EmptyTypes) });
    private static readonly MethodInfo[] s_virtualMethods = EnsureNoNulls (new[] { typeof (object).GetMethod ("ToString"), typeof(object).GetMethod("GetHashCode") });
    private static readonly MethodInfo[] s_nonVirtualMethods = EnsureNoNulls (new[] { typeof (object).GetMethod ("ReferenceEquals"), typeof (string).GetMethod ("Concat", new[] { typeof (object) }) });
    private static readonly MethodInfo[] s_finalMethods = EnsureNoNulls (new[] { typeof(DomainType).GetMethod("FinalMethod") });
    private static readonly MethodInfo[] s_nonGenericMethods = EnsureNoNulls (new[] { typeof (object).GetMethod ("ToString"), typeof (string).GetMethod ("Concat", new[] { typeof (object) }) });
    private static readonly MethodInfo[] s_genericMethods = EnsureNoNulls (new[] { typeof (Enumerable).GetMethod ("Empty"), typeof (ReflectionObjectMother).GetMethod ("GetRandomElement", BindingFlags.NonPublic | BindingFlags.Static) });
    private static readonly MethodInfo[] s_modifiableMethodInfos = EnsureNoNulls (new[] { typeof (object).GetMethod ("ToString"), typeof (object).GetMethod ("Equals", new[] { typeof (object ) }) });
    private static readonly ParameterInfo[] s_parameterInfos = EnsureNoNulls (typeof (Dictionary<,>).GetMethod ("TryGetValue").GetParameters());

    public static Type GetSomeType ()
    {
      return GetRandomElement (s_types);
    }

    public static Type GetSomeDifferentType ()
    {
      return GetRandomElement (s_otherTypes);
    }

    public static Type GetSomeSubclassableType ()
    {
      return GetRandomElement (s_unsealedTypes);
    }

    public static Type GetSomeInterfaceType ()
    {
      return GetRandomElement (s_interfaceTypes);
    }

    public static Type GetSomeDifferentInterfaceType ()
    {
      return GetRandomElement (s_otherInterfaceTypes);
    }

    public static MemberInfo GetSomeMember ()
    {
      return GetRandomElement (s_members);
    }

    public static FieldInfo GetSomeField ()
    {
      return GetRandomElement (s_fields);
    }

    public static ConstructorInfo GetSomeDefaultConstructor ()
    {
      return GetRandomElement (s_defaultCtors);
    }

    public static ConstructorInfo GetSomeConstructor ()
    {
      return GetSomeDefaultConstructor();
    }

    public static MethodInfo GetSomeMethod ()
    {
      return GetRandomElement (
          s_nonGenericMethods
              .Concat(s_genericMethods)
              .Concat(s_virtualMethods)
              .Concat(s_nonVirtualMethods)
              .Concat(s_finalMethods)
              .ToArray());
    }

    public static MethodInfo GetSomeVirtualMethod ()
    {
      var method = GetRandomElement (s_virtualMethods);
      Assertion.IsTrue (method.IsVirtual);
      return method;
    }

    public static MethodInfo GetSomeNonVirtualMethod ()
    {
      var method = GetRandomElement (s_nonVirtualMethods);
      Assertion.IsFalse (method.IsVirtual);
      return method;
    }

    public static MethodInfo GetSomeFinalMethod ()
    {
      var method = GetRandomElement (s_finalMethods);
      Assertion.IsTrue (method.IsFinal);
      return method;
    }

    public static MethodInfo GetSomeNonGenericMethod ()
    {
      var method = GetRandomElement (s_nonGenericMethods);
      Assertion.IsFalse (method.IsGenericMethod);
      return method;
    }

    public static MethodInfo GetSomeGenericMethod ()
    {
      var method = GetRandomElement (s_genericMethods);
      Assertion.IsTrue (method.IsGenericMethod);
      return method;
    }

    public static MethodInfo GetSomeModifiableMethod ()
    {
      return GetRandomElement (s_modifiableMethodInfos);
    }

    public static MethodInfo[] GetMultipeMethods (int count)
    {
      var result = s_nonGenericMethods.Take (count).ToArray();
      Assertion.IsTrue (result.Length == count, "Count must be at most {0} (or add elements to s_methodInfos).", s_nonGenericMethods.Length);
      return result;
    }

    public static ParameterInfo GetSomeParameter ()
    {
      return GetRandomElement (s_parameterInfos);
    }

    public static object GetDefaultValue (Type type)
    {
      return type.IsValueType ? Activator.CreateInstance (type) : null;
    }

    private static T GetRandomElement<T> (T[] array)
    {
      var index = s_random.Next (array.Length);
      return array[index];
    }

    private static T[] EnsureNoNulls<T> (T[] items)
    {
      foreach (var item in items)
        Assertion.IsNotNull (item);
      return items;
    }

    private class DomainTypeBase
    {
      public virtual void FinalMethod () { }
    }

    private class DomainType : DomainTypeBase
    {
      public sealed override void FinalMethod () { }
    }
  }
}