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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Sorting
{
  [TestFixture]
  public class BusinessObjectPropertyPathBasedComparerTest
  {
    [Test]
    public void Compare_StringValues ()
    {
      var valueA = TypeWithAllDataTypes.Create ();
      valueA.String = "A";

      var valueB = TypeWithAllDataTypes.Create ();
      valueB.String = "B";

      var valueNull = TypeWithAllDataTypes.Create ();
      valueNull.String = null;

      var bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (TypeWithAllDataTypes));


      var propertyPath = BusinessObjectPropertyPath.CreateStatic (bindableObjectClass, "String");
      var comparer = new BusinessObjectPropertyPathBasedComparer (propertyPath);

      AssertCompare (comparer, valueA, valueNull, valueB);
    }

    [Test]
    public void Compare_ReferenceValues ()
    {
      var valueA = TypeWithReference.Create();
      valueA.ReferenceValue = TypeWithReference.Create ("A");

      var valueB = TypeWithReference.Create();
      valueB.ReferenceValue=TypeWithReference.Create ("B");

      var valueNull = TypeWithReference.Create ();
      valueNull.ReferenceValue = null;

      var bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (TypeWithReference));

      var propertyPath = BusinessObjectPropertyPath.CreateStatic (bindableObjectClass, "ReferenceValue");
      var comparer = new BusinessObjectPropertyPathBasedComparer (propertyPath);

      AssertCompare (comparer, valueA, valueNull, valueB);
    }
    
    private void AssertCompare (BusinessObjectPropertyPathBasedComparer comparer, object valueA, object valueNull, object valueB)
    {
      CompareEqualValues (comparer, (IBusinessObject) valueA, (IBusinessObject) valueA);
      CompareEqualValues (comparer, (IBusinessObject) valueNull, (IBusinessObject) valueNull);

      CompareAscendingValues (comparer, (IBusinessObject) valueA, (IBusinessObject) valueB);
      CompareAscendingValues (comparer, (IBusinessObject) valueNull, (IBusinessObject) valueA);

      CompareDescendingValues (comparer, (IBusinessObject) valueB, (IBusinessObject) valueA);
      CompareDescendingValues (comparer, (IBusinessObject) valueA, (IBusinessObject) valueNull);
    }

    private void CompareEqualValues (IComparer<BocListRow> comparer, IBusinessObject left, IBusinessObject right)
    {
      var rowLeft = new BocListRow (0, left);
      var rowRight = new BocListRow (0, right);

      int compareResultLeftRight = comparer.Compare (rowLeft, rowRight);
      int compareResultRightLeft = comparer.Compare (rowRight, rowLeft);

      Assert.IsTrue (compareResultLeftRight == 0, "Left - Right != zero");
      Assert.IsTrue (compareResultRightLeft == 0, "Right - Left != zero");
    }

    private void CompareAscendingValues (IComparer<BocListRow> comparer, IBusinessObject left, IBusinessObject right)
    {
      var rowLeft = new BocListRow (0, left);
      var rowRight = new BocListRow (0, right);

      int compareResultLeftRight = comparer.Compare (rowLeft, rowRight);
      int compareResultRightLeft = comparer.Compare (rowRight, rowLeft);

      Assert.IsTrue (compareResultLeftRight < 0, "Left - Right <= zero.");
      Assert.IsTrue (compareResultRightLeft > 0, "Right - Left >= zero.");
    }

    private void CompareDescendingValues (IComparer<BocListRow> comparer, IBusinessObject left, IBusinessObject right)
    {
      var rowLeft = new BocListRow (0, left);
      var rowRight = new BocListRow (0, right);

      int compareResultLeftRight = comparer.Compare (rowLeft, rowRight);
      int compareResultRightLeft = comparer.Compare (rowRight, rowLeft);

      Assert.IsTrue (compareResultLeftRight > 0, "Right - Left >= zero.");
      Assert.IsTrue (compareResultRightLeft < 0, "Left - Right <= zero.");
    }
  }
}