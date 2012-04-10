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
using Microsoft.Scripting.Ast;
using NUnit.Framework;

namespace Remotion.TypePipe.UnitTests.MutableReflection.ExpressionTreeIntegration
{
  [TestFixture]
  public class MutablePropertyInfoTest
  {
    [Test]
    public void Property_Read_Static ()
    {
      var getMethod = MutableMethodInfoObjectMother.Create (methodAttributes: MethodAttributes.Static);
      var property = MutablePropertyInfoObjectMother.Create (getMethod: getMethod);

      var expression = Expression.Property (null, property);

      Assert.That (expression.Member, Is.SameAs (property));
    }

    [Test]
    public void Property_Read_Instance ()
    {
      var declaringType = MutableTypeObjectMother.Create();
      var instance = Expression.Variable (declaringType);
      var property = MutablePropertyInfoObjectMother.Create (declaringType: declaringType);

      var expression = Expression.Property (instance, property);

      Assert.That (expression.Member, Is.SameAs (property));
    }

    [Test]
    public void Property_Write_Static ()
    {
      var propertyType = MutableTypeObjectMother.Create();
      var setMethod = MutableMethodInfoObjectMother.Create (methodAttributes: MethodAttributes.Static);
      var property = MutablePropertyInfoObjectMother.Create (propertyType: propertyType, setMethod: setMethod);
      var value = Expression.Variable (propertyType);

      var propertyExpression = Expression.Property (null, property);
      var expression = Expression.Assign (propertyExpression, value);

      Assert.That (propertyExpression.Member, Is.SameAs (property));
      Assert.That (expression.Left, Is.SameAs (propertyExpression));
    }

    [Test]
    public void Property_Write_Instance ()
    {
      var declaringType = MutableTypeObjectMother.Create();
      var instance = Expression.Variable (declaringType);
      var propertyType = MutableTypeObjectMother.Create();
      var setMethod = MutableMethodInfoObjectMother.Create();
      var property = MutablePropertyInfoObjectMother.Create (declaringType: declaringType, propertyType: propertyType, setMethod: setMethod);
      var value = Expression.Variable (propertyType);

      var propertyExpression = Expression.Property (instance, property);
      var expression = Expression.Assign (propertyExpression, value);

      Assert.That (propertyExpression.Member, Is.SameAs (property));
      Assert.That (expression.Left, Is.SameAs (propertyExpression));
    }
  }
}