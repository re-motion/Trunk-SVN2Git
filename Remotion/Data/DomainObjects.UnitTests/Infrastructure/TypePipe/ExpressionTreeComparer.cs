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
using System.Collections;
using System.Reflection;
using Remotion.TypePipe.Dlr.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  // This class was copied from the TypePipe.UnitTests project (Microsoft.Scripting.Ast expressions).
  public class ExpressionTreeComparer
  {
    public static void CheckAreEqualTrees (Expression expectedTree, Expression actualTree)
    {
      var comparer = new ExpressionTreeComparer (expectedTree.DebugView, actualTree.DebugView);
      comparer.CheckAreEqualNodes (expectedTree, actualTree);
    }

    private readonly object _expectedInitial;
    private readonly object _actualInitial;

    public ExpressionTreeComparer (object expectedInitial, object actualInitial)
    {
      ArgumentUtility.CheckNotNull ("expectedInitial", expectedInitial);
      ArgumentUtility.CheckNotNull ("actualInitial", actualInitial);

      _expectedInitial = expectedInitial;
      _actualInitial = actualInitial;
    }

    public void CheckAreEqualNodes (Expression expected, Expression actual)
    {
      if (expected == null)
        Assert.IsNull (actual, GetMessage (expected, actual, "Null nodes"));
      else
      {
        CheckAreEqualExpressionTypes (expected, actual);
        Assert.AreEqual (expected.NodeType, actual.NodeType, GetMessage (expected, actual, "NodeType"));
        Assert.AreEqual (expected.Type, actual.Type, GetMessage (expected, actual, "Type"));
        CheckAreEqualObjects (expected, actual);
      }
    }

    public void CheckAreEqualObjects (object expected, object actual)
    {
      if (expected is Expression)
      {
        Assert.IsInstanceOf<Expression> (actual, GetMessage (expected, actual, "NodeType"));
        CheckAreEqualExpressionTypes ((Expression) expected, (Expression) actual);
      }
      else
        Assert.AreEqual (expected.GetType(), actual.GetType(), GetMessage (expected, actual, "GetType()"));

      foreach (PropertyInfo property in expected.GetType().GetProperties (BindingFlags.Instance | BindingFlags.Public))
      {
        object value1 = property.GetValue (expected, null);
        object value2 = property.GetValue (actual, null);
        CheckAreEqualProperties (property, property.PropertyType, value1, value2, expected, actual);
      }
    }

    private void CheckAreEqualExpressionTypes (Expression expected, Expression actual)
    {
      if (expected is BlockExpression)
        Assert.IsInstanceOf<BlockExpression> (actual, GetMessage (expected, actual, "NodeType"));
      else if (expected is MethodCallExpression)
        Assert.IsInstanceOf<MethodCallExpression> (actual, GetMessage (expected, actual, "NodeType"));
      else
        Assert.AreEqual (expected.GetType(), actual.GetType(), GetMessage (expected, actual, "NodeType"));
    }

    private void CheckAreEqualProperties (PropertyInfo property, Type valueType, object value1, object value2, object expected, object actual)
    {
      if (typeof (Expression).IsAssignableFrom (valueType))
      {
        Expression subNode1 = (Expression) value1;
        Expression subNode2 = (Expression) value2;
        CheckAreEqualNodes (subNode1, subNode2);
      }
      else if (typeof (MemberBinding).IsAssignableFrom (valueType) || typeof (ElementInit).IsAssignableFrom (valueType))
        CheckAreEqualObjects (value1, value2);
      else if (typeof (IList).IsAssignableFrom (valueType))
      {
        IList list1 = (IList) value1;
        IList list2 = (IList) value2;
        if (list1 == null || list2 == null)
          Assert.AreEqual (list1, list2, "One of the lists in " + property.Name + " is null.");
        else
        {
          Assert.AreEqual (list1.Count, list2.Count, GetMessage (expected, actual, "Number of elements in " + property.Name));
          for (int i = 0; i < list1.Count; ++i)
          {
            var elementType1 = list1[i] != null ? list1[i].GetType() : typeof (object);
            var elementType2 = list2[i] != null ? list2[i].GetType() : typeof (object);

            if (typeof (BlockExpression).IsAssignableFrom (elementType1))
              Assert.That (typeof (BlockExpression).IsAssignableFrom (elementType2));
            else if (typeof (MethodCallExpression).IsAssignableFrom (elementType1))
              Assert.That (typeof (MethodCallExpression).IsAssignableFrom (elementType2));
            else
            {
              Assert.AreSame (
                  elementType1,
                  elementType2,
                  string.Format (
                      "The item types of the items in the lists in {0} differ: One is '{1}', the other is '{2}'.\nExpected tree: {3}\nActual tree: {4}",
                      property.Name,
                      elementType1,
                      elementType2,
                      _expectedInitial,
                      _actualInitial));
            }

            CheckAreEqualProperties (property, elementType1, list1[i], list2[i], expected, actual);
          }
        }
      }
      else if (value1 is NonVirtualCallMethodInfoAdapter && value2 is NonVirtualCallMethodInfoAdapter)
      {
        var adapter1 = (NonVirtualCallMethodInfoAdapter) value1;
        var adapter2 = (NonVirtualCallMethodInfoAdapter) value2;
        var ctorAdapter1 = adapter1.AdaptedMethod as ConstructorAsMethodInfoAdapter;
        var ctorAdapter2 = adapter2.AdaptedMethod as ConstructorAsMethodInfoAdapter;

        Assert.That (
            adapter1.AdaptedMethod == adapter2.AdaptedMethod
            || (ctorAdapter1 != null && ctorAdapter2 != null && ctorAdapter1.AdaptedConstructor == ctorAdapter2.AdaptedConstructor),
            Is.True, "Adapted MethodInfo is not equal (non-virtual or ctor).");
      }
      else if (value1 is MemberInfo && value2 is MemberInfo)
        Assert.That (value1, Is.EqualTo (value2).Using (MemberInfoEqualityComparer<MemberInfo>.Instance), GetMessage (value1, value2, "MemberInfos"));
      else
        Assert.AreEqual (value1, value2, GetMessage (expected, actual, "Property " + property.Name));
    }

    private string GetMessage (object expected, object actual, string context)
    {
      return string.Format (
          "Trees are not equal: {0}\nExpected node: {1}\nActual node: {2}\nExpected tree: {3}\nActual tree: {4}",
          context,
          expected,
          actual,
          _expectedInitial,
          _actualInitial);
    }
  }
}