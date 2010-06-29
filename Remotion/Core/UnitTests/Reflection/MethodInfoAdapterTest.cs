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
using Remotion.Reflection;
using Remotion.UnitTests.Reflection.PropertyInfoAdapterTestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class MethodInfoAdapterTest
  {
    private MethodInfo _method;
    private IMethodInformation _adapter;

    [SetUp]
    public void SetUp ()
    {
      _method = typeof (Test).GetMethod ("GetStringWithoutArguments");
      _adapter = new MethodInfoAdapter (_method);
    }

    [Test]
    public void GetMethodInfo ()
    {
      Assert.That (((MethodInfoAdapter)_adapter).MethodInfo, Is.SameAs (_method));
    }

    
    [Test]
    public void Equals ()
    {
      Assert.That (_adapter, Is.EqualTo (new MethodInfoAdapter(_method)));
      Assert.AreNotEqual (_adapter, new MethodInfoAdapter (typeof (Test).GetMethod ("TestMethod")));
    }

    [Test]
    public void GetReturnType ()
    {
      Assert.That (_adapter.ReturnType, Is.EqualTo (_method.ReturnType));
    }

    [Test]
    public void GetName ()
    {
      Assert.That (_adapter.Name, Is.EqualTo (_method.Name));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (
          _adapter.IsDefined<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.IsDefined<SampleAttribute> (_method, true)));
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.That (_adapter.DeclaringType, Is.EqualTo (_method.DeclaringType));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_adapter.GetOriginalDeclaringType(), Is.EqualTo (_method.DeclaringType));
    }

    [Test]
    public void GetCustomAttribut ()
    {
      Assert.That (_adapter.GetCustomAttribute<SampleAttribute> (true), 
        Is.EqualTo (AttributeUtility.GetCustomAttribute<SampleAttribute> (_method, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (_adapter.GetCustomAttributes<SampleAttribute> (true), 
        Is.EqualTo (AttributeUtility.GetCustomAttributes<SampleAttribute> (_method, false)));
    }

    [Test]
    public void Invoke_WithoutParameters ()
    {
      var result = _adapter.Invoke (new Test(), new object[]{});
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void Invoke_WithOneParameter ()
    {
      var methodInfo = typeof (Test).GetMethod ("GetStringWithOneArgument");
      var adapter = new MethodInfoAdapter (methodInfo);
      var result = adapter.Invoke (new Test(), new object[] {string.Empty });

      Assert.That (result, Is.EqualTo ("Test"));
    }

    public class Test
    {
      public void TestMethod ()
      {
      }

      public string GetStringWithoutArguments ()
      {
        return "Test";
      }

      public string GetStringWithOneArgument (string parameter)
      {
        return "Test";
      }
    }

    
  }

}