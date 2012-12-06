// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Serialization;
using Remotion.TypePipe.Serialization.Implementation;

namespace Remotion.TypePipe.IntegrationTests.Serialization
{
  [Ignore ("5222")]
  [TestFixture]
  public class ComplexSerializationTest : SerializationTestBase
  {
    private const string c_factoryIdentifier = "abc";

    private static SerializationParticipant CreateSerializationParticipant ()
    {
      return new SerializationParticipant (c_factoryIdentifier, new FieldSerializationExpressionBuilder ());
    }

    [MethodImpl (MethodImplOptions.NoInlining)]
    protected override IObjectFactory CreateObjectFactoryForSerialization (params IParticipant[] participants)
    {
      var allParticipants = participants.Concat (CreateSerializationParticipant());
      var factory = CreateObjectFactory (allParticipants, stackFramesToSkip: 1);

      return factory;
    }

    protected override void CheckDeserializationInNewAppDomain (TestContext context)
    {
      // Do not flush.

      AppDomainRunner.Run (
          args =>
          {
            // Register a factory for deserialization in current (new) app domain.
            IObjectFactory factory;
            using (new ServiceLocatorScope (typeof (IParticipant), CreateSerializationParticipant))
              factory = SafeServiceLocator.Current.GetInstance<IObjectFactory>();
            var registry = SafeServiceLocator.Current.GetInstance<IObjectFactoryRegistry>();
            registry.Register (c_factoryIdentifier, factory);

            var ctx = (TestContext) args.Single();
            var deserializedInstance = (SerializableType) Serializer.Deserialize (ctx.SerializedData);

            Assert.That (deserializedInstance.GetType().AssemblyQualifiedName, Is.Not.EqualTo (ctx.ExpectedAssemblyQualifiedName));
            Assert.That (deserializedInstance.GetType().FullName, Is.EqualTo (ctx.ExpectedTypeFullName));
            ctx.Assertions (deserializedInstance, ctx);
          },
          context);
    }
  }
}