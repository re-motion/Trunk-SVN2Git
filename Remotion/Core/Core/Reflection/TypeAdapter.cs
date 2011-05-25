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

    /// <summary>
    /// Gets the name of the current member.
    /// </summary>
    /// <returns>
    /// A <see cref="String"/> containing the name of this member.
    /// </returns>
    public string Name
    {
      get { return _type.Name; }
    }

    /// <summary>
    /// Gets the fully qualified name of the <see cref="Type"/>, including the namespace of the <see cref="Type"/> but not the assembly.
    /// </summary>
    /// <returns>
    /// The fully qualified name of the <see cref="Type"/>, including the namespace of the <see cref="Type"/> but not the assembly; 
    /// or <see langword="null"/> if the current instance represents a generic type parameter, an array type, pointer type, 
    /// or byref type based on a type parameter, or a generic type that is not a generic type definition but contains unresolved type parameters.
    /// </returns>
    public string FullName
    {
      get { return _type.FullName; }
    }

    /// <summary>
    /// Gets the namespace of the <see cref="Type"/>.
    /// </summary>
    /// <returns>
    /// The namespace of the <see cref="Type"/>, or <see langword="null"/> if the current instance represents a generic parameter.
    /// </returns>
    public string Namespace
    {
      get { return _type.Namespace; }
    }

    /// <summary>
    /// Gets the assembly-qualified name of the <see cref="Type"/>, which includes the name of the assembly from which the <see cref="Type"/> was loaded.
    /// </summary>
    /// <returns>
    /// The assembly-qualified name of the <see cref="Type"/>, which includes the name of the assembly from which the <see cref="Type"/> was loaded, or <see langword="null"/> if the current instance represents a generic type parameter.
    /// </returns>
    public string AssemblyQualifiedName
    {
      get { return _type.AssemblyQualifiedName; }
    }

    /// <summary>
    /// Gets the <see cref="System.Reflection.Assembly"/> in which the type is declared. For generic types, gets the <see cref="System.Reflection.Assembly"/> in which the generic type is defined.
    /// </summary>
    /// <returns>
    /// An <see cref="System.Reflection.Assembly"/> instance that describes the assembly containing the current type. For generic types, the instance describes the assembly that contains the generic type definition, not the assembly that creates and uses a particular constructed type.
    /// </returns>
    public Assembly Assembly
    {
      get { return _type.Assembly; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is a class; that is, not a value type or interface.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is a class; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsClass
    {
      get { return _type.IsClass; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is a value type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is a value type; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsValueType
    {
      get { return _type.IsValueType; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is an interface; that is, not a class or a value type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is an interface; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsInterface
    {
      get { return _type.IsInterface; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is an array.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is an array; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsArray
    {
      get { return _type.IsArray; }
    }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> represents an enumeration.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current <see cref="Type"/> represents an enumeration; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEnum
    {
      get { return _type.IsEnum; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is a pointer.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is a pointer; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsPointer
    {
      get { return _type.IsPointer; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is passed by reference.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is passed by reference; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsByRef
    {
      get { return _type.IsByRef; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is declared sealed.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is declared sealed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsSealed
    {
      get { return _type.IsSealed; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is abstract and must be overridden.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is abstract; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsAbstract
    {
      get { return _type.IsAbstract; }
    }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> object represents a type whose definition is nested inside the definition of another type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is nested inside another type; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsNested
    {
      get { return _type.IsNested; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Type"/> is serializable.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is serializable; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsSerializable
    {
      get { return _type.IsSerializable; }
    }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> encompasses or refers to another type; 
    /// that is, whether the current <see cref="Type"/> is an array, a pointer, or is passed by reference.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> is an array, a pointer, or is passed by reference; otherwise, <see langword="false"/>.
    /// </returns>
    public bool HasElementType
    {
      get { return _type.HasElementType; }
    }

    /// <summary>
    /// When overridden in a derived class, returns the <see cref="Type"/> of the object encompassed or referred to by the current array, pointer or reference type.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> of the object encompassed or referred to by the current array, pointer, or reference type, 
    /// or <see langword="null"/> if the current <see cref="Type"/> is not an array or a pointer, or is not passed by reference, 
    /// or represents a generic type or a type parameter in the definition of a generic type or generic method.
    /// </returns>
    public ITypeInformation GetElementType ()
    {
      return Maybe.ForValue (_type.GetElementType()).Select (TypeAdapter.Create).ValueOrDefault();
    }

    /// <summary>
    /// Gets a value indicating whether the current type is a generic type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current type is a generic type; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsGenericType
    {
      get { return _type.IsGenericType; }
    }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> represents a generic type definition, from which other generic types can be constructed.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> object represents a generic type definition; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsGenericTypeDefinition
    {
      get { return _type.IsGenericTypeDefinition; }
    }

    /// <summary>
    /// Returns a <see cref="Type"/> object that represents a generic type definition from which the current generic type can be constructed.
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/> object representing a generic type from which the current type can be constructed.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current type is not a generic type. That is, <see cref="IsGenericType"/> returns <see langword="false"/>.</exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
    public ITypeInformation GetGenericTypeDefinition ()
    {
      return TypeAdapter.Create (_type.GetGenericTypeDefinition());
    }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> object has type parameters that have not been replaced by specific types.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> object is itself a generic type parameter or has type parameters for which specific types have not been supplied; otherwise, <see langword="false"/>.
    /// </returns>
    public bool ContainsGenericParameters
    {
      get { return _type.ContainsGenericParameters; }
    }

    /// <summary>
    /// Returns an array of <see cref="Type"/> objects that represent the type arguments of a generic type or the type parameters of a generic type definition.
    /// </summary>
    /// <returns>
    /// An array of <see cref="Type"/> objects that represent the type arguments of a generic type. Returns an empty array if the current type is not a generic type.
    /// </returns>
    public ITypeInformation[] GetGenericArguments ()
    {
      return ConvertToTypeAdapters(_type.GetGenericArguments());
    }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Type"/> represents a type parameter in the definition of a generic type or method.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Type"/> object represents a type parameter of a generic type definition or generic method definition; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsGenericParameter
    {
      get { return _type.IsGenericParameter; }
    }

    /// <summary>
    /// Gets the position of the type parameter in the type parameter list of the generic type or method that declared the parameter, 
    /// when the <see cref="Type"/> object represents a type parameter of a generic type or a generic method.
    /// </summary>
    /// <returns>
    /// The position of a type parameter in the type parameter list of the generic type or method that defines the parameter. Position numbers begin at 0.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current type does not represent a type parameter. That is, <see cref="IsGenericParameter"/> returns <see langword="false"/>.</exception>
    public int GenericParameterPosition
    {
      get { return _type.GenericParameterPosition; }
    }

    /// <summary>
    /// Returns an array of <see cref="Type"/> objects that represent the constraints on the current generic type parameter. 
    /// </summary>
    /// <returns>
    /// An array of <see cref="Type"/> objects that represent the constraints on the current generic type parameter.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current <see cref="Type"/> object is not a generic type parameter. That is, the <see cref="IsGenericParameter"/> property returns <see langword="false"/>.</exception>
    public ITypeInformation[] GetGenericParameterConstraints ()
    {
      return ConvertToTypeAdapters (_type.GetGenericParameterConstraints ());
    }

    /// <summary>
    /// Gets a combination of <see cref="GenericParameterAttributes"/> flags that describe the covariance and special constraints of the current generic type parameter. 
    /// </summary>
    /// <returns>
    /// A bitwise combination of <see cref="GenericParameterAttributes"/> values that describes the covariance and special constraints of the current generic type parameter.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current <see cref="Type"/> object is not a generic type parameter. That is, the <see cref="IsGenericParameter"/> property returns <see langword="false"/>.</exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
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

     * public virtual Type MakeByRefType ();
    public virtual Type MakePointerType ();
    public virtual Type MakeArrayType ();
    public virtual Type MakeArrayType (int rank);
        public virtual int GetArrayRank ();
    public abstract Type[] GetInterfaces ();
    public virtual InterfaceMapping GetInterfaceMap (Type interfaceType);

     */
  }
}