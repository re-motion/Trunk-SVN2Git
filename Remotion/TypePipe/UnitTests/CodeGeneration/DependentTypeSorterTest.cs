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
using NUnit.Framework;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.TypePipe.UnitTests.CodeGeneration
{
  [TestFixture]
  public class DependentTypeSorterTest
  {
    private DependentTypeSorter _sorter;

    [SetUp]
    public void SetUp ()
    {
      _sorter = new DependentTypeSorter();
    }

    [Test]
    public void Sort_BaseType ()
    {
      var baseType = MutableTypeObjectMother.Create();
      var derivedType = MutableTypeObjectMother.Create (baseType: baseType);

      var result = _sorter.Sort (new[] { derivedType, baseType }.AsOneTime());

      Assert.That (result, Is.EqualTo (new[] { baseType, derivedType }));
    }

    [Test]
    public void Sort_GetInterfaces ()
    {
      var interface_ = MutableTypeObjectMother.CreateInterface();
      var type = MutableTypeObjectMother.Create();
      type.AddInterface (interface_);

      var result = _sorter.Sort (new[] { type, interface_ });

      Assert.That (result, Is.EqualTo (new[] { interface_, type }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "MutableTypes must not contain cycles in their dependencies, i.e., an algorithm that recursively follows the types returned by "
        + "Type.BaseType and Type.GetInterfaces must terminate.")]
    public void Sort_ThrowsForCycles ()
    {
      var interface1 = MutableTypeObjectMother.CreateInterface ();
      var interface2 = MutableTypeObjectMother.CreateInterface ();
      interface1.AddInterface (interface2);
      interface2.AddInterface (interface1);

      _sorter.Sort (new[] { interface1, interface2 }).ForceEnumeration();
    }
  }
}