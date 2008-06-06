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
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixinBaseClassTest
  {
    public class MixinWithOnInitialize1 : Mixin<object>
    {
      public object ThisValue;

      public MixinWithOnInitialize1()
      {
        try
        {
          object t = This;
          Assert.Fail("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }
        Assert.IsNull (ThisValue);
      }

      protected override void OnInitialized ()
      {
        Assert.IsNotNull (This);
        ThisValue = This;
        base.OnInitialized();
      }
    }

    [Test]
    public void ThisAccessInCtorAndInitialize()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithOnInitialize1)).With();
      MixinWithOnInitialize1 mixin = Mixin.Get<MixinWithOnInitialize1> (bt1);
      Assert.IsNotNull (mixin);
      Assert.IsNotNull (mixin.ThisValue);
    }

    private FuncInvokerWrapper<TTargetType> CreateMixedObject<TTargetType> (params Type[] types)
    {
      using (MixinConfiguration.BuildNew ().ForClass<TTargetType> ().AddMixins (types).EnterScope ())
      {
        return ObjectFactory.Create<TTargetType> ();
      }
    }

    public class MixinWithOnInitialize2 : Mixin<object, IBaseType2>
    {
      public object ThisValue;
      public object BaseValue;

      public MixinWithOnInitialize2 ()
      {
        try
        {
          object t = This;
          Assert.Fail ("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }

        try
        {
          object t = Base;
          Assert.Fail ("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }

        Assert.IsNull (ThisValue);
        Assert.IsNull (BaseValue);
      }

      protected override void OnInitialized ()
      {
        Assert.IsNotNull (This);
        Assert.IsNotNull (Base);
        ThisValue = This;
        BaseValue = Base;
        base.OnInitialized ();
      }
    }

    [Test]
    public void BaseAccessInCtorAndInitialize ()
    {
      BaseType2 bt2 = CreateMixedObject<BaseType2> (typeof (MixinWithOnInitialize2)).With ();
      MixinWithOnInitialize2 mixin = Mixin.Get<MixinWithOnInitialize2> (bt2);
      Assert.IsNotNull (mixin);
      Assert.IsNotNull (mixin.ThisValue);
      Assert.IsNotNull (mixin.BaseValue);
    }
  }
}
