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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace TypePipe.IntegrationTests
{
  [Ignore ("TODO 5119")]
  [TestFixture]
  public class AddTypeInitializationTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void Standard ()
    {
      var field = NormalizingMemberInfoFromExpressionUtility.GetField (() => DomainType.StaticField);

      var type = AssembleType<DomainType> (
          mutableType =>
          {
            Assert.That (mutableType.TypeInitializations, Is.Empty);

            var initializationExpression = Expression.Assign (Expression.Field (null, field), Expression.Constant ("abc"));
            mutableType.AddTypeInitialization (initializationExpression);

            Assert.That (mutableType.TypeInitializations, Is.EqualTo (new[] { initializationExpression }));

            // TODO 5119 whats happens if we modify type with a type initializer
            // TODO 5119 what should be returned for mutableType.TypeInitializer
          });

      // Force type to be loaded.
      // TODO 5119: Better alternative?
      Activator.CreateInstance (type);

      Assert.That (DomainType.StaticField, Is.EqualTo ("abc"));
    }

    public class DomainType
    {
      public static string StaticField;
    }
  }
}