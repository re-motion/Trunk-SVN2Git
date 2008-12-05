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
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class SignatureEqualityComparerTest
  {
    interface IGeneric<T>
    {
      int Method1(int i);
      T Method2 (int i);
      int Method3 (T t);

      T Property { get; set; }
      int this[T t] { get; set; }

      event Func<int, string> Event1;
      event Func<int, T> Event2;
    }

    interface IGeneric2<T> : IGeneric<T>
    {
      new T Method2 (int i);
      new T Property { get; set;}
      new event Func<int, T> Event2;
    }

    public class GenericImplementer<T> : IGeneric<T>
    {
      public int Method1 (int i)
      {
        throw new NotImplementedException();
      }

      public T Method2 (int i)
      {
        throw new NotImplementedException();
      }

      public int Method3 (T t)
      {
        throw new NotImplementedException();
      }

      public T Property
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public int this [T t]
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public event Func<int, string> Event1;
      public event Func<int, T> Event2;
    }

    [Test]
    public void EqualsForMethodSignatures()
    {
      MethodInfo m1 = typeof (IBaseType31).GetMethod ("IfcMethod");
      MethodInfo m2 = typeof (IBaseType32).GetMethod ("IfcMethod");
      MethodInfo m3 = typeof (IBaseType35).GetMethod ("IfcMethod2");
      MethodInfo m4 = typeof (BaseType4).GetMethod ("NonVirtualMethod");
      MethodInfo m5 = typeof (object).GetMethod ("ToString");
      MethodInfo m6 = typeof (object).GetMethod ("GetHashCode");

      MethodInfo m7 = typeof (IGeneric<string>).GetMethod ("Method1");
      MethodInfo m8 = typeof (IGeneric<string>).GetMethod ("Method2");
      MethodInfo m9 = typeof (IGeneric<string>).GetMethod ("Method3");
      MethodInfo m10 = typeof (IGeneric<int>).GetMethod ("Method2");
      MethodInfo m11 = typeof (IGeneric<int>).GetMethod ("Method3");

      MethodInfo m12 = typeof (IGeneric<>).GetMethod ("Method2");
      MethodInfo m13 = typeof (IGeneric2<>).GetMethod ("Method2");
      MethodInfo m14 = typeof (GenericImplementer<>).GetMethod ("Method2");

      IEqualityComparer<MethodInfo> comparer = new MethodSignatureEqualityComparer();

      Assert.IsTrue (comparer.Equals (null, null));
      Assert.IsFalse (comparer.Equals (m1, null));
      Assert.IsFalse (comparer.Equals (null, m1));

      Assert.IsTrue (comparer.Equals (m1, m1));
      Assert.IsTrue (comparer.Equals (m1, m2));
      Assert.IsTrue (comparer.Equals (m1, m3));
      Assert.IsTrue (comparer.Equals (m1, m4));
      Assert.IsTrue (comparer.Equals (m2, m4));
      Assert.IsTrue (comparer.Equals (m4, m4));
      Assert.IsTrue (comparer.Equals (m4, m5));
      Assert.IsFalse (comparer.Equals (m4, m6));
      Assert.IsTrue (comparer.Equals (m6, m6));
      
      Assert.IsFalse (comparer.Equals (m7, m8));
      Assert.IsFalse (comparer.Equals (m7, m9));
      Assert.IsFalse (comparer.Equals (m8, m9));
      Assert.IsTrue (comparer.Equals (m8, m8));
      Assert.IsTrue (comparer.Equals (m7, m10));
      Assert.IsTrue (comparer.Equals (m10, m11));

      Assert.IsFalse (comparer.Equals (m12, m13));
      Assert.IsFalse (comparer.Equals (m12, m14));
      Assert.IsFalse (comparer.Equals (m13, m14));
    }

    [Test]
    public void GetHashCodeForMethodSignatures ()
    {
      MethodInfo m1 = typeof (IBaseType31).GetMethod ("IfcMethod");
      MethodInfo m2 = typeof (IBaseType32).GetMethod ("IfcMethod");
      MethodInfo m3 = typeof (IBaseType35).GetMethod ("IfcMethod2");
      MethodInfo m4 = typeof (BaseType4).GetMethod ("NonVirtualMethod");
      MethodInfo m5 = typeof (object).GetMethod ("ToString");
      MethodInfo m6 = typeof (object).GetMethod ("GetHashCode");

      MethodInfo m7 = typeof (IGeneric<string>).GetMethod ("Method1");
      MethodInfo m8 = typeof (IGeneric<string>).GetMethod ("Method2");
      MethodInfo m9 = typeof (IGeneric<string>).GetMethod ("Method3");
      MethodInfo m10 = typeof (IGeneric<int>).GetMethod ("Method2");
      MethodInfo m11 = typeof (IGeneric<int>).GetMethod ("Method3");

      MethodInfo m12 = typeof (IGeneric<>).GetMethod ("Method2");
      MethodInfo m13 = typeof (IGeneric2<>).GetMethod ("Method2");
      MethodInfo m14 = typeof (GenericImplementer<>).GetMethod ("Method2");

      IEqualityComparer<MethodInfo> comparer = new MethodSignatureEqualityComparer ();

      Assert.AreEqual (comparer.GetHashCode(m1), comparer.GetHashCode(m1));
      Assert.AreEqual (comparer.GetHashCode(m1), comparer.GetHashCode(m2));
      Assert.AreEqual (comparer.GetHashCode(m1), comparer.GetHashCode(m3));
      Assert.AreEqual (comparer.GetHashCode(m1), comparer.GetHashCode(m4));
      Assert.AreEqual (comparer.GetHashCode(m2), comparer.GetHashCode(m4));
      Assert.AreEqual (comparer.GetHashCode(m4), comparer.GetHashCode(m4));
      Assert.AreEqual (comparer.GetHashCode(m4), comparer.GetHashCode(m5));
      Assert.AreNotEqual (comparer.GetHashCode(m4), comparer.GetHashCode(m6));
      Assert.AreEqual (comparer.GetHashCode(m6), comparer.GetHashCode(m6));

      Assert.AreNotEqual (comparer.GetHashCode(m7), comparer.GetHashCode(m8));
      Assert.AreNotEqual (comparer.GetHashCode(m7), comparer.GetHashCode(m9));
      Assert.AreNotEqual (comparer.GetHashCode(m8), comparer.GetHashCode(m9));
      Assert.AreEqual (comparer.GetHashCode(m8), comparer.GetHashCode(m8));
      Assert.AreEqual (comparer.GetHashCode(m7), comparer.GetHashCode(m10));
      Assert.AreEqual (comparer.GetHashCode(m10), comparer.GetHashCode(m11));

      Assert.AreEqual (comparer.GetHashCode (m12), comparer.GetHashCode (m13));
      Assert.AreEqual (comparer.GetHashCode (m12), comparer.GetHashCode (m14));
      Assert.AreEqual (comparer.GetHashCode (m13), comparer.GetHashCode (m13));
    }

    [Test]
    public void EqualsForPropertySignatures ()
    {
      PropertyInfo p1 = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo p2 = typeof (IGeneric<string>).GetProperty ("Property");
      PropertyInfo p3 = typeof (IGeneric<int>).GetProperty ("Property");
      PropertyInfo p4 = typeof (IGeneric<string>).GetProperty ("Item");
      PropertyInfo p5 = typeof (IGeneric<int>).GetProperty ("Item");

      PropertyInfo p6 = typeof (IGeneric<>).GetProperty ("Property");
      PropertyInfo p7 = typeof (IGeneric2<>).GetProperty ("Property");
      PropertyInfo p8 = typeof (GenericImplementer<>).GetProperty ("Property");

      IEqualityComparer<PropertyInfo> comparer = new PropertySignatureEqualityComparer();

      Assert.IsTrue (comparer.Equals (null, null));
      Assert.IsFalse (comparer.Equals (p1, null));
      Assert.IsFalse (comparer.Equals (null, p1));

      Assert.IsTrue (comparer.Equals (p1, p1));
      Assert.IsTrue (comparer.Equals (p1, p2));
      Assert.IsFalse (comparer.Equals (p1, p3));
      Assert.IsTrue (comparer.Equals (p3, p3));
      Assert.IsTrue (comparer.Equals (p4, p4));
      Assert.IsFalse (comparer.Equals (p4, p5));
      Assert.IsFalse (comparer.Equals (p1, p4));

      Assert.IsFalse (comparer.Equals (p6, p7));
      Assert.IsFalse (comparer.Equals (p6, p8));
      Assert.IsFalse (comparer.Equals (p7, p8));
    }

    [Test]
    public void GetHashCodeForPropertySignatures ()
    {
      PropertyInfo p1 = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo p2 = typeof (IGeneric<string>).GetProperty ("Property");
      PropertyInfo p3 = typeof (IGeneric<int>).GetProperty ("Property");
      PropertyInfo p4 = typeof (IGeneric<string>).GetProperty ("Item");
      PropertyInfo p5 = typeof (IGeneric<int>).GetProperty ("Item");

      PropertyInfo p6 = typeof (IGeneric<>).GetProperty ("Property");
      PropertyInfo p7 = typeof (IGeneric2<>).GetProperty ("Property");
      PropertyInfo p8 = typeof (GenericImplementer<>).GetProperty ("Property");

      IEqualityComparer<PropertyInfo> comparer = new PropertySignatureEqualityComparer ();
      Assert.AreEqual (comparer.GetHashCode(p1), comparer.GetHashCode(p1));
      Assert.AreEqual (comparer.GetHashCode(p1), comparer.GetHashCode(p2));
      Assert.AreNotEqual (comparer.GetHashCode(p1), comparer.GetHashCode(p3));
      Assert.AreEqual (comparer.GetHashCode(p3), comparer.GetHashCode(p3));
      Assert.AreEqual (comparer.GetHashCode(p4), comparer.GetHashCode(p4));
      Assert.AreNotEqual (comparer.GetHashCode(p4), comparer.GetHashCode(p5));
      Assert.AreNotEqual (comparer.GetHashCode(p1), comparer.GetHashCode(p4));

      Assert.AreEqual (comparer.GetHashCode(p6), comparer.GetHashCode(p7));
      Assert.AreEqual (comparer.GetHashCode(p6), comparer.GetHashCode(p8));
      Assert.AreEqual (comparer.GetHashCode(p7), comparer.GetHashCode(p8));
    }

    [Test]
    public void EqualsForEventSignatures ()
    {
      EventInfo e1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo e2 = typeof (IGeneric<string>).GetEvent ("Event1");
      EventInfo e3 = typeof (IGeneric<string>).GetEvent ("Event2");
      EventInfo e4 = typeof (IGeneric<int>).GetEvent ("Event2");

      EventInfo e5 = typeof (IGeneric<>).GetEvent ("Event2");
      EventInfo e6 = typeof (IGeneric2<>).GetEvent ("Event2");
      EventInfo e7 = typeof (GenericImplementer<>).GetEvent ("Event2");

      IEqualityComparer<EventInfo> comparer = new EventSignatureEqualityComparer ();
      Assert.IsTrue (comparer.Equals (null, null));
      Assert.IsFalse (comparer.Equals (e1, null));
      Assert.IsFalse (comparer.Equals (null, e1));

      Assert.IsTrue (comparer.Equals (e1, e1));
      Assert.IsFalse (comparer.Equals (e1, e2));
      Assert.IsTrue (comparer.Equals (e2, e3));
      Assert.IsFalse (comparer.Equals (e3, e4));

      Assert.IsFalse (comparer.Equals (e5, e6));
      Assert.IsFalse (comparer.Equals (e5, e7));
      Assert.IsFalse (comparer.Equals (e6, e7));
    }

    [Test]
    public void GetHashCodeForEventSignatures ()
    {
      EventInfo e1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo e2 = typeof (IGeneric<string>).GetEvent ("Event1");
      EventInfo e3 = typeof (IGeneric<string>).GetEvent ("Event2");
      EventInfo e4 = typeof (IGeneric<int>).GetEvent ("Event2");

      EventInfo e5 = typeof (IGeneric<>).GetEvent ("Event2");
      EventInfo e6 = typeof (IGeneric2<>).GetEvent ("Event2");
      EventInfo e7 = typeof (GenericImplementer<>).GetEvent ("Event2");

      IEqualityComparer<EventInfo> comparer = new EventSignatureEqualityComparer ();
      Assert.AreEqual (comparer.GetHashCode(e1), comparer.GetHashCode(e1));
      Assert.AreNotEqual (comparer.GetHashCode(e1), comparer.GetHashCode(e2));
      Assert.AreEqual (comparer.GetHashCode(e2), comparer.GetHashCode(e3));
      Assert.AreNotEqual (comparer.GetHashCode(e3), comparer.GetHashCode(e4));

      Assert.AreEqual (comparer.GetHashCode (e5), comparer.GetHashCode (e6));
      Assert.AreEqual (comparer.GetHashCode (e5), comparer.GetHashCode (e7));
      Assert.AreEqual (comparer.GetHashCode (e6), comparer.GetHashCode (e7));
    }
  }
}
