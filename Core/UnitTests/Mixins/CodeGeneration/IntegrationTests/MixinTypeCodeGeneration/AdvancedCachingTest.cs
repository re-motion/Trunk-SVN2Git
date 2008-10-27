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
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class AdvancedCachingTest : CodeGenerationBaseTest
  {
    [Test]
    public void BaseClassNotOverridingMixinMethod()
    {
      var instance = ObjectFactory.Create<BaseClassNotOverridingMixinMethod> ().With ();
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses>(instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("Mixin.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("Mixin.M2"));
    }

    [Test]
    public void DerivedClassOverridingMixinMethod ()
    {
      var instance = ObjectFactory.Create<DerivedClassOverridingMixinMethod> ().With ();
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("Mixin.M2"));
    }

    [Test]
    public void DerivedClassOverridingMixinMethod2 ()
    {
      var instance = ObjectFactory.Create<DerivedClassOverridingMixinMethod2> ().With ();
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod2.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("Mixin.M2"));
    }

    [Test]
    public void DerivedDerivedClassOverridingMixinMethod ()
    {
      var instance = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> ().With ();
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedClassOverridingMixinMethod.M2"));
    }

    [Test]
    public void DerivedDerivedClassWithDifferentMixinConfigurations ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<DerivedDerivedClassOverridingMixinMethod> ().AddMixin<MixinWithMethodsOverriddenByDifferentClasses> ().EnterScope ())
      {
        var instance = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> ().With ();
        var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
        Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
        Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedClassOverridingMixinMethod.M2"));
      }

      using (MixinConfiguration.BuildNew ().ForClass<DerivedDerivedClassOverridingMixinMethod> ().AddMixin<MixinWithMethodsOverriddenByDifferentClasses2> ().EnterScope())
      {
        var instance = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod>().With();
        var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses2> (instance);
        Assert.That (mixin.M1(), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
        Assert.That (mixin.M2(), Is.EqualTo ("DerivedDerivedClassOverridingMixinMethod.M2"));
      }
    }

    [Test]
    public void DerivedDerivedDerivedClassOverridingMixinMethod ()
    {
      var instance = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod> ().With ();
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod.M2"));
    }

    [Test]
    public void DerivedDerivedDerivedClassOverridingMixinMethod2 ()
    {
      var instance = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod2> ().With ();
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod2.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod2.M2"));
    }

    [Test]
    public void Caching_Bottom_To_Top()
    {
      var instance1 = ObjectFactory.Create<BaseClassNotOverridingMixinMethod> ().With ();
      var mixin1 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance1);

      var instance2 = ObjectFactory.Create<DerivedClassOverridingMixinMethod> ().With ();
      var mixin2 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2);

      var instance2b = ObjectFactory.Create<DerivedClassOverridingMixinMethod2> ().With ();
      var mixin2b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2b);

      var instance3 = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> ().With ();
      var mixin3 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance3);

      var instance4 = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod> ().With ();
      var mixin4 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4);

      var instance4b = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod2> ().With ();
      var mixin4b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4b);

      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin2.GetType ()));
      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2b.GetType (), Is.Not.SameAs (mixin2.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4b.GetType ()));
    }

    [Test]
    public void Caching_Top_To_Bottom ()
    {
      var instance4b = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod2> ().With ();
      var mixin4b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4b);

      var instance4 = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod> ().With ();
      var mixin4 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4);

      var instance3 = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> ().With ();
      var mixin3 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance3);

      var instance2b = ObjectFactory.Create<DerivedClassOverridingMixinMethod2> ().With ();
      var mixin2b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2b);

      var instance2 = ObjectFactory.Create<DerivedClassOverridingMixinMethod> ().With ();
      var mixin2 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2);

      var instance1 = ObjectFactory.Create<BaseClassNotOverridingMixinMethod> ().With ();
      var mixin1 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance1);

      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin2.GetType ()));
      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2b.GetType (), Is.Not.SameAs (mixin2.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4b.GetType ()));
    }

    [Test]
    public void Tests_Bottom_To_Top ()
    {
      BaseClassNotOverridingMixinMethod();
      DerivedClassOverridingMixinMethod ();
      DerivedClassOverridingMixinMethod2 ();
      DerivedDerivedClassOverridingMixinMethod();
      DerivedDerivedDerivedClassOverridingMixinMethod ();
      DerivedDerivedDerivedClassOverridingMixinMethod2 ();
    }

    [Test]
    public void Tests_Top_To_Bottom ()
    {
      DerivedDerivedDerivedClassOverridingMixinMethod2 ();
      DerivedDerivedDerivedClassOverridingMixinMethod ();
      DerivedDerivedClassOverridingMixinMethod ();
      DerivedClassOverridingMixinMethod2 ();
      DerivedClassOverridingMixinMethod ();
      BaseClassNotOverridingMixinMethod ();
    }
  }
}