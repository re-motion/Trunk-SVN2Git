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
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectBaseIntegrationTest
  {
    private ClassDerivedFromBindableObjectBase _instance;
    private ClassDerivedFromBindableObjectBaseOverridingDisplayName _instanceOverridingDisplayName;

    [SetUp]
    public void SetUp()
    {
      _instance = new ClassDerivedFromBindableObjectBase ();
      _instanceOverridingDisplayName = new ClassDerivedFromBindableObjectBaseOverridingDisplayName ();
    }

    [Test]
    public void BusinessObjectClass()
    {
      Assert.That (_instance.BusinessObjectClass, Is.InstanceOfType (typeof (BindableObjectClass)));
      var bindableObjectClass = (BindableObjectClass) _instance.BusinessObjectClass;
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.InstanceOfType (typeof (BindableObjectProvider)));
      Assert.That (bindableObjectClass.ConcreteType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectBase)));
      Assert.That (bindableObjectClass.TargetType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectBase)));
    }

    [Test]
    public void DisplayName_Default()
    {
      Assert.That (_instance.DisplayName, Is.EqualTo (TypeUtility.GetPartialAssemblyQualifiedName (typeof (ClassDerivedFromBindableObjectBase))));
    }

    [Test]
    public void DisplayName_Overridden ()
    {
      Assert.That (_instanceOverridingDisplayName.DisplayName, Is.EqualTo ("Overrotten!"));
    }

    [Test]
    public void GetProperty()
    {
      _instance.String = "hoo";
      Assert.That (_instance.GetProperty ("String"), Is.EqualTo ("hoo"));
    }

    [Test]
    public void SetProperty ()
    {
      _instance.SetProperty ("String", "damn");
      Assert.That (_instance.String, Is.EqualTo ("damn"));
    }
  }
}