// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="ITypeInformation"/> to wrap a <see cref="Type"/> instance.
  /// </summary>
  public sealed class TypeAdapter : ITypeInformation
  {
    private readonly Type _type;
    //If this is changed to an (expiring) cache, equals implementation must be updated.
    private static readonly IDataStore<Type, TypeAdapter> s_dataStore =
        new InterlockedDataStore<Type, TypeAdapter> (
            new SimpleDataStore<Type, TypeAdapter> (MemberInfoEqualityComparer<Type>.Instance));

    public static TypeAdapter Create (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return s_dataStore.GetOrCreateValue (type, t => new TypeAdapter (t));
    }

    private TypeAdapter (Type type)
    {
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return _type.Name; }
    }

    public string FullName
    {
      get { return _type.FullName; }
    }

    public string Namespace
    {
      get { return _type.Namespace; }
    }

    public string AssemblyQualifiedName
    {
      get { return _type.AssemblyQualifiedName; }
    }

    public Assembly Assembly
    {
      get { return _type.Assembly; }
    }

    Type IMemberInformation.DeclaringType
    {
      get { return _type.DeclaringType; }
    }

    public ITypeInformation DeclaringType
    {
      get { return Maybe.ForValue (_type.DeclaringType).Select (TypeAdapter.Create).ValueOrDefault (); }
    }

    public Type GetOriginalDeclaringType ()
    {
      return _type.DeclaringType;
    }

    public bool IsClass
    {
      get { return _type.IsClass; }
    }

    public bool IsValueType
    {
      get { return _type.IsValueType; }
    }

    public bool IsInterface
    {
      get { return _type.IsInterface; }
    }

    public bool IsArray
    {
      get { return _type.IsArray; }
    }

    public int GetArrayRank ()
    {
      return _type.GetArrayRank ();
    }

    public ITypeInformation MakeArrayType (int rank)
    {
      return TypeAdapter.Create (_type.MakeArrayType (rank));
    }

    public ITypeInformation MakeArrayType ()
    {
      return TypeAdapter.Create (_type.MakeArrayType ());
    }

    public bool IsEnum
    {
      get { return _type.IsEnum; }
    }

    public bool IsPointer
    {
      get { return _type.IsPointer; }
    }

    public ITypeInformation MakePointerType ()
    {
      return TypeAdapter.Create (_type.MakePointerType ());
    }

    public bool IsByRef
    {
      get { return _type.IsByRef; }
    }

    public ITypeInformation MakeByRefType ()
    {
      return TypeAdapter.Create (_type.MakeByRefType ());
    }

    public bool IsSealed
    {
      get { return _type.IsSealed; }
    }

    public bool IsAbstract
    {
      get { return _type.IsAbstract; }
    }

    public bool IsNested
    {
      get { return _type.IsNested; }
    }

    public bool IsSerializable
    {
      get { return _type.IsSerializable; }
    }

    public bool HasElementType
    {
      get { return _type.HasElementType; }
    }

    public ITypeInformation GetElementType ()
    {
      return Maybe.ForValue (_type.GetElementType()).Select (TypeAdapter.Create).ValueOrDefault();
    }

    public bool IsGenericType
    {
      get { return _type.IsGenericType; }
    }

    public bool IsGenericTypeDefinition
    {
      get { return _type.IsGenericTypeDefinition; }
    }

    public ITypeInformation GetGenericTypeDefinition ()
    {
      return TypeAdapter.Create (_type.GetGenericTypeDefinition());
    }

    public bool ContainsGenericParameters
    {
      get { return _type.ContainsGenericParameters; }
    }

    public ITypeInformation[] GetGenericArguments ()
    {
      return ConvertToTypeAdapters(_type.GetGenericArguments());
    }

    public bool IsGenericParameter
    {
      get { return _type.IsGenericParameter; }
    }

    public int GenericParameterPosition
    {
      get { return _type.GenericParameterPosition; }
    }

    public ITypeInformation[] GetGenericParameterConstraints ()
    {
      return ConvertToTypeAdapters (_type.GetGenericParameterConstraints ());
    }

    public GenericParameterAttributes GenericParameterAttributes
    {
      get { return _type.GenericParameterAttributes; }
    }


    public T GetCustomAttribute<T> (bool inherited) where T : class
    {
      return AttributeUtility.GetCustomAttribute<T> (_type, inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T : class
    {
      return AttributeUtility.GetCustomAttributes<T> (_type, inherited);
    }

    public bool IsDefined<T> (bool inherited) where T : class
    {
      return AttributeUtility.IsDefined<T> (_type, inherited);
    }

    /// <summary>
    /// Gets the type from which the current <see cref="Type"/> directly inherits.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> from which the current <see cref="Type"/> directly inherits, 
    /// or null if the current Type represents the <see cref="Object"/> class or an interface.
    /// </returns>
    public ITypeInformation BaseType
    {
      get { return Maybe.ForValue (_type.BaseType).Select (TypeAdapter.Create).ValueOrDefault(); }
    }

    /// <summary>
    /// Determines whether the specified object is an instance of the current <see cref="Type"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current Type is in the inheritance hierarchy of the object represented by <paramref name="o"/>, 
    /// or if the current Type is an interface that <paramref name="o"/> supports. <see langword="false"/> if neither of these conditions is the case, 
    /// or if <paramref name="o"/> is <see langword="null"/>, or if the current Type is an open generic type 
    /// (that is, <see cref="ContainsGenericParameters"/> returns <see langword="true"/>).
    /// </returns>
    /// <param name="o">The object to compare with the current Type. </param>
    public bool IsInstanceOfType (object o)
    {
      return _type.IsInstanceOfType (o);
    }

    /// <summary>
    /// Determines whether the class represented by the current <see cref="Type"/> derives from the class represented by the specified <see cref="Type"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the Type represented by the <paramref name="c"/> parameter and the current Type represent classes, 
    /// and the class represented by the current Type derives from the class represented by <paramref name="c"/>; otherwise, <see langword="false"/>. 
    /// This method also returns <see langword="false"/> if <paramref name="c"/> and the current Type represent the same class.
    /// In addition, the implementation of <paramref name="c"/> must match the implementation of this <see cref="ITypeInformation"/>,
    /// otherwise this method will also return <see langword="false" />.
    /// </returns>
    /// <param name="c">The Type to compare with the current Type. </param>
    /// <exception cref="ArgumentNullException">The <paramref name="c"/> parameter is <see langword="null"/>. </exception>
    public bool IsSubclassOf (ITypeInformation c)
    {
      ArgumentUtility.CheckNotNull ("c", c);

      var otherTypeAsTypeAdapter = c as TypeAdapter;
      if (otherTypeAsTypeAdapter == null)
        return false;

      return _type.IsSubclassOf (otherTypeAsTypeAdapter.Type);
    }

    /// <summary>
    /// Determines whether an instance of the current <see cref="Type"/> can be assigned from an instance of the specified Type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="c"/> and the current Type represent the same type, 
    /// or if the current Type is in the inheritance hierarchy of <paramref name="c"/>, 
    /// or if the current Type is an interface that <paramref name="c"/> implements, 
    /// or if <paramref name="c"/> is a generic type parameter and the current Type represents one of the constraints of <paramref name="c"/>. 
    /// <see langword="false"/> if none of these conditions are <see langword="true"/>, or if <paramref name="c"/> is <see langword="null"/>,
    /// or the implementation of <paramref name="c"/> does not match the implementation of this <see cref="ITypeInformation"/>.
    /// </returns>
    /// <param name="c">The Type to compare with the current Type. </param>
    public bool IsAssignableFrom (ITypeInformation c)
    {
      var otherTypeAsTypeAdapter = c as TypeAdapter;
      if (otherTypeAsTypeAdapter == null)
        return false;
      return _type.IsAssignableFrom (otherTypeAsTypeAdapter.Type);
    }

    public override string ToString ()
    {
      return _type.ToString ();
    }

    private ITypeInformation[] ConvertToTypeAdapters (Type[] types)
    {
      return Array.ConvertAll (types, t => (ITypeInformation) TypeAdapter.Create (t));
    }

    /*




     * 

     * 

     // use this.GetProperties {name.equals(value.name), returntype.equals(value.returntype)}
     public static PropertyInfo GetProperty (this IPropertyInformation
        string name, BindingFlags bindingAttr, Type returnType, Type[] types);
    public abstract PropertyInfo[] GetProperties (BindingFlags bindingAttr);
         // use this.GetProperties {name.equals(value.name), returntype.equals(value.returntype)}
     public static MethodInfo GetMethod (this IPropertyInformation
        string name, BindingFlags bindingAttr, Type returnType, Type[] types);
    public abstract MethodInfo[] GetMethods (BindingFlags bindingAttr);

    public abstract Type[] GetInterfaces ();
    public virtual InterfaceMapping GetInterfaceMap (Type interfaceType);

     */
  }
}