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
using NUnit.Framework;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class RenderUtilityTest
  {
    [Test]
    public void JoinLinesWithEncoding_WithEmptySequence_ReturnsEmptyString ()
    {
      Assert.That (
          RenderUtility.JoinLinesWithEncoding (Enumerable.Empty<string>()),
          Is.EqualTo (""));
    }

    [Test]
    public void JoinLinesWithEncoding_WithSingleItem_ReturnsString ()
    {
      Assert.That (
          RenderUtility.JoinLinesWithEncoding (new[] { "First" }),
          Is.EqualTo ("First"));
    }

    [Test]
    public void JoinLinesWithEncoding_WithMultipleItems_ReturnsConcatenatedString ()
    {
      Assert.That (
          RenderUtility.JoinLinesWithEncoding (new[] { "First", "Second" }),
          Is.EqualTo ("First<br />Second"));
    }

    [Test]
    public void JoinLinesWithEncoding_WithSingleItemAndRequiringEncoding_ReturnsEncodedString ()
    {
      Assert.That (
          RenderUtility.JoinLinesWithEncoding (new[] { "Fir<html>st" }),
          Is.EqualTo ("Fir&lt;html&gt;st"));
    }

    [Test]
    public void JoinLinesWithEncoding_WithMultipleItemsAndRequiringEncoding_ReturnsConcatenatedAndEncodedString ()
    {
      Assert.That (
          RenderUtility.JoinLinesWithEncoding (new[] { "Fir<html>st", "Second" }),
          Is.EqualTo ("Fir&lt;html&gt;st<br />Second"));
    }
  }
}