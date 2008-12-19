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
  public class SignatureCheckerTest
  {
    public void SimpleMethod1()
    {
    }

    public void SimpleMethod2()
    {
    }

    public Dictionary<List<int>, string> ComplexMethod1 (string p1, object p2, object[] p3, List<int> p4, object[,] p5, out object p6, out int p7, ref string p8)
    {
      p6 = null;
      p7 = 0;
      return null;
    }

    public Dictionary<List<int>, string> ComplexMethod2 (string p1, object p2, object[] p3, List<int> p4, object[,] p5, out object p6, out int p7, ref string p8)
    {
      p6 = null;
      p7 = 0;
      return null;
    }

    public Dictionary<List<int>, string> ComplexMethod3 (object p2, string p1, object[] p3, List<int> p4, object[,] p5, out object p6, out int p7, ref string p8)
    {
      p6 = null;
      p7 = 0;
      return null;
    }

    public Dictionary<List<uint>, string> ComplexMethod4 (string p1, object p2, object[] p3, List<int> p4, object[,] p5, out object p6, out int p7, ref string p8)
    {
      p6 = null;
      p7 = 0;
      return null;
    }

    public Dictionary<List<int>, string> ComplexMethod5 (string p1, object p2, object[] p3, List<int> p4, object[,,] p5, out object p6, out int p7, ref string p8)
    {
      p6 = null;
      p7 = 0;
      return null;
    }

    public Dictionary<List<int>, string> ComplexMethod6 (string p1, object p2, object[] p3, List<int> p4, object[,] p5, out object p6, ref int p7, ref string p8)
    {
      p6 = null;
      p7 = 0;
      return null;
    }

    public R GenericMethod1<T, R> (T t)
    {
      return default (R);
    }

    public T GenericMethod2<T, R> (R t)
    {
      return default (T);
    }

    public T GenericMethod3<R, T> (R t)
    {
      return default (T);
    }

    public R GenericMethod4<T, R, A> (T t)
    {
      return default (R);
    }

    interface IGeneric<A, B>
    {
      void Method (A one, B two);
    }

    class GenericClass<A, B> : IGeneric<B, A>
    {
      public void Method (B one, A two)
      {
        throw new NotImplementedException();
      }

      public void Method2 (B one, A two)
      {
        throw new NotImplementedException ();
      }

      public void Method3 (A one, B two)
      {
        throw new NotImplementedException ();
      }
    }

    public int SimpleProperty1
    {
      get { return 1; }
    }

    public int SimpleProperty2
    {
      get { return 2; }
    }

    public object SimpleProperty3
    {
      get { return 3; }
    }

    public Dictionary<int, string> ComplexProperty1
    {
      get { return null; }
    }

    public Dictionary<int, string> ComplexProperty2
    {
      get { return null; }
    }

    public Dictionary<string, int> ComplexProperty3
    {
      get { return null; }
    }

    [Test]
    public void SimpleMethods()
    {
      MethodInfo m1 = typeof (SignatureCheckerTest).GetMethod ("SimpleMethod1");
      MethodInfo m2 = typeof (SignatureCheckerTest).GetMethod ("SimpleMethod2");
      MethodInfo c1 = typeof (SignatureCheckerTest).GetMethod ("ComplexMethod1");

      SignatureChecker sc = new SignatureChecker();

      Assert.IsTrue (sc.MethodSignaturesMatch (m1, m2));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, c1));
    }

    [Test]
    public void ComplexMethods ()
    {
      MethodInfo m1 = typeof (SignatureCheckerTest).GetMethod ("ComplexMethod1");
      MethodInfo m2 = typeof (SignatureCheckerTest).GetMethod ("ComplexMethod2");
      MethodInfo m3 = typeof (SignatureCheckerTest).GetMethod ("ComplexMethod3");
      MethodInfo m4 = typeof (SignatureCheckerTest).GetMethod ("ComplexMethod4");
      MethodInfo m5 = typeof (SignatureCheckerTest).GetMethod ("ComplexMethod5");
      MethodInfo m6 = typeof (SignatureCheckerTest).GetMethod ("ComplexMethod6");

      SignatureChecker sc = new SignatureChecker ();

      Assert.IsTrue (sc.MethodSignaturesMatch (m1, m2));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, m3));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, m4));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, m5));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, m6));
    }

    [Test]
    public void GenericMethods ()
    {
      MethodInfo m1 = typeof (SignatureCheckerTest).GetMethod ("GenericMethod1");
      MethodInfo m2 = typeof (SignatureCheckerTest).GetMethod ("GenericMethod2");
      MethodInfo m3 = typeof (SignatureCheckerTest).GetMethod ("GenericMethod3");
      MethodInfo m4 = typeof (SignatureCheckerTest).GetMethod ("GenericMethod4");

      SignatureChecker sc = new SignatureChecker ();

      Assert.IsTrue (sc.MethodSignaturesMatch (m1, m1));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, m2));
      Assert.IsTrue (sc.MethodSignaturesMatch (m1, m3));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, m4));
    }

    [Test]
    public void MethodsInGenericClasses ()
    {
      MethodInfo m1 = typeof (GenericClass<,>).GetMethod ("Method");
      MethodInfo m2 = typeof (GenericClass<,>).GetMethod ("Method2");
      MethodInfo m3 = typeof (GenericClass<,>).GetMethod ("Method3");

      SignatureChecker sc = new SignatureChecker ();

      Assert.IsTrue (sc.MethodSignaturesMatch (m1, m1));
      Assert.IsTrue (sc.MethodSignaturesMatch (m1, m2));
      Assert.IsFalse (sc.MethodSignaturesMatch (m1, m3));
      Assert.IsFalse (sc.MethodSignaturesMatch (m2, m3));
      Assert.IsTrue (sc.MethodSignaturesMatch (m3, m3));
    }

    [Test]
    public void SimpleProperties ()
    {
      PropertyInfo p1 = typeof (SignatureCheckerTest).GetProperty ("SimpleProperty1");
      PropertyInfo p2 = typeof (SignatureCheckerTest).GetProperty ("SimpleProperty2");
      PropertyInfo p3 = typeof (SignatureCheckerTest).GetProperty ("SimpleProperty3");

      SignatureChecker sc = new SignatureChecker();

      Assert.IsTrue (sc.PropertySignaturesMatch (p1, p1));
      Assert.IsTrue (sc.PropertySignaturesMatch (p1, p2));
      Assert.IsFalse (sc.PropertySignaturesMatch (p1, p3));
      Assert.IsTrue (sc.PropertySignaturesMatch (p3, p3));
    }

    [Test]
    public void ComplexProperties ()
    {
      PropertyInfo p1 = typeof (SignatureCheckerTest).GetProperty ("ComplexProperty1");
      PropertyInfo p2 = typeof (SignatureCheckerTest).GetProperty ("ComplexProperty2");
      PropertyInfo p3 = typeof (SignatureCheckerTest).GetProperty ("ComplexProperty3");

      SignatureChecker sc = new SignatureChecker ();

      Assert.IsTrue (sc.PropertySignaturesMatch (p1, p1));
      Assert.IsTrue (sc.PropertySignaturesMatch (p1, p2));
      Assert.IsFalse (sc.PropertySignaturesMatch (p1, p3));
      Assert.IsTrue (sc.PropertySignaturesMatch (p3, p3));
    }
  }
}
