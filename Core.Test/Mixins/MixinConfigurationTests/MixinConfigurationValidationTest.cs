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
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationValidationTest
  {
    [Test]
    public void ValidateWithNoErrors ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
        {
          IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
          Assert.IsTrue (log.GetNumberOfSuccesses () > 0);
          Assert.AreEqual (0, log.GetNumberOfFailures ());
        }
      }
    }

    [Test]
    public void ValidateWithErrors ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<int> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
        {
          IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
          Assert.IsTrue (log.GetNumberOfFailures () > 0);
        }
      }
    }

    class UninstantiableGeneric<T>
      where T : ISerializable, IServiceProvider
    {
    }

    [Test]
    public void ValidateWithGenerics_IgnoresGenerics ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (KeyValuePair<,>)).Clear ().AddMixins (typeof (NullMixin))
          .ForClass (typeof (UninstantiableGeneric<>)).Clear().AddMixins (typeof (NullMixin))
          .EnterScope ())
      {
        IValidationLog log = MixinConfiguration.ActiveConfiguration.Validate ();
        Assert.AreEqual (0, log.GetNumberOfFailures ());
      }
    }
  }
}
