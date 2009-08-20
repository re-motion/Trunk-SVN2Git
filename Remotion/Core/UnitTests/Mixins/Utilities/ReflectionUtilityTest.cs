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

    public class BlaAttribute : Attribute { }

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
