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
using System.Linq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class MemberInfoFromExpressionUtilityTest
  {
    [Test]
    public void GetMember_Static_MemberExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember (() => DomainType.StaticField);

      var expected = typeof (DomainType).GetMember ("StaticField").Single();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Static_MethodCallExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember (() => DomainType.StaticMethod ());

      var expected = typeof (DomainType).GetMember ("StaticMethod").Single();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Static_NewExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember (() => new DomainType());

      var expected = typeof (DomainType).GetMember (".ctor").Single();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Must be a MemberExpression, MethodCallExpression or NewExpression.\r\nParameter name: expression")]
    public void GetMember_Static_InvalidExpression ()
    {
      MemberInfoFromExpressionUtility.GetMember (() => 1);
    }

    [Test]
    public void GetMember_Instance_MemberExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.InstanceProperty);

      var expected = typeof (DomainType).GetMember ("InstanceProperty").Single();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Instance_MethodCallExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.InstanceMethod ());

      var expected = typeof (DomainType).GetMember ("InstanceMethod").Single();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Instance_NewExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => new DomainType());

      var expected = typeof (DomainType).GetMember (".ctor").Single();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Must be a MemberExpression, MethodCallExpression or NewExpression.\r\nParameter name: expression")]
    public void GetMember_Instance_InvalidExpression ()
    {
      MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => 1);
    }

    [Test]
    public void GetField_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetField (() => DomainType.StaticField);

      var expected = typeof (DomainType).GetField ("StaticField");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetField_Static_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetField (() => DomainType.StaticMethod ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a field access expression.\r\nParameter name: expression")]
    public void GetField_Static_NonField ()
    {
      MemberInfoFromExpressionUtility.GetField (() => DomainType.StaticProperty);
    }

    [Test]
    public void GetField_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.InstanceField);

      var expected = typeof (DomainType).GetField ("InstanceField");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetField_Instance_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.InstanceMethod());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a field access expression.\r\nParameter name: expression")]
    public void GetField_Instance_NonField ()
    {
      MemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.InstanceProperty);
    }

    [Test]
    public void GetConstructor ()
    {
      var member = MemberInfoFromExpressionUtility.GetConstructor (() => new DomainType());

      var expected = typeof (DomainType).GetConstructor (Type.EmptyTypes);
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a NewExpression.\r\nParameter name: expression")]
    public void GetConstructor_NonNewExpression ()
    {
      MemberInfoFromExpressionUtility.GetConstructor (() => DomainType.StaticField);
    }

    [Test]
    public void GetMethod_StaticVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticVoidMethod());

      var expected = typeof (DomainType).GetMethod ("StaticVoidMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticMethod());

      var expected = typeof (DomainType).GetMethod ("StaticMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetMethod_Static_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticProperty);
    }

    [Test]
    public void GetMethod_InstanceVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceVoidMethod());

      var expected = typeof (DomainType).GetMethod ("InstanceVoidMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceMethod());

      var expected = typeof (DomainType).GetMethod ("InstanceMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetMethod_Instance_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceProperty);
    }

    [Test]
    public void GetGenericMethodDefinition_StaticVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticVoidGenericMethod<Dev.T> (null));

      var expected = typeof (DomainType).GetMethod ("StaticVoidGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_StaticVoid_NonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticVoidMethod ());
    }

    [Test]
    public void GetGenericMethodDefinition_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticGenericMethod<Dev.T> (null));

      var expected = typeof (DomainType).GetMethod ("StaticGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Static_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticProperty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Static_NonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticMethod ());
    }

    [Test]
    public void GetGenericMethodDefinition_InstanceVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceVoidGenericMethod<Dev.T> (null));

      var expected = typeof (DomainType).GetMethod ("InstanceVoidGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_InstanceVoid_NonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceVoidMethod ());
    }

    [Test]
    public void GetGenericMethodDefinition_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceGenericMethod<Dev.T> (null));

      var expected = typeof (DomainType).GetMethod ("InstanceGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Instance_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceProperty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Instance_NonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceMethod ());
    }

    [Test]
    public void GetProperty_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty (() => DomainType.StaticProperty);

      var expected = typeof (DomainType).GetProperty ("StaticProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetProperty_Static_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetProperty (() => DomainType.StaticMethod());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a property access expression.\r\nParameter name: expression")]
    public void GetProperty_Static_NonProperty ()
    {
      MemberInfoFromExpressionUtility.GetProperty (() => DomainType.StaticField);
    }

    [Test]
    public void GetProperty_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.InstanceProperty);

      var expected = typeof (DomainType).GetProperty ("InstanceProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetProperty_Instance_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.InstanceMethod());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a property access expression.\r\nParameter name: expression")]
    public void GetProperty_Instance_NonProperty ()
    {
      MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.InstanceField);
    }

    public class DomainType
    {
      public static int StaticField;
      public int InstanceField;

      public DomainType ()
      {
        StaticField = 0;
        InstanceField = 0;
      }

      public static void StaticVoidMethod () { }
      public static int StaticMethod () { return 0; }
      public void InstanceVoidMethod () { }
      public int InstanceMethod () { return 0; }

      public static void StaticVoidGenericMethod<T> (T t) { }
      public static int StaticGenericMethod<T> (T t) { return 0; }
      public void InstanceVoidGenericMethod<T> (T t) { }
      public int InstanceGenericMethod<T> (T t) { return 0; }

      public static int StaticProperty { get; set; }
      public int InstanceProperty { get; set; }

      public static event Action SaticEvent;
      public event Action InstanceEvent;
    }
  }
}