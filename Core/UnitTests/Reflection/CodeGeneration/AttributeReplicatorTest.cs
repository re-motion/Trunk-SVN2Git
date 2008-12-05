// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
      IAttributableEmitter emitter = mockRepository.StrictMock<IAttributableEmitter> ();
      
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
