/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
