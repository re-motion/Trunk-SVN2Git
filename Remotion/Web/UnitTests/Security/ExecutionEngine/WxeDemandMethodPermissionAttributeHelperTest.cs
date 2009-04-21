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
using NUnit.Framework;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandMethodPermissionAttributeHelperTest
  {
    [Test]
    public void InitializeWithMethodTypeInstance ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show");

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Instance, helper.MethodType);
      Assert.IsNull (helper.SecurableClass);
      Assert.AreEqual ("Show", helper.MethodName);
    }

    [Test]
    public void InitializeWithMethodTypeInstanceAndSecurableClass ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("ShowSpecial", typeof (DerivedSecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Instance, helper.MethodType);
      Assert.AreSame (typeof (DerivedSecurableObject), helper.SecurableClass);
      Assert.AreEqual ("ShowSpecial", helper.MethodName);
    }

    [Test]
    public void InitializeWithMethodTypeInstanceAndMethodEnum ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Instance, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Show", helper.MethodName);
    }


    [Test]
    public void InitializeWithMethodTypeStatic ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute ("Search", typeof (SecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Static, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Search", helper.MethodName);
    }

    [Test]
    public void InitializeWithMethodTypeStaticAndMethodEnum ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Static, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Search", helper.MethodName);
    }

    [Test]
    public void InitializeWithMethodTypeStaticAndMethodEnumFromBaseClass ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = 
          new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search, typeof (DerivedSecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Static, helper.MethodType);
      Assert.AreSame (typeof (DerivedSecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Search", helper.MethodName);
    }


    [Test]
    public void InitializeWithMethodTypeConstructor ()
    {
      WxeDemandTargetPermissionAttribute attribute = new WxeDemandCreatePermissionAttribute (typeof (SecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Constructor, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.IsNull (helper.MethodName);
    }
  }
}
