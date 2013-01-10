﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.Configuration;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.IntegrationTests
{
  [TestFixture]
  public class StrongNamingTest : ObjectFactoryIntegrationTestBase
  {
    private Type _signedType;
    private Type _unsignedType;

    public override void SetUp ()
    {
      base.SetUp ();

      _signedType = typeof (int);
      _unsignedType = CreateUnsignedType ();
    }

    [Test]
    public void NoStrongName_Default ()
    {
      // Could be strong-named, but isn't - the default is to output assemblies without strong name.
      var participant = CreateParticipant (mt => mt.AddField ("Field", _signedType));

      CheckStrongNaming (participant, forceStrongNaming: false);
    }

    [Test]
    public void NoStrongName_WithField ()
    {
      var participant = CreateParticipant (mt => mt.AddField ("Field", _unsignedType));

      CheckStrongNaming (participant, forceStrongNaming: false);
    }

    // TODO Review: test with custom key file

    [Test]
    public void ForceStrongName ()
    {
      var participant = CreateParticipant (mt => mt.AddField ("Field", _signedType));

      CheckStrongNaming (participant, forceStrongNaming: true);
      // TODO Review: Check that default key is used to sign resulting assembly.
    }

    [Test]
    public void ForceStrongName_MutableTypeInFieldSignature ()
    {
      var participant = CreateParticipant (mt => mt.AddField ("Field", mt));

      CheckStrongNaming (participant, forceStrongNaming: true);
    }

    [Test]
    public void ForceStrongName_MutableTypeInExpression ()
    {
      var participant = CreateParticipant (
          mutableType =>
          {
            var expression = Expression.New (mutableType);
            // TODO 4778
            var usableExpression = Expression.Convert (expression, typeof (DomainType));
            mutableType.AddMethod ("Method", 0, typeof (DomainType), ParameterDeclaration.EmptyParameters, ctx => usableExpression);
          });

      CheckStrongNaming (participant, forceStrongNaming: true);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), MatchType = MessageMatch.Regex, ExpectedMessage =
        "Strong-naming is enabled but a participant used the type 'UnsignedType' which comes from the unsigned assembly 'testAssembly'.")]
    public void ForceStrongName_IncompatibleType_InFieldSignature ()
    {
      SkipSavingAndPeVerification();
      var participant = CreateParticipant (mt => mt.AddField ("Field", _unsignedType));
      var objectFactory = CreateObjectFactoryForStrongNaming (true, 0, participant);

      objectFactory.GetAssembledType (typeof (DomainType));
    }

    // TODO Review: Refactor above test to be one-liner, add tests for (positive and negative case):
    // base type, interface types
    // method parameter, method return type
    // constructor parameter
    // attributes (on constructors, methods, type, fields, parameters, return parameter, later: events, properties - mark TODO 4675 and 4676)
    // later: event type, property type, property index parameter  - mark TODO 4675 and 4676

    [Test]
    [ExpectedException (typeof (InvalidOperationException), MatchType = MessageMatch.Regex, ExpectedMessage =
        "Strong-naming is enabled but a participant used the type 'UnsignedType' which comes from the unsigned assembly 'testAssembly'.")]
    public void ForceStrongName_IncompatibleType_InExpression ()
    {
      SkipSavingAndPeVerification();
      var participant = CreateParticipant (
          mt => mt.AddMethod ("Method", 0, typeof (object), ParameterDeclaration.EmptyParameters, ctx => Expression.New (_unsignedType)));
      var objectFactory = CreateObjectFactoryForStrongNaming (true, 0, participant);

      objectFactory.GetAssembledType (typeof (DomainType));
    }

    // TODO Review: Refactor above test to be one-liner, add tests for each opcode type, catch blocks, local variables (positive and negative case)

    [MethodImpl (MethodImplOptions.NoInlining)]
    private void CheckStrongNaming (IParticipant participant, bool forceStrongNaming)
    {
      var objectFactory = CreateObjectFactoryForStrongNaming (forceStrongNaming, stackFramesToSkip: 1, participants: new[] { participant });

      objectFactory.GetAssembledType (typeof (DomainType));
      var assemblyPath = Flush();

      AppDomainRunner.Run (
          args =>
          {
            var path = (string) args[0];
            var expectedIsStrongNamed = (bool) args[1];
            var assembly = Assembly.LoadFrom (path);

            var isStrongNamed = assembly.GetName().GetPublicKeyToken().Length > 0;
            Assert.That (isStrongNamed, Is.EqualTo (expectedIsStrongNamed));
          },
          assemblyPath,
          forceStrongNaming);
    }

    [MethodImpl (MethodImplOptions.NoInlining)]
    private IObjectFactory CreateObjectFactoryForStrongNaming (bool forceStrongNaming, int stackFramesToSkip, params IParticipant[] participants)
    {
      // TODO Review: Use config section instead of stub. (Use utility class to deserialize section from string.)
      var typePipeConfigurationProviderStub = MockRepository.GenerateStub<ITypePipeConfigurationProvider>();
      typePipeConfigurationProviderStub.Stub (stub => stub.ForceStrongNaming).Return (forceStrongNaming);

      using (new ServiceLocatorScope (typeof (ITypePipeConfigurationProvider), () => typePipeConfigurationProviderStub))
        return CreateObjectFactory (participants, stackFramesToSkip: stackFramesToSkip + 1);
    }

    private Type CreateUnsignedType ()
    {
      var assemblyName = "testAssembly";
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName (assemblyName), AssemblyBuilderAccess.Run);
      var moduleBuilder = assemblyBuilder.DefineDynamicModule (assemblyName + ".dll");
      var typeBuilder = moduleBuilder.DefineType ("UnsignedType");
      var type = typeBuilder.CreateType();

      return type;
    }

    public class DomainType {}
  }
}