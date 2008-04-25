using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions.Building;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MemberChainTest
  {
    class Base
    {
      public virtual void Foo () { }
      public virtual int FooP { get { return 0; } set { } }
      public virtual event EventHandler FooE;
    }

    class Derived1 : Base
    {
      public new virtual void Foo () { }
      public new virtual int FooP { get { return 0; } set { } }
      public new virtual event EventHandler FooE;
    }

    class Derived2 : Derived1
    {
      public override void Foo () { }
      public override int FooP { get { return 0; } set { } }
      public override event EventHandler FooE;
    }

    class Derived3 : Derived2
    {
      public override void Foo () { }
      public override int FooP { get { return 0; } set { } }
      public override event EventHandler FooE;
    }

    class Derived4 : Derived3
    {
      public new void Foo () { }
      public new int FooP { get { return 0; } set { } }
      public new event EventHandler FooE;
    }

    class Derived5 : Derived4
    {
      public new virtual void Foo () { }
      public new virtual int FooP { get { return 0; } set { } }
      public new virtual event EventHandler FooE;
    }

    class Derived6 : Derived5
    {
      public override void Foo () { }
      public override int FooP { get { return 0; } set { } }
      public override event EventHandler FooE;
    }

    class Derived7 : Derived6
    {
      public override void Foo () { }
      public override int FooP { get { return 0; } set { } }
      public override event EventHandler FooE;
    }

    private static List<MethodInfo> GetMethods ()
    {
      List<MethodInfo> methods = new List<MethodInfo> ();
      methods.Add (typeof (Base).GetMethod ("Foo"));
      methods.Add (typeof (Derived1).GetMethod ("Foo"));
      methods.Add (typeof (Derived2).GetMethod ("Foo"));
      methods.Add (typeof (Derived3).GetMethod ("Foo"));
      methods.Add (typeof (Derived4).GetMethod ("Foo"));
      methods.Add (typeof (Derived5).GetMethod ("Foo"));
      methods.Add (typeof (Derived6).GetMethod ("Foo"));
      methods.Add (typeof (Derived7).GetMethod ("Foo"));
      return methods;
    }

    private static List<PropertyInfo> GetProperties ()
    {
      List<PropertyInfo> properties = new List<PropertyInfo> ();
      properties.Add (typeof (Base).GetProperty ("FooP"));
      properties.Add (typeof (Derived1).GetProperty ("FooP"));
      properties.Add (typeof (Derived2).GetProperty ("FooP"));
      properties.Add (typeof (Derived3).GetProperty ("FooP"));
      properties.Add (typeof (Derived4).GetProperty ("FooP"));
      properties.Add (typeof (Derived5).GetProperty ("FooP"));
      properties.Add (typeof (Derived6).GetProperty ("FooP"));
      properties.Add (typeof (Derived7).GetProperty ("FooP"));
      return properties;
    }

    private static List<EventInfo> GetEvents ()
    {
      List<EventInfo> events = new List<EventInfo> ();
      events.Add (typeof (Base).GetEvent ("FooE"));
      events.Add (typeof (Derived1).GetEvent ("FooE"));
      events.Add (typeof (Derived2).GetEvent ("FooE"));
      events.Add (typeof (Derived3).GetEvent ("FooE"));
      events.Add (typeof (Derived4).GetEvent ("FooE"));
      events.Add (typeof (Derived5).GetEvent ("FooE"));
      events.Add (typeof (Derived6).GetEvent ("FooE"));
      events.Add (typeof (Derived7).GetEvent ("FooE"));
      return events;
    }


    private static void MemberChainSortsMembers<T>(List<T> source) where T : MemberInfo
    {
      List<T> members = new List<T> ();
      members.Add (source[4]);
      members.Add (source[1]);
      members.Add (source[7]);
      members.Add (source[5]);
      members.Add (source[3]);
      members.Add (source[0]);
      members.Add (source[2]);
      members.Add (source[6]);

      MemberChain<T> chain = new MemberChain<T> (members);

      List<T> sortedMethods = new List<T> (chain.GetSortedMembers());
      Assert.AreEqual (8, sortedMethods.Count);
      for (int i = 0; i < source.Count; ++i)
        Assert.AreSame (source[i], sortedMethods[i]);
    }

    private static void IsOverridden<T> (List<T> source) where T : MemberInfo
    {
      MemberChain<T> chain = new MemberChain<T> (source);
      Assert.IsFalse (chain.IsOverridden (source[0]));
      Assert.IsTrue (chain.IsOverridden (source[1]));
      Assert.IsTrue (chain.IsOverridden (source[2]));
      Assert.IsFalse (chain.IsOverridden (source[3]));
      Assert.IsFalse (chain.IsOverridden (source[4]));
      Assert.IsTrue (chain.IsOverridden (source[5]));
      Assert.IsTrue (chain.IsOverridden (source[6]));
      Assert.IsFalse (chain.IsOverridden (source[7]));
    }

    private void GetNonOverridden<T> (List<T> source) where T : MemberInfo
    {
      MemberChain<T> chain = new MemberChain<T> (source);
      List<T> nonOverridden = new List<T> (chain.GetNonOverriddenMembers ());
      Assert.AreEqual (4, nonOverridden.Count);
      Assert.AreSame (nonOverridden[0], source[0]);
      Assert.AreSame (nonOverridden[1], source[3]);
      Assert.AreSame (nonOverridden[2], source[4]);
      Assert.AreSame (nonOverridden[3], source[7]);
    }

    [Test]
    public void MemberChainSortsMethods()
    {
      MemberChainSortsMembers (GetMethods());
    }

    [Test]
    public void MemberChainSortsProperties()
    {
      MemberChainSortsMembers (GetProperties());
    }

    [Test]
    public void MemberChainSortsEvents()
    {
      MemberChainSortsMembers (GetEvents());
    }

    [Test]
    public void IsOverriddenMethods()
    {
      IsOverridden (GetMethods());
    }

    [Test]
    public void IsOverriddenProperties ()
    {
      IsOverridden (GetProperties());
    }

    [Test]
    public void IsOverriddenEvents()
    {
      IsOverridden (GetEvents());
    }

    [Test]
    public void GetNonOverriddenMethods()
    {
      GetNonOverridden (GetMethods());
    }

    [Test]
    public void GetNonOverriddenProperties()
    {
      GetNonOverridden (GetProperties());
    }

    [Test]
    public void GetNonOverriddenEvents()
    {
      GetNonOverridden (GetEvents());
    }
  }
}