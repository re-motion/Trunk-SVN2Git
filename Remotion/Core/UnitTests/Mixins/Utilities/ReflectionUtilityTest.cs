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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Mixins.Samples;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.TestDomain;
using Remotion.UnitTests.Mixins.Utilities.TestDomain.AssociatedMethods;
using Rhino.Mocks;

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

    [Test]
    public void GetAssociatedMethods_MethodInfo ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetMethod ("Method");
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EqualTo (new[] { member }));
    }

    [Test]
    public void GetAssociatedMethods_PropertyInfo ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetProperty ("Property");
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetGetMethod (), member.GetSetMethod () }));
    }

    [Test]
    public void GetAssociatedMethods_PropertyInfo_Protected ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetProperty ("ProtectedProperty", BindingFlags.NonPublic | BindingFlags.Instance);
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetGetMethod (true), member.GetSetMethod (true) }));
    }

    [Test]
    public void GetAssociatedMethods_EventInfo ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetEvent ("Event");
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetAddMethod(), member.GetRemoveMethod () }));
    }

    [Test]
    public void GetAssociatedMethods_EventInfo_Protected ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetEvent ("ProtectedEvent", BindingFlags.NonPublic | BindingFlags.Instance);
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetAddMethod (true), member.GetRemoveMethod (true) }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Associated methods can only be retrieved for methods, properties, and events.")]
    public void GetAssociatedMethods_InvalidMemberInfoKind ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetConstructor (Type.EmptyTypes);
      ReflectionUtility.GetAssociatedMethods (member);
    }

    [Test]
    public void IsReachableFromSignedAssembly_True ()
    {
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (object)), Is.True);
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (EquatableMixin<>)), Is.True);
    }

    [Test]
    public void IsReachableFromSignedAssembly_False ()
    {
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (NullMixin)), Is.False);
    }

    [Test]
    public void IsReachableFromSignedAssembly_False_GenericArgument ()
    {
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (EquatableMixin<NullMixin>)), Is.False);
    }

    [Test]
    public void IsRangeReachableFromSignedAssembly_True ()
    {
      Assert.That (ReflectionUtility.IsRangeReachableFromSignedAssembly (new[] { typeof (object), typeof (EquatableMixin<>) }), Is.True);
    }

    [Test]
    public void IsRangeReachableFromSignedAssembly_False ()
    {
      Assert.That (ReflectionUtility.IsRangeReachableFromSignedAssembly (new[] { typeof (object), typeof (EquatableMixin<NullMixin>) }), Is.False);
    }
  }
}
