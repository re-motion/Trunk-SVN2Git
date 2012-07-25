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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class CustomTypeTest
  {
    private IMemberSelector _memberSelector;

    private Type _underlyingSystemType;
    private Type _baseType;
    private TypeAttributes _typeAttributes;
    private string _name;
    private string _namespace;
    private string _fullName;

    private CustomTypeStub _customType;

    [SetUp]
    public void SetUp ()
    {
      _memberSelector = MockRepository.GenerateStrictMock<IMemberSelector>();

      _underlyingSystemType = ReflectionObjectMother.GetSomeType();
      _baseType = ReflectionObjectMother.GetSomeType();
      _typeAttributes = (TypeAttributes) 777;
      _name = "type name";
      _namespace = "namespace";
      _fullName = "full type name";

      _customType = new CustomTypeStub (_memberSelector, _underlyingSystemType, _baseType, _typeAttributes, _name, _namespace, _fullName);
    }

    [Test]
    public void Initialization_Null ()
    {
      Type baseType = null;
      new CustomTypeStub (_memberSelector, _underlyingSystemType, baseType, _typeAttributes, _name, _namespace, _fullName);
    }

    [Test]
    public void Assembly ()
    {
      Assert.That (_customType.Assembly, Is.Null);
    }

    [Test]
    public void Module ()
    {
      Assert.That (_customType.Module, Is.Null);
    }

    [Test]
    public void UnderlyingSystemType ()
    {
      Assert.That (_underlyingSystemType, Is.Not.Null);

      Assert.That (_customType.UnderlyingSystemType, Is.SameAs (_underlyingSystemType));
    }

    [Test]
    public void BaseType ()
    {
      Assert.That (_baseType, Is.Not.Null);

      Assert.That (_customType.BaseType, Is.SameAs (_baseType));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_name, Is.Not.Null.And.Not.Empty);

      Assert.That (_customType.Name, Is.EqualTo (_name));
    }

    [Test]
    public void Namespace ()
    {
      Assert.That (_namespace, Is.Not.Null.And.Not.Empty);

      Assert.That (_customType.Namespace, Is.EqualTo (_namespace));
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_fullName, Is.Not.Null.And.Not.Empty);

      Assert.That (_customType.FullName, Is.EqualTo (_fullName));
    }

    [Test]
    public void GetInterfaces ()
    {
      var interfaces = new[] { typeof (IDisposable), typeof (IComparable) };
      _customType.AllInterfaces = interfaces;

      Assert.That (_customType.GetInterfaces(), Is.EqualTo (interfaces));
    }

    [Test]
    public void GetInterface_NoMatch ()
    {
      _customType.AllInterfaces = new[] { typeof (IDisposable) };

      var result = _customType.GetInterface ("IComparable", false);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetInterface_CaseSensitive_NoMatch ()
    {
      _customType.AllInterfaces = new[] { typeof (IDisposable) };

      var result = _customType.GetInterface ("idisposable", false);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetInterface_CaseSensitive ()
    {
      _customType.AllInterfaces = new[] { typeof (IDisposable) };

      var result = _customType.GetInterface ("IDisposable", false);

      Assert.That (result, Is.SameAs (typeof (IDisposable)));
    }

    [Test]
    public void GetInterface_IgnoreCase ()
    {
      _customType.AllInterfaces = new[] { typeof (IDisposable) };

      var result = _customType.GetInterface ("idisposable", true);

      Assert.That (result, Is.SameAs (typeof (IDisposable)));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Ambiguous interface name 'idisposable'.")]
    public void GetInterface_IgnoreCase_Ambiguous ()
    {
      _customType.AllInterfaces = new[] { typeof (IDisposable), typeof (Idisposable) };

      _customType.GetInterface ("idisposable", true);
    }

    [Test]
    public new void ToString ()
    {
      Assert.That (_customType.ToString (), Is.EqualTo ("type name"));
    }

    [Test]
    public void ToDebugString ()
    {
      var typeName = _customType.GetType().Name;
      Assert.That (_customType.ToDebugString (), Is.EqualTo (typeName + " = \"type name\""));
    }

    [Test]
    public void GetElementType ()
    {
      Assert.That (_customType.GetElementType (), Is.Null);
    }

    [Test]
    public void HasElementTypeImpl ()
    {
      Assert.That (_customType.HasElementType, Is.False);
    }

    [Test]
    public void GetAttributeFlagsImpl ()
    {
      Assert.That (_customType.Attributes, Is.EqualTo (_typeAttributes));
    }

    [Test]
    public void IsByRefImpl ()
    {
      Assert.That (_customType.IsByRef, Is.False);
    }

    [Test]
    public void IsArrayImpl ()
    {
      Assert.That (_customType.IsArray, Is.False);
    }

    [Test]
    public void IsPointerImpl ()
    {
      Assert.That (_customType.IsPointer, Is.False);
    }

    [Test]
    public void IsPrimitiveImpl ()
    {
      Assert.That (_customType.IsPrimitive, Is.False);
    }

    [Test]
    public void IsCOMObjectImpl ()
    {
      Assert.That (_customType.IsCOMObject, Is.False);
    }

    [Test]
    public void UnsupportedMembers ()
    {
      CheckThrowsNotSupported (() => Dev.Null = _customType.MetadataToken, "Property", "MetadataToken");
      CheckThrowsNotSupported (() => Dev.Null = _customType.GUID, "Property", "GUID");
      CheckThrowsNotSupported (() => Dev.Null = _customType.AssemblyQualifiedName, "Property", "AssemblyQualifiedName");
      CheckThrowsNotSupported (() => Dev.Null = _customType.StructLayoutAttribute, "Property", "StructLayoutAttribute");
      CheckThrowsNotSupported (() => Dev.Null = _customType.GenericParameterAttributes, "Property", "GenericParameterAttributes");
      CheckThrowsNotSupported (() => Dev.Null = _customType.GenericParameterPosition, "Property", "GenericParameterPosition");
      CheckThrowsNotSupported (() => Dev.Null = _customType.TypeHandle, "Property", "TypeHandle");

      CheckThrowsNotSupported (() => _customType.GetDefaultMembers (), "Method", "GetDefaultMembers");
      CheckThrowsNotSupported (() => _customType.GetInterfaceMap (null), "Method", "GetInterfaceMap");
      CheckThrowsNotSupported (() => _customType.InvokeMember (null, 0, null, null, null), "Method", "InvokeMember");
      CheckThrowsNotSupported (() => _customType.MakePointerType (), "Method", "MakePointerType");
      CheckThrowsNotSupported (() => _customType.MakeByRefType (), "Method", "MakeByRefType");
      CheckThrowsNotSupported (() => _customType.MakeArrayType (), "Method", "MakeArrayType");
      CheckThrowsNotSupported (() => _customType.MakeArrayType (7), "Method", "MakeArrayType");
      CheckThrowsNotSupported (() => _customType.GetArrayRank (), "Method", "GetArrayRank");
      CheckThrowsNotSupported (() => _customType.GetGenericParameterConstraints (), "Method", "GetGenericParameterConstraints");
      CheckThrowsNotSupported (() => _customType.MakeGenericType (), "Method", "MakeGenericType");
      CheckThrowsNotSupported (() => _customType.GetGenericArguments (), "Method", "GetGenericArguments");
      CheckThrowsNotSupported (() => _customType.GetGenericTypeDefinition (), "Method", "GetGenericTypeDefinition");
    }

    private void CheckThrowsNotSupported (TestDelegate memberInvocation, string memberType, string memberName)
    {
      var message = string.Format ("{0} {1} is not supported.", memberType, memberName);
      Assert.That (memberInvocation, Throws.TypeOf<NotSupportedException> ().With.Message.EqualTo (message));
    }

    // This exists for GetInterface method with ignore case parameter.
    private interface Idisposable
    {
    }
  }
}