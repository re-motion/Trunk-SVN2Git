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

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization.TestDomain
{
  [Serializable]
  public class MixinWithOnInitializedAndOnDeserialized : Mixin<object, object>
  {
    [NonSerialized]
    public bool OnInitializedCalled;
    [NonSerialized]
    public bool OnDeserializedCalled;

    protected override void OnInitialized ()
    {
      OnInitializedCalled = true;
      Assert.IsNotNull (This);
      Assert.IsNotNull (Base);
      base.OnInitialized ();
    }

    protected override void OnDeserialized ()
    {
      OnDeserializedCalled = true;
      Assert.IsNotNull (This);
      Assert.IsNotNull (Base);
      base.OnDeserialized ();
    }
  }
}