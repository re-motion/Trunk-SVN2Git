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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class TypeReferenceWrapperTest
  {
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
    }

    [Test]
    public void TypeAndOwner ()
    {
      Reference ownerReference = _mockRepository.StrictMock<Reference>();
      Reference wrappedReference = _mockRepository.StrictMock<Reference> (ownerReference);

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference, typeof (int));
      Assert.AreEqual (typeof (int), tr.Type);
      Assert.AreSame (ownerReference, tr.OwnerReference);
    }

    [Test]
    public void LoadReference ()
    {
      Reference wrappedReference = _mockRepository.StrictMock<Reference> ();
      ILGenerator gen = new DynamicMethod ("Foo", typeof (void), Type.EmptyTypes, AssemblyBuilder.GetExecutingAssembly().ManifestModule).GetILGenerator();

      // expect
      wrappedReference.LoadReference (gen);

      _mockRepository.ReplayAll ();

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference, typeof (int));
      tr.LoadReference (gen);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void StoreReference ()
    {
      Reference wrappedReference = _mockRepository.StrictMock<Reference> ();
      ILGenerator gen = new DynamicMethod ("Foo", typeof (void), Type.EmptyTypes, AssemblyBuilder.GetExecutingAssembly ().ManifestModule).GetILGenerator ();

      // expect
      wrappedReference.StoreReference (gen);

      _mockRepository.ReplayAll ();

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference, typeof (int));
      tr.StoreReference (gen);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadAddressReference ()
    {
      Reference wrappedReference = _mockRepository.StrictMock<Reference> ();
      ILGenerator gen = new DynamicMethod ("Foo", typeof (void), Type.EmptyTypes, AssemblyBuilder.GetExecutingAssembly ().ManifestModule).GetILGenerator ();

      // expect
      wrappedReference.LoadAddressOfReference (gen);

      _mockRepository.ReplayAll ();

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference, typeof (int));
      tr.LoadAddressOfReference (gen);

      _mockRepository.VerifyAll ();
    }
  }
}
