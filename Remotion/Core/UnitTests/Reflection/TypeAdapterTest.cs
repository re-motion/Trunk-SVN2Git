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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class TypeAdapterTest
  {
    private Type _type;
    private TypeAdapter _adapter;

    [SetUp]
    public void SetUp ()
    {
      var typeAdapterDataStore =
          (IDataStore<Type, TypeAdapter>) PrivateInvoke.GetNonPublicStaticField (typeof (TypeAdapter), "s_dataStore");
      typeAdapterDataStore.Clear ();

      _type = typeof (List);
      _adapter = TypeAdapter.Create (_type);
    }

    [Test]
    public void Create_ReturnsSameInstance ()
    {
      Assert.That (TypeAdapter.Create (_type), Is.SameAs (TypeAdapter.Create (_type)));
    }

    [Test]
    public void Type ()
    {
      Assert.That (_adapter.Type, Is.SameAs (_type));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_adapter.Name, Is.EqualTo(_type.Name));
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_adapter.FullName, Is.EqualTo (_type.FullName));
    }

    [Test]
    public void Namespace ()
    {
      Assert.That (_adapter.Namespace, Is.EqualTo (_type.Namespace));
    }
    
    [Test]
    public void AssemblyQualifiedName ()
    {
      Assert.That (_adapter.AssemblyQualifiedName, Is.EqualTo (_type.AssemblyQualifiedName));
    }
    
    [Test]
    public void Assembly ()
    {
      Assert.That (_adapter.Assembly, Is.SameAs(_type.Assembly));
    }

    [Test]
    public void DeclaringType_NestedType ()
    {
      var type = typeof (Environment.SpecialFolder);
      Assert.That (TypeAdapter.Create (type).DeclaringType, Is.TypeOf<TypeAdapter> ().And.Property ("Type").SameAs (type.DeclaringType));
    }

    [Test]
    public void DeclaringType_NotNestedType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).DeclaringType, Is.Null);
    }

    [Test]
    public void GetOriginalDeclaringType_NestedType ()
    {
      var type = typeof (Environment.SpecialFolder);
      //Assert.That (TypeAdapter.Create (type).GetOriginalDeclaration (), Is.TypeOf<TypeAdapter> ().And.Property ("Type").SameAs (type.DeclaringType));
      Assert.That (TypeAdapter.Create (type).GetOriginalDeclaringType (), Is.SameAs (type.DeclaringType));
    }

    [Test]
    public void GetOriginalDeclaringType_NotNestedType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).GetOriginalDeclaringType(), Is.Null);
    }

    [Test]
    public void IsClass_ReferenceType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsClass, Is.EqualTo (type.IsClass).And.True);
    }

    [Test]
    public void IsClass_ValueType ()
    {
      var type = typeof (Guid);
      Assert.That (TypeAdapter.Create (type).IsClass, Is.EqualTo (type.IsClass).And.False);
    }

    [Test]
    public void IsInterface_Interface ()
    {
      var type = typeof (IList<>);
      Assert.That (TypeAdapter.Create (type).IsInterface, Is.EqualTo (type.IsInterface).And.True);
    }

    [Test]
    public void IsInterface_ReferenceType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsInterface, Is.EqualTo (type.IsInterface).And.False);
    }

    [Test]
    public void IsValueType_ReferenceType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsValueType, Is.EqualTo (type.IsValueType).And.False);
    }

    [Test]
    public void IsValueType_ValueType ()
    {
      var type = typeof (TestDomain.Struct);
      Assert.That (TypeAdapter.Create (type).IsValueType, Is.EqualTo (type.IsValueType).And.True);
    }

    [Test]
    public void IsArray_Array ()
    {
      var type = typeof (object[]);
      Assert.That (TypeAdapter.Create (type).IsArray, Is.EqualTo (type.IsArray).And.True);
    }

    [Test]
    public void IsArray_NotArray ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsArray, Is.EqualTo (type.IsArray).And.False);
    }

    [Test]
    public void GetArrayRank_Array ()
    {
      var type = typeof (object[,]);
      Assert.That (TypeAdapter.Create (type).GetArrayRank (), Is.EqualTo (type.GetArrayRank ()).And.EqualTo (2));
    }

    [Test]
    public void GetArrayRank_NotArray ()
    {
      var type = typeof (List);
      Assert.That (()=>TypeAdapter.Create (type).GetArrayRank(), Throws.ArgumentException);
    }

    [Test]
    public void MakeArrayType_WithRank ()
    {
      var type = typeof (object);
      Assert.That (
          TypeAdapter.Create (type).MakeArrayType (2),
          Is.TypeOf<TypeAdapter>().And.Property ("Type").SameAs (type.MakeArrayType (2)));
    }

    [Test]
    public void MakeArrayType_WithInvalidRank ()
    {
      Assert.That (() => TypeAdapter.Create (typeof (object)).MakeArrayType (0), Throws.TypeOf<IndexOutOfRangeException>());
    }

    [Test]
    public void MakeArrayType_WithoutRank ()
    {
      var type = typeof (object);
      Assert.That (
          TypeAdapter.Create (type).MakeArrayType (),
          Is.TypeOf<TypeAdapter> ().And.Property ("Type").SameAs (type.MakeArrayType ()));
    }

    [Test]
    public void IsEnum_Enum ()
    {
      var type = typeof (System.Reflection.MemberTypes);
      Assert.That (TypeAdapter.Create (type).IsEnum, Is.EqualTo (type.IsEnum).And.True);
    }

    [Test]
    public void IsEnum_NotEnum ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsEnum, Is.EqualTo (type.IsEnum).And.False);
    }

    [Test]
    public void IsPointer_Pointer ()
    {
      var type = typeof (int).MakePointerType();
      Assert.That (TypeAdapter.Create (type).IsPointer, Is.EqualTo (type.IsPointer).And.True);
    }

    [Test]
    public void IsPointer_NotPointer ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsPointer, Is.EqualTo (type.IsPointer).And.False);
    }

    [Test]
    public void MakePointerType ()
    {
      var type = typeof (int);
      Assert.That (
          TypeAdapter.Create (type).MakePointerType(),
          Is.TypeOf<TypeAdapter>().And.Property ("Type").SameAs (type.MakePointerType()));
    }

    [Test]
    public void IsByRef_ByRef ()
    {
      var type = typeof (int).MakeByRefType();
      Assert.That (TypeAdapter.Create (type).IsByRef, Is.EqualTo (type.IsByRef).And.True);
    }

    [Test]
    public void IsByRef_NotByRef ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsByRef, Is.EqualTo (type.IsByRef).And.False);
    }

    [Test]
    public void MakeByRefType ()
    {
      var type = typeof (int);
      Assert.That (
          TypeAdapter.Create (type).MakeByRefType (),
          Is.TypeOf<TypeAdapter> ().And.Property ("Type").SameAs (type.MakeByRefType ()));
    }

    [Test]
    public void IsSealed_SealedType ()
    {
      var type = typeof (Activator);
      Assert.That (TypeAdapter.Create (type).IsSealed, Is.EqualTo (type.IsSealed).And.True);
    }

    [Test]
    public void IsSealed_NotSealedType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsSealed, Is.EqualTo (type.IsSealed).And.False);
    }

    [Test]
    public void IsAbstract_AbstractType ()
    {
      var type = typeof (Array);
      Assert.That (TypeAdapter.Create (type).IsAbstract, Is.EqualTo (type.IsAbstract).And.True);
    }

    [Test]
    public void IsAbstract_NotAbstractType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsAbstract, Is.EqualTo (type.IsAbstract).And.False);
    }

    [Test]
    public void IsNested_NestedType ()
    {
      var type = typeof (Environment.SpecialFolder);
      Assert.That (TypeAdapter.Create (type).IsNested, Is.EqualTo (type.IsNested).And.True);
    }

    [Test]
    public void IsNested_NotNestedType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsNested, Is.EqualTo (type.IsNested).And.False);
    }

    [Test]
    public void IsSerializable_SerializableType ()
    {
      var type = typeof (Exception);
      Assert.That (TypeAdapter.Create (type).IsSerializable, Is.EqualTo (type.IsSerializable).And.True);
    }

    [Test]
    public void IsSerializable_NotSerializableType ()
    {
      var type = typeof (Activator);
      Assert.That (TypeAdapter.Create (type).IsSerializable, Is.EqualTo (type.IsSerializable).And.False);
    }

    [Test]
    public void HasElementType_TypeWithElementType ()
    {
      var type = typeof (int[]);
      Assert.That (TypeAdapter.Create (type).HasElementType, Is.EqualTo (type.HasElementType).And.True);
    }

    [Test]
    public void HasElementType_TypeWithoutElementType ()
    {
      var type = typeof (int);
      Assert.That (TypeAdapter.Create (type).HasElementType, Is.EqualTo (type.HasElementType).And.False);
    }

    [Test]
    public void GetElementType_TypeWithElementType ()
    {
      var type = typeof (int[]);
      Assert.That (
          TypeAdapter.Create (type).GetElementType(),
          Is.TypeOf<TypeAdapter>().And.Property ("Type").SameAs (type.GetElementType()));
    }

    [Test]
    public void GetElementType_TypeWithoutElementType ()
    {
      var type = typeof (int);
      Assert.That (TypeAdapter.Create (type).GetElementType(), Is.EqualTo (type.GetElementType()).And.Null);
    }

    [Test]
    public void IsGenericType_ClosedGenericType ()
    {
      var type = typeof (List<int>);
      Assert.That (TypeAdapter.Create (type).IsGenericType, Is.EqualTo (type.IsGenericType).And.True);
    }

    [Test]
    public void IsGenericType_NotGenericType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsGenericType, Is.EqualTo (type.IsGenericType).And.False);
    }

    [Test]
    public void IsGenericTypeDefinition_GenericTypeDefinition ()
    {
      var type = typeof (List<>);
      Assert.That (TypeAdapter.Create (type).IsGenericTypeDefinition, Is.EqualTo (type.IsGenericTypeDefinition).And.True);
    }

    [Test]
    public void IsGenericTypeDefinition_NotGenericTypeDefinition ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).IsGenericTypeDefinition, Is.EqualTo (type.IsGenericTypeDefinition).And.False);
    }

    [Test]
    public void GetGenericTypeDefintion_ClosedGenericType ()
    {
      var type = typeof (List<int>);
      Assert.That (
          TypeAdapter.Create (type).GetGenericTypeDefinition(),
          Is.TypeOf<TypeAdapter>().And.Property ("Type").SameAs (type.GetGenericTypeDefinition()));
    }

    [Test]
    public void GetGenericTypeDefintion_GenericTypeDefinition()
    {
      var type = typeof (List<>);
      Assert.That (
          TypeAdapter.Create (type).GetGenericTypeDefinition (),
          Is.TypeOf<TypeAdapter> ().And.Property ("Type").SameAs (type.GetGenericTypeDefinition ()));
    }

    [Test]
    public void GetGenericTypeDefintion_NotGenericType ()
    {
      Assert.That (()=>
          TypeAdapter.Create (typeof (List)).GetGenericTypeDefinition (),
          Throws.InvalidOperationException);
    }

    [Test]
    public void ContainsGenericParameters_OpenGenericType ()
    {
      var type = typeof (List<>);
      Assert.That (TypeAdapter.Create (type).ContainsGenericParameters, Is.EqualTo (type.ContainsGenericParameters).And.True);
    }

    [Test]
    public void ContainsGenericParameters_ClosedGenericType ()
    {
      var type = typeof (List<int>);
      Assert.That (TypeAdapter.Create (type).ContainsGenericParameters, Is.EqualTo (type.ContainsGenericParameters).And.False);
    }

    [Test]
    public void ContainsGenericParameters_NotGenericType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).ContainsGenericParameters, Is.EqualTo (type.ContainsGenericParameters).And.False);
    }

    [Test]
    public void GetGenericArguments_ClosedGenericType ()
    {
      var type = typeof (List<int>);
      Assert.That (TypeAdapter.Create (type).GetGenericArguments(), Has.All.TypeOf<TypeAdapter>());
      Assert.That (TypeAdapter.Create (type).GetGenericArguments().Cast<TypeAdapter>().Select (ta => ta.Type), Is.EqualTo (new[] { typeof (int) }));
    }

    [Test]
    public void GetGenericArguments_NotGenericType ()
    {
      var type = typeof (List);
      Assert.That (TypeAdapter.Create (type).GetGenericArguments (), Is.Empty);
    }

    [Test]
    public void IsGenericParameter_GenericParameter ()
    {
      var type = typeof (Nullable<>).GetGenericArguments ().Single ();

      Assert.That (TypeAdapter.Create (type).IsGenericParameter, Is.EqualTo (type.IsGenericParameter).And.True);
    }

    [Test]
    public void IsGenericParameter_NotGenericParameter ()
    {
      var type = typeof (ValueType);

      Assert.That (TypeAdapter.Create (type).IsGenericParameter, Is.EqualTo (type.IsGenericParameter).And.False);
    }

    [Test]
    public void GetGenericParameterConstraints_GenericParameter ()
    {
      var type = typeof (Nullable<>).GetGenericArguments().Single();

      Assert.That (TypeAdapter.Create (type).GetGenericParameterConstraints(), Has.All.TypeOf<TypeAdapter>());
      Assert.That (
          TypeAdapter.Create (type).GetGenericParameterConstraints().Cast<TypeAdapter>().Select (ta => ta.Type),
          Is.EqualTo (new[] { typeof (ValueType) }));
    }

    [Test]
    public void GetGenericParameterConstraints_NotGenericParameter ()
    {
      Assert.That (() =>
          TypeAdapter.Create (typeof (ValueType)).GetGenericTypeDefinition (),
          Throws.InvalidOperationException);
    }

    [Test]
    public void GenericParameterPosition_GenericParameter ()
    {
      var type = typeof (Tuple<,>).GetGenericArguments()[1];

      Assert.That (TypeAdapter.Create (type).GenericParameterPosition, Is.EqualTo (type.GenericParameterPosition).And.EqualTo (1));
    }

    [Test]
    public void GenericParameterPosition_NotGenericParameter ()
    {
      Assert.That (() =>
          TypeAdapter.Create (typeof (ValueType)).GenericParameterPosition,
          Throws.InvalidOperationException);
    }

    [Test]
    public void GenericParameterAttributes_GenericParameter ()
    {
      var type = typeof (Nullable<>).GetGenericArguments().Single();

      Assert.That (TypeAdapter.Create (type).GenericParameterAttributes, Is.EqualTo (type.GenericParameterAttributes));
    }

    [Test]
    public void GenericParameterAttributes_NotGenericParameter ()
    {
      Assert.That (() =>
          TypeAdapter.Create (typeof (ValueType)).GenericParameterAttributes,
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetCustomAttribute ()
    {
      var type = typeof (DateTime);
      var adapter = TypeAdapter.Create (type);
      Assert.That (
          adapter.GetCustomAttribute<SerializableAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttribute<SerializableAttribute> (type, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var type = typeof (DateTime);
      var adapter = TypeAdapter.Create (type);
      Assert.That (
          adapter.GetCustomAttributes<SerializableAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttributes<SerializableAttribute> (type, true)));
    }

    [Test]
    public void IsDefined ()
    {
      var type = typeof (DateTime);
      var adapter = TypeAdapter.Create (type);
      Assert.That (
          adapter.IsDefined<SerializableAttribute> (true),
          Is.EqualTo (AttributeUtility.IsDefined<SerializableAttribute> (type, true)));
    }


  }
}