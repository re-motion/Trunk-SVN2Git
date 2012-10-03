// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.DynamicProxy
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

      var data = new CustomAttributeDataAdapter (CustomAttributeData.GetCustomAttributes (typeof (AttributeGeneratorTest))[0]);
      _generator.GenerateAttribute (emitterMock, data);

      mockRepository.VerifyAll ();
    }
  }
}
