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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class ReflectionUtilityTest
  {
    class Base
    {
      public void Foo (int i) { }
      public virtual void Bar (int i) { }

      public int FooP { get { return 0; } set { } }
      public virtual int BarP { get { return 0; } set { } }

      public event Func<int> FooE;
      public virtual event Func<int> BarE;
    }

    class Derived : Base
    {
      public virtual new void Foo (int i) { }
      public override void Bar (int i) { }
      public void Baz (int i) { }

      public virtual new int FooP { get { return 0; } set { } }
      public override int BarP { get { return 0; } set { } }
      public int BazP { get { return 0; } set { } }

      public virtual new event Func<int> FooE;
      public override event Func<int> BarE;
      public event Func<int> BazE;
    }

    [Test]
    public void IsNewSlotMember()
    {
      Assert.IsTrue (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Foo")));
      Assert.IsTrue (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("FooP")));
      Assert.IsTrue (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("FooE")));

      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Bar")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("BarP")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("BarE")));

      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Baz")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("BazP")));
      Assert.IsFalse (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("BazE")));
    }

    [Test]
    public void IsVirtualMember ()
    {
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Foo")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("FooP")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("FooE")));

      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Bar")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("BarP")));
      Assert.IsTrue (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("BarE")));

      Assert.IsFalse (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Baz")));
      Assert.IsFalse (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("BazP")));
      Assert.IsFalse (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("BazE")));
    }

    [Test]
    public void GetMethodSignature()
    {
      Tuple<Type, Type[]> methodSignature = ReflectionUtility.GetMethodSignature (typeof (object).GetMethod ("ReferenceEquals"));
      Assert.AreEqual (typeof (bool), methodSignature.A);
      Assert.AreEqual (2, methodSignature.B.Length);
      Assert.AreEqual (typeof (object), methodSignature.B[0]);
      Assert.AreEqual (typeof (object), methodSignature.B[1]);
    }

    public class BlaAttribute : Attribute { }

    class C<T1, T2, [Bla]T3> : Mixin<T2>
        where T2 : class
    {
    }

    [Test]
    public void GetGenericArgumentsBoundToAttribute()
    {
      List<Type> arguments = new List<Type> (ReflectionUtility.GetGenericParametersAssociatedWithAttribute (typeof (C<,,>), typeof (BlaAttribute)));
      Assert.AreEqual (1, arguments.Count);
      Assert.IsNotNull (arguments.Find (delegate (Type arg) { return arg.Name == "T3"; }));

      Type thisAttribute = typeof (Mixin).Assembly.GetType ("Remotion.Mixins.ThisAttribute");
      arguments = new List<Type> (ReflectionUtility.GetGenericParametersAssociatedWithAttribute (typeof (C<,,>), thisAttribute));
      Assert.AreEqual (1, arguments.Count);
      Assert.IsNotNull (arguments.Find (delegate (Type arg) { return arg.Name == "T2"; }));
    }

    class GenericBase<T>
    {
      public virtual void Foo () { }
    }

    class GenericSub<T> : GenericBase<T>
    {
      public override void Foo () { }
    }

    class NonGenericSub : GenericSub<int>
    {
      public new void Foo () { }
    }

    [Test]
    public void IsSameTypeIgnoreGenerics ()
    {
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<>)));
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericBase<>), typeof (GenericBase<string>)));
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericBase<int>), typeof (GenericBase<string>)));

      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<>)));
      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<>), typeof (GenericBase<string>)));
      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<int>), typeof (GenericBase<string>)));

      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<>), typeof (object)));
      Assert.IsFalse (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (GenericSub<int>), typeof (object)));

      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (object), typeof (object)));
      Assert.IsTrue (ReflectionUtility.IsSameTypeIgnoreGenerics (typeof (object), typeof (object)));
    }

    interface IInterface
    {
      void Explicit ();
    }

    class ClassWithAllVisibilityMethods : IInterface
    {
      public void Public () { }
      protected void Protected () { }
      protected internal void ProtectedInternal () { }
      internal void Internal () { }
      private void Private () { }

      void IInterface.Explicit () { }
    }

    [Test]
    public void IsPublicOrProtected ()
    {
      BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      Assert.IsTrue (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Public", bf)));
      Assert.IsTrue (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Protected", bf)));
      Assert.IsTrue (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("ProtectedInternal", bf)));
      Assert.IsFalse (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Internal", bf)));
      Assert.IsFalse (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Private", bf)));
      Assert.IsFalse (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Remotion.UnitTests.Mixins.Utilities.ReflectionUtilityTest.IInterface.Explicit", bf)));
    }

    [Test]
    public void IsPublicOrProtectedOrExplicitInterface ()
    {
      BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      Assert.IsTrue (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Public", bf)));
      Assert.IsTrue (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Protected", bf)));
      Assert.IsTrue (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("ProtectedInternal", bf)));
      Assert.IsFalse (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Internal", bf)));
      Assert.IsFalse (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Private", bf)));
      Assert.IsTrue (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Remotion.UnitTests.Mixins.Utilities.ReflectionUtilityTest.IInterface.Explicit", bf)));
    }

    [Test]
    public void IsAssemblySigned_Assembly ()
    {
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (typeof (object).Assembly));
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (typeof (Uri).Assembly));
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (typeof (Mixin).Assembly));

      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (typeof (ReflectionUtilityTest).Assembly));
    }

    [Test]
    public void IsAssemblySigned_Name ()
    {
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (typeof (object).Assembly.GetName()));
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (typeof (Uri).Assembly.GetName ()));
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (typeof (Mixin).Assembly.GetName ()));

      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (typeof (ReflectionUtilityTest).Assembly.GetName ()));
    }

    [Test]
    public void IsAssemblySigned_StringName ()
    {
      string fullName = typeof (object).Assembly.FullName;
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (new AssemblyName (fullName)));

      fullName = typeof (ReflectionUtilityTest).Assembly.FullName;
      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (new AssemblyName (fullName)));
    }
  }
}
