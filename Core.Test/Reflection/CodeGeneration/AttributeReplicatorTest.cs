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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class AttributeReplicatorTest
  {
    [Test]
    public void ReplicateAttribute ()
    {
      MockRepository mockRepository = new MockRepository ();
      IAttributableEmitter emitter = mockRepository.CreateMock<IAttributableEmitter> ();
      
      // expect
      emitter.AddCustomAttribute (null);
      LastCall.Constraints (Mocks_Is.NotNull ());

      mockRepository.ReplayAll ();

      CustomAttributeData data = CustomAttributeData.GetCustomAttributes (typeof (AttributeReplicatorTest))[0];
      AttributeReplicator.ReplicateAttribute (emitter, data);

      mockRepository.VerifyAll ();
    }
  }
}
