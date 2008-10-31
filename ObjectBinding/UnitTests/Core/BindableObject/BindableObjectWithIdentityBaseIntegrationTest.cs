/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityBaseIntegrationTest
  {
    private ClassDerivedFromBindableObjectWithIdentityBase _instance;
    private ClassDerivedFromBindableObjectWithIdentityBaseOverridingDisplayName _instanceOverridingDisplayName;

    [SetUp]
    public void SetUp()
    {
      _instance = new ClassDerivedFromBindableObjectWithIdentityBase ();
      _instanceOverridingDisplayName = new ClassDerivedFromBindableObjectWithIdentityBaseOverridingDisplayName ();
    }

    [Test]
    public void BusinessObjectClass()
    {
      Assert.That (_instance.BusinessObjectClass, Is.InstanceOfType (typeof (BindableObjectClass)));
      var bindableObjectClass = (BindableObjectClass) _instance.BusinessObjectClass;
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.InstanceOfType (typeof (BindableObjectProvider)));
      Assert.That (bindableObjectClass.ConcreteType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectWithIdentityBase)));
      Assert.That (bindableObjectClass.TargetType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectWithIdentityBase)));
    }

    [Test]
    public void DisplayName_Default()
    {
      Assert.That (_instance.DisplayName, Is.EqualTo (TypeUtility.GetPartialAssemblyQualifiedName (typeof (ClassDerivedFromBindableObjectWithIdentityBase))));
    }

    [Test]
    public void DisplayName_Overridden ()
    {
      Assert.That (_instanceOverridingDisplayName.DisplayName, Is.EqualTo ("Overrotten!"));
    }

    [Test]
    public void DisplayNameSafe_Default ()
    {
      Assert.That (_instance.DisplayNameSafe, Is.EqualTo (TypeUtility.GetPartialAssemblyQualifiedName (typeof (ClassDerivedFromBindableObjectWithIdentityBase))));
    }

    [Test]
    public void DisplayNameSafe_Overridden ()
    {
      Assert.That (_instanceOverridingDisplayName.DisplayNameSafe, Is.EqualTo ("Overrotten!"));
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

    [Test]
    public void UniqueIdentifier()
    {
      _instance.SetUniqueIdentifier ("hu");
      Assert.That (_instance.UniqueIdentifier, Is.EqualTo ("hu"));
    }

    [Test]
    public void GetProvider ()
    {
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectWithIdentityBase)),
          Is.SameAs (BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute> ()));
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectWithIdentityBase)),
          Is.Not.SameAs (BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute> ()));
    }

    [Test]
    public void ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod ();
      Assert.That (instance.BusinessObjectClass, Is.InstanceOfType (typeof (BindableObjectClass)));
      Assert.That (((BindableObjectClass) instance.BusinessObjectClass).TargetType, Is.SameAs (typeof (ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod)));
      Assert.That (((BindableObjectClass) instance.BusinessObjectClass).ConcreteType, Is.SameAs (TypeFactory.GetConcreteType (typeof (ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod))));
    }
  }
}