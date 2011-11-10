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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class ParamListImplementationTest
  {
    private ParamListImplementation _implementation0;
    private ParamListImplementation<int> _implementation1;
    private ParamListImplementation<int, string, double> _implementation3;

    [SetUp]
    public void SetUp ()
    {
      _implementation0 = new ParamListImplementation ();
      _implementation1 = new ParamListImplementation<int> (1);
      _implementation3 = new ParamListImplementation<int, string, double> (1, "2", 3.0);
    }

    [Test]
    public void FuncType ()
    {
      Assert.That (_implementation0.FuncType, Is.EqualTo (typeof (Func<object>)));
      Assert.That (_implementation1.FuncType, Is.EqualTo (typeof (Func<int, object>)));
      Assert.That (_implementation3.FuncType, Is.EqualTo (typeof (Func<int, string, double, object>)));
    }

    [Test]
    public void ActionType ()
    {
      Assert.That (_implementation0.ActionType, Is.EqualTo (typeof (Action)));
      Assert.That (_implementation1.ActionType, Is.EqualTo (typeof (Action<int>)));
      Assert.That (_implementation3.ActionType, Is.EqualTo (typeof (Action<int, string, double>)));
    }

    [Test]
    public void InvokeFunc ()
    {
      var ret = "ret";
      Assert.That (_implementation0.InvokeFunc (((Func<object>) (() => ret))), Is.SameAs (ret));
      Assert.That (_implementation1.InvokeFunc (((Func<int, object>) (i => i + ret))), Is.EqualTo (1 + ret));
      Assert.That (_implementation3.InvokeFunc (((Func<int, string, double, object>) ((i, s, d) => i + s + d + ret))), Is.EqualTo (1 + "2" + 3.0 + ret));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Test")]
    public void InvokeFunc_WithException ()
    {
      _implementation0.InvokeFunc (((Func<object>) (() => { throw new InvalidOperationException ("Test"); })));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument func has type System.Func`1[System.Int32] when type "
        + "System.Func`1[System.Object] was expected.\r\nParameter name: func")]
    public void InvokeFunc_InvalidDelegate ()
    {
      _implementation0.InvokeFunc (((Func<int>) (() => 5)));
    }
    
    [Test]
    public void InvokeAction ()
    {
      string result = null;
      _implementation0.InvokeAction (((Action) (delegate { result = "done"; })));
      Assert.That (result, Is.EqualTo ("done"));

      _implementation1.InvokeAction (((Action<int>) (delegate (int i) { result = "done" + i; })));
      Assert.That (result, Is.EqualTo ("done1"));

      _implementation3.InvokeAction (((Action<int, string, double>) (delegate (int i, string s, double d) { result = "done" + i + s + d; })));
      Assert.That (result, Is.EqualTo ("done123"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Test")]
    public void InvokeAction_WithException ()
    {
      _implementation0.InvokeAction (((Action) (() => { throw new InvalidOperationException ("Test"); })));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument action has type System.Action`1[System.Int32] when type " 
        + "System.Action was expected.\r\nParameter name: action")]
    public void InvokeAction_InvalidDelegate ()
    {
      _implementation0.InvokeAction (((Action<int>) (i => { })));
    }

    [Test]
    public void InvokeConstructor ()
    {
      var info = new ConstructorLookupInfo (typeof (List<int>));
      var list = _implementation1.InvokeConstructor (info);

      Assert.That (list, Is.InstanceOf (typeof (List<int>)));
      Assert.That (((List<int>)list).Capacity, Is.EqualTo (_implementation1.GetParameterValues()[0]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void InvokeConstructor_WithException ()
    {
      var info = new ConstructorLookupInfo (typeof (List<int>));
      new ParamListImplementation<int> (-2).InvokeConstructor (info);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type 'System.Collections.Generic.List`1[System.Int32]' does not contain a constructor with the following arguments types: "
        + "System.Int32, System.String, System.Double.")]
    public void InvokeConstructor_NoMatchingCtor ()
    {
      var info = new ConstructorLookupInfo (typeof (List<int>));
      _implementation3.InvokeConstructor (info);
    }

    [Test]
    public void GetParameterTypes ()
    {
      Assert.That (_implementation0.GetParameterTypes (), Is.Empty);
      Assert.That (_implementation1.GetParameterTypes (), Is.EqualTo (new[] { typeof (int) }));
      Assert.That (_implementation3.GetParameterTypes (), Is.EqualTo (new[] { typeof (int), typeof (string), typeof (double) }));
    }

    [Test]
    public void GetParameterValues ()
    {
      Assert.That (_implementation0.GetParameterValues (), Is.Empty);
      Assert.That (_implementation1.GetParameterValues (), Is.EqualTo (new object[] { 1 }));
      Assert.That (_implementation3.GetParameterValues (), Is.EqualTo (new object[] { 1, "2", 3.0 }));
    }
  }
}
