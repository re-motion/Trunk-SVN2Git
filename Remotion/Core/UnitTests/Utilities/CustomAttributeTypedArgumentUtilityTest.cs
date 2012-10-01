// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class CustomAttributeTypedArgumentUtilityTest
  {
    [Test]
    [Domain ("simple")]
    public void Unwrap_Simple ()
    {
      var typedArgument = GetTypedArgument (MethodBase.GetCurrentMethod());

      var result = CustomAttributeTypedArgumentUtility.Unwrap (typedArgument);

      Assert.That (result, Is.EqualTo ("simple"));
    }

    [Test]
    [Domain (MyEnum.C)]
    public void Unwrap_Enum ()
    {
      var typedArgument = GetTypedArgument (MethodBase.GetCurrentMethod());

      var result = CustomAttributeTypedArgumentUtility.Unwrap (typedArgument);

      Assert.That (result, Is.EqualTo (MyEnum.C));
    }

    [Test]
    [Domain (new[] { 1, 2, 3 })]
    public void Unwrap_Array ()
    {
      var typedArgument = GetTypedArgument (MethodBase.GetCurrentMethod());

      var result = CustomAttributeTypedArgumentUtility.Unwrap (typedArgument);

      Assert.That (result, Is.TypeOf<int[]>());
      Assert.That (result, Is.EqualTo (new[] { 1, 2, 3 }));
    }

    [Test]
    [Domain (new object[] { "s", 7, new[] { MyEnum.B, MyEnum.A }, typeof (int), new[] { 4, 5 } })]
    public void Unwrap_Recursive ()
    {
      var typedArgument = GetTypedArgument (MethodBase.GetCurrentMethod());

      var result = CustomAttributeTypedArgumentUtility.Unwrap (typedArgument);

      Assert.That (result, Is.TypeOf<object[]>());
      var array = ((object[]) result);
      Assert.That (array[2], Is.TypeOf<MyEnum[]>());
      Assert.That (array[4], Is.TypeOf<int[]>());
      Assert.That (array, Is.EqualTo (new object[] { "s", 7, new[] { MyEnum.B, MyEnum.A }, typeof (int), new[] { 4, 5 } }));
    }

    [Test]
    [Domain (null)]
    public void Unwrap_Null ()
    {
      var typedArgument = GetTypedArgument (MethodBase.GetCurrentMethod());

      var result = CustomAttributeTypedArgumentUtility.Unwrap (typedArgument);

      Assert.That (result, Is.Null);
    }

    [Test]
    [Domain (new[] { "1", "2", null })]
    public void Unwrap_RecursiveNull ()
    {
      var typedArgument = GetTypedArgument (MethodBase.GetCurrentMethod());

      var result = CustomAttributeTypedArgumentUtility.Unwrap (typedArgument);

      Assert.That (result, Is.EqualTo (new[] { "1", "2", null }));
    }

    private CustomAttributeTypedArgument GetTypedArgument (MethodBase method)
    {
      return CustomAttributeData.GetCustomAttributes (method)
          .Single (a => a.Constructor.DeclaringType == typeof (DomainAttribute))
          .ConstructorArguments.Single();
    }

    private class DomainAttribute : Attribute
    {
      public DomainAttribute (object obj)
      {
        Dev.Null = obj;
      }
    }

    private enum MyEnum { A, B, C }
  }
}