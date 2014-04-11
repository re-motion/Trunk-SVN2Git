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
using Remotion.Linq.Parsing.ExpressionTreeVisitors.MemberBindings;

namespace Remotion.Linq.UnitTests.Parsing.ExpressionTreeVisitors.MemberBindings
{
  [TestFixture]
  public class MemberBindingTest : MemberBindingTestBase
  {
    [Test]
    public void Bind_Method ()
    {
      var binding = MemberBinding.Bind (Method, AssociatedExpression);

      Assert.That (binding, Is.InstanceOf (typeof (MethodInfoBinding)));
      Assert.That (binding.BoundMember, Is.SameAs (Method));
      Assert.That (binding.AssociatedExpression, Is.SameAs (AssociatedExpression));
    }

    [Test]
    public void Bind_Property ()
    {
      var binding = MemberBinding.Bind (Property, AssociatedExpression);

      Assert.That (binding, Is.InstanceOf (typeof (PropertyInfoBinding)));
      Assert.That (binding.BoundMember, Is.SameAs (Property));
      Assert.That (binding.AssociatedExpression, Is.SameAs (AssociatedExpression));
    }

    [Test]
    public void Bind_Field ()
    {
      var binding = MemberBinding.Bind (Field, AssociatedExpression);

      Assert.That (binding, Is.InstanceOf (typeof (FieldInfoBinding)));
      Assert.That (binding.BoundMember, Is.SameAs (Field));
      Assert.That (binding.AssociatedExpression, Is.SameAs (AssociatedExpression));
    }
  }
}
