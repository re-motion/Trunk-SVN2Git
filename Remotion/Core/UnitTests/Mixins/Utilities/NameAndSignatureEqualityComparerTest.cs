// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class NameAndSignatureEqualityComparerTest
  {
    interface IGeneric<T>
    {
      T Method (T i);

      T Property { get; set; }

      event Func<T> Event1;
    }

    class ConcreteImplementer : IGeneric<string>
    {
      public string Method (string s)
      {
        throw new NotImplementedException();
      }

      public string Method (int i)
      {
        throw new NotImplementedException ();
      }

      public string Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      public event Func<string> Event1;
    }

    class GenericImplementer<T> : IGeneric<T>
    {
      public T Method (T i)
      {
        throw new NotImplementedException ();
      }

      public T Method2 (T i)
      {
        throw new NotImplementedException ();
      }

      public T Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      public T Property2
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      public event Func<T> Event1;
      public event Func<T> Event2;
    }

    [Test]
    public void EqualsForMethods()
    {
      MethodInfo m1 = typeof (IGeneric<>).GetMethod ("Method");
      MethodInfo m2 = typeof (IGeneric<string>).GetMethod ("Method");
      MethodInfo m3 = typeof (ConcreteImplementer).GetMethod ("Method", new Type[] {typeof (string)});
      MethodInfo m4 = typeof (ConcreteImplementer).GetMethod ("Method", new Type[] { typeof (int) });
      MethodInfo m5 = typeof (GenericImplementer<>).GetMethod ("Method");
      MethodInfo m6 = typeof (GenericImplementer<string>).GetMethod ("Method");

      IEqualityComparer<MethodInfo> comparer = new MethodNameAndSignatureEqualityComparer();

      Assert.IsTrue (comparer.Equals (null, null));
      Assert.IsFalse (comparer.Equals (null, m1));
      Assert.IsFalse (comparer.Equals (m1, null));

      Assert.IsTrue (comparer.Equals (m1, m1));
      Assert.IsFalse (comparer.Equals (m1, m2));
      Assert.IsFalse (comparer.Equals (m1, m3));
      Assert.IsTrue (comparer.Equals (m2, m3));
      Assert.IsFalse (comparer.Equals (m3, m4));
      Assert.IsFalse (comparer.Equals (m1, m5));
      Assert.IsFalse (comparer.Equals (m5, m6));
      Assert.IsTrue (comparer.Equals (m2, m6));
    }

    [Test]
    public void GetHashCodeForMethods ()
    {
      MethodInfo m1 = typeof (IGeneric<>).GetMethod ("Method");
      MethodInfo m2 = typeof (IGeneric<string>).GetMethod ("Method");
      MethodInfo m3 = typeof (ConcreteImplementer).GetMethod ("Method", new Type[] { typeof (string) });
      MethodInfo m4 = typeof (ConcreteImplementer).GetMethod ("Method", new Type[] { typeof (int) });
      MethodInfo m5 = typeof (GenericImplementer<>).GetMethod ("Method");
      MethodInfo m6 = typeof (GenericImplementer<string>).GetMethod ("Method");

      IEqualityComparer<MethodInfo> comparer = new MethodNameAndSignatureEqualityComparer ();

      Assert.AreEqual (comparer.GetHashCode (m1), comparer.GetHashCode (m1));
      Assert.AreNotEqual (comparer.GetHashCode(m1), comparer.GetHashCode(m2));
      Assert.AreNotEqual (comparer.GetHashCode(m1), comparer.GetHashCode(m3));
      Assert.AreEqual (comparer.GetHashCode(m2), comparer.GetHashCode(m3));
      Assert.AreNotEqual (comparer.GetHashCode(m3), comparer.GetHashCode(m4));
      Assert.AreEqual (comparer.GetHashCode(m1), comparer.GetHashCode(m5));
      Assert.AreNotEqual (comparer.GetHashCode(m5), comparer.GetHashCode(m6));
      Assert.AreEqual (comparer.GetHashCode (m2), comparer.GetHashCode (m6));
    }

    [Test]
    public void EqualsForProperties ()
    {
      PropertyInfo p1 = typeof (IGeneric<string>).GetProperty("Property");
      PropertyInfo p2 = typeof (GenericImplementer<string>).GetProperty ("Property");
      PropertyInfo p3 = typeof (GenericImplementer<string>).GetProperty ("Property2");
      PropertyInfo p4 = typeof (ConcreteImplementer).GetProperty ("Property");

      PropertyInfo p5 = typeof (IGeneric<>).GetProperty ("Property");
      PropertyInfo p6 = typeof (GenericImplementer<>).GetProperty ("Property");

      IEqualityComparer<PropertyInfo> comparer = new PropertyNameAndSignatureEqualityComparer ();

      Assert.IsTrue (comparer.Equals (null, null));
      Assert.IsFalse (comparer.Equals (null, p1));
      Assert.IsFalse (comparer.Equals (p1, null));

      Assert.IsTrue (comparer.Equals (p1, p2));
      Assert.IsFalse (comparer.Equals (p1, p3));
      Assert.IsFalse (comparer.Equals (p2, p3));
      Assert.IsFalse (comparer.Equals (p3, p4));
      Assert.IsTrue (comparer.Equals (p1, p4));

      Assert.IsTrue (comparer.Equals (p5, p5));
      Assert.IsFalse (comparer.Equals (p5, p6));
    }

    [Test]
    public void GetHashCodeForProperties ()
    {
      PropertyInfo p1 = typeof (IGeneric<string>).GetProperty ("Property");
      PropertyInfo p2 = typeof (GenericImplementer<string>).GetProperty ("Property");
      PropertyInfo p3 = typeof (GenericImplementer<string>).GetProperty ("Property2");
      PropertyInfo p4 = typeof (ConcreteImplementer).GetProperty ("Property");

      PropertyInfo p5 = typeof (IGeneric<>).GetProperty ("Property");
      PropertyInfo p6 = typeof (GenericImplementer<>).GetProperty ("Property");

      IEqualityComparer<PropertyInfo> comparer = new PropertyNameAndSignatureEqualityComparer ();

      Assert.AreEqual (comparer.GetHashCode(p1), comparer.GetHashCode(p2));
      Assert.AreNotEqual (comparer.GetHashCode(p1), comparer.GetHashCode(p3));
      Assert.AreNotEqual (comparer.GetHashCode(p2), comparer.GetHashCode(p3));
      Assert.AreNotEqual (comparer.GetHashCode(p3), comparer.GetHashCode(p4));
      Assert.AreEqual (comparer.GetHashCode(p1), comparer.GetHashCode(p4));

      Assert.AreEqual (comparer.GetHashCode(p5), comparer.GetHashCode(p5));
      Assert.AreEqual (comparer.GetHashCode(p5), comparer.GetHashCode(p6));
    }

    [Test]
    public void EqualsForEvents ()
    {
      EventInfo p1 = typeof (IGeneric<string>).GetEvent ("Event1");
      EventInfo p2 = typeof (GenericImplementer<string>).GetEvent ("Event1");
      EventInfo p3 = typeof (GenericImplementer<string>).GetEvent ("Event2");
      EventInfo p4 = typeof (ConcreteImplementer).GetEvent ("Event1");

      EventInfo p5 = typeof (IGeneric<>).GetEvent ("Event1");
      EventInfo p6 = typeof (GenericImplementer<>).GetEvent ("Event1");

      IEqualityComparer<EventInfo> comparer = new EventNameAndSignatureEqualityComparer ();

      Assert.IsTrue (comparer.Equals (null, null));
      Assert.IsFalse (comparer.Equals (null, p1));
      Assert.IsFalse (comparer.Equals (p1, null));

      Assert.IsTrue (comparer.Equals (p1, p2));
      Assert.IsFalse (comparer.Equals (p1, p3));
      Assert.IsFalse (comparer.Equals (p2, p3));
      Assert.IsFalse (comparer.Equals (p3, p4));
      Assert.IsTrue (comparer.Equals (p1, p4));

      Assert.IsTrue (comparer.Equals (p5, p5));
      Assert.IsFalse (comparer.Equals (p5, p6));
    }

    [Test]
    public void GetHashCodeForEvents ()
    {
      EventInfo p1 = typeof (IGeneric<string>).GetEvent ("Event1");
      EventInfo p2 = typeof (GenericImplementer<string>).GetEvent ("Event1");
      EventInfo p3 = typeof (GenericImplementer<string>).GetEvent ("Event2");
      EventInfo p4 = typeof (ConcreteImplementer).GetEvent ("Event1");

      EventInfo p5 = typeof (IGeneric<>).GetEvent ("Event1");
      EventInfo p6 = typeof (GenericImplementer<>).GetEvent ("Event1");

      IEqualityComparer<EventInfo> comparer = new EventNameAndSignatureEqualityComparer ();

      Assert.AreEqual (comparer.GetHashCode (p1), comparer.GetHashCode (p2));
      Assert.AreNotEqual (comparer.GetHashCode (p1), comparer.GetHashCode (p3));
      Assert.AreNotEqual (comparer.GetHashCode (p2), comparer.GetHashCode (p3));
      Assert.AreNotEqual (comparer.GetHashCode (p3), comparer.GetHashCode (p4));
      Assert.AreEqual (comparer.GetHashCode (p1), comparer.GetHashCode (p4));

      Assert.AreEqual (comparer.GetHashCode (p5), comparer.GetHashCode (p5));
      Assert.AreEqual (comparer.GetHashCode (p5), comparer.GetHashCode (p6));
    }
  }
}
