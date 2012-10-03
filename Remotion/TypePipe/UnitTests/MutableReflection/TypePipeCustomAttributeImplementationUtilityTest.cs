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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Development.UnitTesting.Reflection;
using SUT = Remotion.TypePipe.MutableReflection.TypePipeCustomAttributeImplementationUtility;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class TypePipeCustomAttributeImplementationUtilityTest
  {
    private MemberInfo _member;
    private bool _randomInherit;

    [SetUp]
    public void SetUp ()
    {
      _member = NormalizingMemberInfoFromExpressionUtility.GetMember (() => Member());
      _randomInherit = BooleanObjectMother.GetRandomBoolean();
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var result = SUT.GetCustomAttributes (_member, _randomInherit);

      Assert.That (result, Has.Length.EqualTo (1));
      var attribute = result.Single ();
      Assert.That (attribute, Is.TypeOf<DerivedAttribute> ());
    }

    [Test]
    public void GetCustomAttributes_NewInstance ()
    {
      var attribute1 = SUT.GetCustomAttributes (_member, _randomInherit).Single();
      var attribute2 = SUT.GetCustomAttributes (_member, _randomInherit).Single ();

      Assert.That (attribute1, Is.Not.SameAs (attribute2));
    }

    [Test]
    public void GetCustomAttributes_Filter ()
    {
      Assert.That (SUT.GetCustomAttributes (_member, typeof (UnrelatedAttribute), _randomInherit), Is.Empty);
      Assert.That (SUT.GetCustomAttributes (_member, typeof (DerivedAttribute), _randomInherit), Has.Length.EqualTo (1));
      Assert.That (SUT.GetCustomAttributes (_member, typeof (BaseAttribute), _randomInherit), Has.Length.EqualTo (1));
      Assert.That (SUT.GetCustomAttributes (_member, typeof (IBaseAttributeInterface), _randomInherit), Has.Length.EqualTo (1));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (SUT.IsDefined (_member, typeof (UnrelatedAttribute), _randomInherit), Is.False);
      Assert.That (SUT.IsDefined (_member, typeof (DerivedAttribute), _randomInherit), Is.True);
      Assert.That (SUT.IsDefined (_member, typeof (BaseAttribute), _randomInherit), Is.True);
      Assert.That (SUT.IsDefined (_member, typeof (IBaseAttributeInterface), _randomInherit), Is.True);
    }

    [Derived]
    void Member () { }

    interface IBaseAttributeInterface { }
    class BaseAttribute : Attribute, IBaseAttributeInterface { }
    class DerivedAttribute : BaseAttribute { }
    class UnrelatedAttribute : Attribute { }
  }
}