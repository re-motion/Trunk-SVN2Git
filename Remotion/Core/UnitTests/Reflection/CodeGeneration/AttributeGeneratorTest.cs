// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class AttributeGeneratorTest
  {
    private AttributeGenerator _generator;

    [SetUp]
    public void SetUp ()
    {
      _generator = new AttributeGenerator ();
    }

    [Test]
    public void GenerateAttribute ()
    {
      var mockRepository = new MockRepository ();
      var emitterMock = mockRepository.StrictMock<IAttributableEmitter> ();
      
      emitterMock.Expect (mock => mock.AddCustomAttribute (Arg<CustomAttributeBuilder>.Is.NotNull));

      mockRepository.ReplayAll ();

      CustomAttributeData data = CustomAttributeData.GetCustomAttributes (typeof (AttributeGeneratorTest))[0];
      _generator.GenerateAttribute (emitterMock, data);

      mockRepository.VerifyAll ();
    }
  }
}
