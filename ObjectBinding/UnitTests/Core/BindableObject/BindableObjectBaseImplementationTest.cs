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
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectBaseImplementationTest
  {
    [Test]
    public void Create ()
    {
      var wrapper = new ClassDerivedFromBindableObjectBase ();
      var mixin = BindableObjectBaseImplementation.Create (wrapper);
      Assert.That (mixin.BusinessObjectClass, Is.Not.Null);
      Assert.That (PrivateInvoke.GetNonPublicProperty (mixin, "This"), Is.SameAs (wrapper));
    }

    [Test]
    public void Deserialization ()
    {
      var wrapper = new ClassDerivedFromBindableObjectBase ();
      var mixin = BindableObjectBaseImplementation.Create (wrapper);
      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.NewTuple (mixin, wrapper));
      Assert.That (deserializedData.A.BusinessObjectClass, Is.Not.Null);
      Assert.That (PrivateInvoke.GetNonPublicProperty (deserializedData.A, "This"), Is.SameAs (deserializedData.B));
    }
  }
}