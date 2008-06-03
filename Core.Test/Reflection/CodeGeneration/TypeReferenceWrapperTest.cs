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
      Reference ownerReference = _mockRepository.CreateMock<Reference>();
      Reference wrappedReference = _mockRepository.CreateMock<Reference> (ownerReference);

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference, typeof (int));
      Assert.AreEqual (typeof (int), tr.Type);
      Assert.AreSame (ownerReference, tr.OwnerReference);
    }

    [Test]
    public void LoadReference ()
    {
      Reference wrappedReference = _mockRepository.CreateMock<Reference> ();
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
      Reference wrappedReference = _mockRepository.CreateMock<Reference> ();
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
      Reference wrappedReference = _mockRepository.CreateMock<Reference> ();
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
