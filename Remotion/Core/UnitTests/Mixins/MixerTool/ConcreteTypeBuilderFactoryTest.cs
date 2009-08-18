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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.MixerTool;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [TestFixture]
  public class ConcreteTypeBuilderFactoryTest
  {
    [Test]
    public void CreateTypeBuilder ()
    {
      var typeNameProviderStub = MockRepository.GenerateStub<INameProvider> ();
      var factory = new ConcreteTypeBuilderFactory (typeNameProviderStub, "Signed", "Unsigned");

      var builder = factory.CreateTypeBuilder (@"c:\directory");

      Assert.That (builder.TypeNameProvider, Is.SameAs (typeNameProviderStub));
      
      Assert.That (builder.Scope.SignedAssemblyName, Is.EqualTo ("Signed"));
      Assert.That (builder.Scope.SignedModulePath, Is.EqualTo (@"c:\directory\Signed.dll"));

      Assert.That (builder.Scope.UnsignedAssemblyName, Is.EqualTo ("Unsigned"));
      Assert.That (builder.Scope.UnsignedModulePath, Is.EqualTo (@"c:\directory\Unsigned.dll"));
    }
  }
}