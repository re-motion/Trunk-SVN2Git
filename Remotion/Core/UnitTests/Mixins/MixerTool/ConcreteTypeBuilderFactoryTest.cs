// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
    private IConcreteMixedTypeNameProvider _typeNameProviderStub;
    private ConcreteTypeBuilderFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _typeNameProviderStub = MockRepository.GenerateStub<IConcreteMixedTypeNameProvider> ();
      _factory = new ConcreteTypeBuilderFactory (_typeNameProviderStub, "Signed", "Unsigned");
    }

    [Test]
    public void CreateTypeBuilder ()
    {
      var builder = (ConcreteTypeBuilder) _factory.CreateTypeBuilder (@"c:\directory");

      Assert.That (builder.TypeNameProvider, Is.SameAs (_typeNameProviderStub));
      
      Assert.That (builder.Scope.SignedAssemblyName, Is.EqualTo ("Signed"));
      Assert.That (builder.Scope.SignedModulePath, Is.EqualTo (@"c:\directory\Signed.dll"));

      Assert.That (builder.Scope.UnsignedAssemblyName, Is.EqualTo ("Unsigned"));
      Assert.That (builder.Scope.UnsignedModulePath, Is.EqualTo (@"c:\directory\Unsigned.dll"));
    }

    [Test]
    public void GetSignedModulePath ()
    {
      Assert.That (_factory.GetSignedModulePath (@"c:\directory"), Is.EqualTo (@"c:\directory\Signed.dll"));
    }

    [Test]
    public void GetUnsignedModulePath ()
    {
      Assert.That (_factory.GetUnsignedModulePath (@"c:\directory"), Is.EqualTo (@"c:\directory\Unsigned.dll"));
    }
  }
}
