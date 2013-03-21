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
using NUnit.Framework;

namespace Remotion.TypePipe.IntegrationTests.ObjectFactory
{
  [TestFixture]
  public class ParticipantStateTest : ObjectFactoryIntegrationTestBase
  {
    [Test]
    public void GlobalState ()
    {
      var participant1 = CreateParticipant (ctx =>
      {
        if (ctx.RequestedType == typeof (RequestedType1))
        {
          Assert.That (ctx.State, Is.Empty);
          ctx.State.Add ("key", 7);
        }
        else
        {
          Assert.That (ctx.State["key"], Is.EqualTo (7), "Participant sees state even when requsted types differ.");
        }
      });
      var participant2 = CreateParticipant (ctx => Assert.That (ctx.State["key"], Is.EqualTo (7), "Participant 2 sees state of participant 1."));

      var factory = CreateObjectFactory (participant1, participant2);

      Assert.That (() => factory.CreateObject<RequestedType1>(), Throws.Nothing);
      Assert.That (() => factory.CreateObject<RequestedType2>(), Throws.Nothing);
    }

    public class RequestedType1 {}
    public class RequestedType2 {}
  }
}