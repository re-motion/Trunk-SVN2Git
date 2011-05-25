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
  public class TypeAdapter : ITypeInformation
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



    public override string ToString ()
    {
      return _type.ToString ();
    }

    private ITypeInformation[] ConvertToTypeAdapters (Type[] types)
    {
      return Array.ConvertAll (types, t => (ITypeInformation) TypeAdapter.Create (t));
    }

    /*


    /// <summary>
    /// Determines whether the class represented by the current <see cref="Type"/> derives from the class represented by the specified <see cref="Type"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the Type represented by the <paramref name="c"/> parameter and the current Type represent classes, and the class represented by the current Type derives from the class represented by <paramref name="c"/>; otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if <paramref name="c"/> and the current Type represent the same class.
    /// </returns>
    /// <param name="c">The Type to compare with the current Type. 
    ///                 </param><exception cref="ArgumentNullException">The <paramref name="c"/> parameter is <see langword="null"/>. 
    ///                 </exception><filterpriority>2</filterpriority>
    public bool IsSubclassOf (Type c)
    {
      return _type.IsSubclassOf (c);
    }

    /// <summary>
    /// Determines whether the specified object is an instance of the current <see cref="Type"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current Type is in the inheritance hierarchy of the object represented by <paramref name="o"/>, or if the current Type is an interface that <paramref name="o"/> supports. <see langword="false"/> if neither of these conditions is the case, or if <paramref name="o"/> is <see langword="null"/>, or if the current Type is an open generic type (that is, <see cref="P:System.Type.ContainsGenericParameters"/> returns <see langword="true"/>).
    /// </returns>
    /// <param name="o">The object to compare with the current Type. 
    ///                 </param><filterpriority>2</filterpriority>
    public bool IsInstanceOfType (object o)
    {
      return _type.IsInstanceOfType (o);
    }


     * 

     * 
    /// <summary>
    /// Determines whether an instance of the current <see cref="Type"/> can be assigned from an instance of the specified Type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="c"/> and the current Type represent the same type, or if the current Type is in the inheritance hierarchy of <paramref name="c"/>, or if the current Type is an interface that <paramref name="c"/> implements, or if <paramref name="c"/> is a generic type parameter and the current Type represents one of the constraints of <paramref name="c"/>. <see langword="false"/> if none of these conditions are <see langword="true"/>, or if <paramref name="c"/> is <see langword="null"/>.
    /// </returns>
    /// <param name="c">The Type to compare with the current Type. 
    ///                 </param><filterpriority>2</filterpriority>
    public bool IsAssignableFrom (Type c)
    {
      return _type.IsAssignableFrom (c);
    }

    /// <summary>
    /// When overridden in a derived class, indicates whether one or more instance of <paramref name="attributeType"/> is applied to this member.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if one or more instance of <paramref name="attributeType"/> is applied to this member; otherwise <see langword="false"/>.
    /// </returns>
    /// <param name="attributeType">The Type object to which the custom attributes are applied. 
    ///                 </param><param name="inherit">Specifies whether to search this member's inheritance chain to find the attributes. 
    ///                 </param>
    public bool IsDefined (Type attributeType, bool inherit)
    {
      return _type.IsDefined (attributeType, inherit);
    }

    /// <summary>
    /// When overridden in a derived class, returns an array of custom attributes identified by <see cref="Type"/>.
    /// </summary>
    /// <returns>
    /// An array of custom attributes applied to this member, or an array with zero (0) elements if no attributes have been applied.
    /// </returns>
    /// <param name="attributeType">The type of attribute to search for. Only attributes that are assignable to this type are returned. 
    ///                 </param><param name="inherit">Specifies whether to search this member's inheritance chain to find the attributes. 
    ///                 </param><exception cref="TypeLoadException">A custom attribute type cannot be loaded. 
    ///                 </exception><exception cref="ArgumentNullException">If <paramref name="attributeType"/> is <see langword="null"/>.
    ///                 </exception><exception cref="InvalidOperationException">This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.
    ///                 </exception>
    public object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      return _type.GetCustomAttributes (attributeType, inherit);
    }

        public override Type DeclaringType { get; }
    public abstract Type BaseType { get; }

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