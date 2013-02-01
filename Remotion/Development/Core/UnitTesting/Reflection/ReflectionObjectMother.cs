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
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Reflection
{
  public static class ReflectionObjectMother
  {
    private static readonly Random s_random = new Random();

    private static readonly Type[] s_types = EnsureNoNulls (new[] { typeof (DateTime), typeof (Random) });
    private static readonly Type[] s_otherTypes = EnsureNoNulls (new[] { typeof (decimal), typeof (StringBuilder) });
    private static readonly Type[] s_serializableTypes = EnsureNoNulls (new[] { typeof (object), typeof (string), typeof(List<int>) });
    private static readonly Type[] s_unsealedTypes = EnsureNoNulls (new[] { typeof (object), typeof (List<int>) });
    private static readonly Type[] s_delegateTypes = EnsureNoNulls (new[] { typeof (EventHandler), typeof (Action<,,>) });
    private static readonly Type[] s_interfaceTypes = EnsureNoNulls (new[] { typeof (IDisposable), typeof (IServiceProvider) });
    private static readonly Type[] s_otherInterfaceTypes = EnsureNoNulls (new[] { typeof (IComparable), typeof (ICloneable) });
    private static readonly FieldInfo[] s_staticFields = EnsureNoNulls (new[] { typeof (string).GetField ("Empty"), typeof (Type).GetField ("EmptyTypes") });
    private static readonly FieldInfo[] s_instanceFields = EnsureNoNulls (new[] { typeof (DomainType).GetField ("Field") });
    private static readonly ConstructorInfo[] s_staticCtors = EnsureNoNulls (new[] { typeof (DomainType).TypeInitializer });
    private static readonly ConstructorInfo[] s_defaultCtors = EnsureNoNulls (new[] { typeof (object).GetConstructor (Type.EmptyTypes), typeof (List<int>).GetConstructor (Type.EmptyTypes) });
    private static readonly MethodInfo[] s_instanceMethod = EnsureNoNulls (new[] { typeof (object).GetMethod ("ToString"), typeof (object).GetMethod ("GetHashCode") });
    private static readonly MethodInfo[] s_staticMethod = EnsureNoNulls (new[] { typeof (object).GetMethod ("ReferenceEquals"), typeof (double).GetMethod ("IsNaN") });
    private static readonly MethodInfo[] s_virtualMethods = EnsureNoNulls (new[] { typeof (object).GetMethod ("ToString"), typeof(object).GetMethod("GetHashCode") });
    private static readonly MethodInfo[] s_nonVirtualMethods = EnsureNoNulls (new[] { typeof (object).GetMethod ("ReferenceEquals"), typeof (string).GetMethod ("Concat", new[] { typeof (object) }) });
    private static readonly MethodInfo[] s_nonVirtualInstanceMethods = EnsureNoNulls (new[] { typeof (object).GetMethod ("GetType"), typeof (string).GetMethod ("Contains", new[] { typeof (string) }) });
    private static readonly MethodInfo[] s_overridingMethods = EnsureNoNulls (new[] { typeof (DomainType).GetMethod ("Override"), typeof (DomainType).GetMethod ("ToString") });
    private static readonly MethodInfo[] s_finalMethods = EnsureNoNulls (new[] { typeof(DomainType).GetMethod("FinalMethod") });
    private static readonly MethodInfo[] s_nonGenericMethods = EnsureNoNulls (new[] { typeof (object).GetMethod ("ToString"), typeof (string).GetMethod ("Concat", new[] { typeof (object) }) });
    private static readonly MethodInfo[] s_genericMethods = EnsureNoNulls (new[] { typeof (Enumerable).GetMethod ("Empty"), typeof (ReflectionObjectMother).GetMethod ("GetRandomElement", BindingFlags.NonPublic | BindingFlags.Static) });
    private static readonly MethodInfo[] s_abstractMethodInfos = EnsureNoNulls (new[] { typeof (MethodInfo).GetMethod ("GetBaseDefinition"), typeof (Type).GetMethod ("GetMethods", new[] { typeof (BindingFlags) }) });
    private static readonly MethodInfo[] s_nonPublicMethodInfos = EnsureNoNulls (new[] { typeof (DomainType).GetMethod ("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance), typeof (DomainType).GetMethod ("ProtectedMethod", BindingFlags.NonPublic | BindingFlags.Instance) });
    private static readonly MethodInfo[] s_publicMethodInfos = EnsureNoNulls (new[] { typeof (DomainType).GetMethod ("FinalMethod"), typeof (DomainType).GetMethod("Override") });
    private static readonly ParameterInfo[] s_parameterInfos = EnsureNoNulls (typeof (Dictionary<,>).GetMethod ("TryGetValue").GetParameters());
    private static readonly PropertyInfo[] s_properties = EnsureNoNulls (new[] { typeof (List<>).GetProperty ("Count"), typeof (Type).GetProperty ("IsArray") });

    public static Type GetSomeType ()
    {
      return GetRandomElement (s_types);
    }

    public static Type GetSomeDifferentType ()
    {
      return GetRandomElement (s_otherTypes);
    }

    public static Type GetSomeSerializableType ()
    {
      return GetRandomElement (s_serializableTypes);
    }

    public static Type GetSomeSubclassableType ()
    {
      return GetRandomElement (s_unsealedTypes);
    }

    public static Type GetSomeDelegateType ()
    {
      return GetRandomElement (s_delegateTypes);
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
      return GetRandomElement (
          AllMethods.Cast<MemberInfo>()
              .Concat (s_instanceFields)
              .Concat (s_staticFields)
              .Concat (s_defaultCtors)
              .ToArray());
    }

    public static FieldInfo GetSomeField ()
    {
      return GetRandomElement (s_instanceFields);
    }

    public static FieldInfo GetSomeOtherField ()
    {
      return GetRandomElement (s_staticFields);
    }

    public static FieldInfo GetSomeInstanceField ()
    {
      var field = GetRandomElement (s_instanceFields);
      Assertion.IsFalse (field.IsStatic);
      return field;
    }

    public static FieldInfo GetSomeStaticField()
    {
      var field = GetRandomElement (s_staticFields);
      Assertion.IsTrue (field.IsStatic);
      return field;
    }

    public static ConstructorInfo GetSomeTypeInitializer()
    {
      var constructor = GetRandomElement (s_staticCtors);
      Assertion.IsTrue (constructor.IsStatic);
      return constructor;
    }

    public static ConstructorInfo GetSomeDefaultConstructor ()
    {
      return GetRandomElement (s_defaultCtors);
    }

    public static ConstructorInfo GetSomeConstructor ()
    {
      return GetSomeDefaultConstructor();
    }

    public static ConstructorInfo GetSomeOtherConstructor ()
    {
      return GetRandomElement (s_staticCtors);
    }

    public static MethodInfo GetSomeMethod ()
    {
      return GetRandomElement (AllMethods.Except(s_genericMethods).ToArray());
    }

    public static MethodInfo GetSomeOtherMethod ()
    {
      return GetRandomElement (s_genericMethods);
    }

    public static MethodInfo GetSomeInstanceMethod ()
    {
      var method = GetRandomElement (s_instanceMethod);
      Assertion.IsFalse (method.IsStatic);
      return method;
    }

    public static MethodInfo GetSomeStaticMethod ()
    {
      var method = GetRandomElement (s_staticMethod);
      Assertion.IsTrue (method.IsStatic);
      return method;
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

    public static MethodInfo GetSomeNonVirtualInstanceMethod ()
    {
      var method = GetRandomElement (s_nonVirtualInstanceMethods);
      Assertion.IsFalse (method.IsVirtual);
      Assertion.IsFalse (method.IsStatic);
      return method;
    }

    public static MethodInfo GetSomeAbstractMethod ()
    {
      var method = GetRandomElement (s_abstractMethodInfos);
      Assertion.IsTrue (method.IsAbstract);
      return method;
    }

    public static MethodInfo GetSomeConcreteMethod ()
    {
      var method = GetRandomElement (s_instanceMethod);
      Assertion.IsFalse (method.IsAbstract);
      return method;
    }

    public static MethodInfo GetSomeOverridingMethod ()
    {
      var method = GetRandomElement (s_overridingMethods);
      Assertion.IsTrue (method.GetBaseDefinition() != method);
      return method;
    }

    public static MethodInfo GetSomeBaseDefinition ()
    {
      var method = GetRandomElement (s_instanceMethod).GetBaseDefinition();
      Assertion.IsTrue (method == method.GetBaseDefinition());
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

    public static MethodInfo GetSomeNonPublicMethod ()
    {
      var method = GetRandomElement (s_nonPublicMethodInfos);
      Assertion.IsFalse (method.IsPublic);
      return method;
    }

    public static MethodInfo GetSomePublicMethod ()
    {
      var method = GetRandomElement (s_publicMethodInfos);
      Assertion.IsTrue (method.IsPublic);
      return method;
    }

    public static MethodInfo GetSomeGenericMethod ()
    {
      var method = GetRandomElement (s_genericMethods);
      Assertion.IsTrue (method.IsGenericMethod);
      return method;
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

    public static PropertyInfo GetSomeProperty ()
    {
      return GetRandomElement (s_properties);
    }

    public static object GetDefaultValue (Type type)
    {
      return type.IsValueType ? Activator.CreateInstance (type) : null;
    }

    private static IEnumerable<MethodInfo> AllMethods
    {
      get
      {
        return s_instanceMethod
            .Concat (s_staticMethod)
            .Concat (s_virtualMethods)
            .Concat (s_nonVirtualMethods)
            .Concat (s_nonVirtualInstanceMethods)
            .Concat (s_overridingMethods)
            .Concat (s_finalMethods)
            .Concat (s_nonGenericMethods)
            .Concat (s_genericMethods)
            .Concat (s_abstractMethodInfos);
      }
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

      public virtual void Override () { }
    }

    private class DomainType : DomainTypeBase
    {
      static DomainType () { }

      [UsedImplicitly] public int Field = 0;

      public sealed override void FinalMethod () { }

      public override void Override () { }
      public override string ToString () { return ""; }

      private void PrivateMethod () { }
      protected void ProtectedMethod () { }
    }
  }
}