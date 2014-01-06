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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class TypePipeCustomAttributeDataWithInheritanceTest
  {
    [Test]
    public void GetCustomAttributes_Inheritance ()
    {
      var type = typeof (NonNestedDomainType);
      var nestedType = typeof (DomainType);
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.Property);
      var @event = typeof (DomainType).GetEvents().Single();

      CheckSimpleAttributeDataInheritance (type);
      CheckSimpleAttributeDataInheritance (nestedType);
      CheckSimpleAttributeDataInheritance (method);
      CheckSimpleAttributeDataInheritance (property);
      CheckSimpleAttributeDataInheritance (@event);
    }

    [Test]
    public void GetCustomAttributes_AttributesOnOriginalMemberAreNotFiltered ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.MethodOnDomainType());
      var customAttributes = TypePipeCustomAttributeData.GetCustomAttributes (member, true);

      var customAttributeTypes = customAttributes.Select (a => a.Type).ToArray();
      Assert.That (customAttributeTypes, Is.EquivalentTo (new[] { typeof (InheritableAttribute), typeof (NonInheritableAttribute) }));
    }

    [Test]
    public void GetCustomAttributes_WithAllowMultipleFiltering_AttributesOnBaseAndDerived ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.AttributesOnBaseAndDerived ());

      var attributes = TypePipeCustomAttributeData.GetCustomAttributes (member, true);

      var attributeTypesAndCtorArgs = attributes.Select (d => new { d.Type, Arg = (string) d.ConstructorArguments.Single () }).ToArray ();
      var expectedAttributeTypesAndCtorArgs =
          new[] 
          {
              new { Type = typeof (InheritableNonMultipleAttribute), Arg = "derived" },
              new { Type = typeof (InheritableAllowMultipleAttribute), Arg = "base" },
              new { Type = typeof (InheritableAllowMultipleAttribute), Arg = "derived" }
          };
      Assert.That (attributeTypesAndCtorArgs, Is.EquivalentTo (expectedAttributeTypesAndCtorArgs));
    }

    [Test]
    public void GetCustomAttributes_WithAllowMultipleFiltering_AttributesOnBaseOnly ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.AttributesOnBaseOnly ());

      var attributes = TypePipeCustomAttributeData.GetCustomAttributes (member, true);

      var attributeTypes = attributes.Select (d => d.Type).ToArray ();
      Assert.That (attributeTypes, Is.EquivalentTo (new[] { typeof (InheritableAllowMultipleAttribute), typeof (InheritableNonMultipleAttribute) }));
    }

    private void CheckSimpleAttributeDataInheritance (MemberInfo member)
    {
      var customAttributesWithoutInheritance = TypePipeCustomAttributeData.GetCustomAttributes (member, false).ToArray ();
      Assert.That (customAttributesWithoutInheritance, Is.Empty);

      var customAttributesWithInheritance = TypePipeCustomAttributeData.GetCustomAttributes (member, true).ToArray ();
      Assert.That (customAttributesWithInheritance, Is.Not.Empty);

      var customAttributeTypesWithInheritance = customAttributesWithInheritance.Select (d => d.Type).ToArray ();
      Assert.That (customAttributeTypesWithInheritance, Is.EqualTo (new[] { typeof (InheritableAttribute) }));
    }

    [Inheritable, NonInheritable]
    public class BaseType
    {
      [Inheritable, NonInheritable]
      public virtual void Method () { }

      [Inheritable, NonInheritable]
      public virtual int Property { get; set; }

      [Inheritable, NonInheritable]
      public virtual event EventHandler Event;

      [InheritableAllowMultiple ("base"), InheritableNonMultiple ("base")]
      public virtual void AttributesOnBaseAndDerived () { }

      [InheritableAllowMultiple ("base"), InheritableNonMultiple ("base")]
      public virtual void AttributesOnBaseOnly () { }
    }

    public class DomainType : BaseType
    {
      public override void Method () { }
      public override int Property { get; set; }
      public override event EventHandler Event;

      [Inheritable, NonInheritable]
      public void MethodOnDomainType () { }

      [InheritableAllowMultiple ("derived"), InheritableNonMultiple ("derived")]
      public override void AttributesOnBaseAndDerived () { }
      public override void AttributesOnBaseOnly () { }
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    public class InheritableAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    public class NonInheritableAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class InheritableAllowMultipleAttribute : Attribute
    {
      public InheritableAllowMultipleAttribute (string arg) { Dev.Null = arg; }
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class InheritableNonMultipleAttribute : Attribute
    {
      public InheritableNonMultipleAttribute (string arg) { Dev.Null = arg; }
    }
  }

  internal class NonNestedDomainType : TypePipeCustomAttributeDataWithInheritanceTest.BaseType { }
}