// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectIDStringSerialization
{
  [TestFixture]
  public class GuidObjectIDValueParserTest
  {
    [Test]
    public void TryParse ()
    {
      object resultValue;
      var success = GuidObjectIDValueParser.Instance.TryParse ("5d09030c-25c2-4735-b514-46333bd28ac8", out resultValue);

      Assert.That (success, Is.True);
      Assert.That (resultValue, Is.EqualTo (new Guid ("5d09030c-25c2-4735-b514-46333bd28ac8")));
    }

    [Test]
    public void TryParse_EmptyValue ()
    {
      object resultValue;
      var success = GuidObjectIDValueParser.Instance.TryParse ("", out resultValue);

      Assert.That (success, Is.False);
      Assert.That (resultValue, Is.Null);
    }

    [Test]
    public void TryParse_NonGuid ()
    {
      object resultValue;
      var success = GuidObjectIDValueParser.Instance.TryParse ("a", out resultValue);

      Assert.That (success, Is.False);
      Assert.That (resultValue, Is.Null);
    }
  }
}