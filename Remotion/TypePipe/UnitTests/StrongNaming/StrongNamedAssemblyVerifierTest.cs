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
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.TypePipe.StrongNaming;

namespace Remotion.TypePipe.UnitTests.StrongNaming
{
  [TestFixture]
  public class StrongNamedAssemblyVerifierTest
  {
    [Test]
    public void IsStrongNamed ()
    {
      var verifier = new StrongNamedAssemblyVerifier();

      var assembly1 = typeof (StrongNamedAssemblyVerifierTest).Assembly;
      var assembly2 = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName("test1"), AssemblyBuilderAccess.Run);
      var assembly3 = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName("test2"), AssemblyBuilderAccess.Run);
      assembly3.GetName().SetPublicKey (null);
      assembly3.GetName().SetPublicKeyToken (null);

      Assert.That (verifier.IsStrongNamed (assembly1), Is.True);
      Assert.That (verifier.IsStrongNamed (assembly2), Is.False);
      Assert.That (verifier.IsStrongNamed (assembly3), Is.False);
    }
  }
}