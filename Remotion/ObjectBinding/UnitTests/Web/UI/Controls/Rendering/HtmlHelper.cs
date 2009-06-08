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
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering
{
  public class HtmlHelper : HtmlHelperBase
  {
    public HtmlHelper ()
        : base (Assert.AreEqual, Assert.Greater, Assert.IsNotNull, Assert.IsNull, Assert.IsTrue)
    {
    }

    public void AssertIcon (XmlNode parentNode, IBusinessObject businessObject, string imageSourcePart)
    {
      XmlNode img = GetAssertedChildElement (parentNode, "img", 0);
      if (imageSourcePart == null)
      {
        string businessObjectClass = businessObject.BusinessObjectClass.Identifier;
        imageSourcePart = businessObjectClass.Substring (0, businessObjectClass.IndexOf (", "));
      }
      AssertAttribute (img, "src", imageSourcePart, AttributeValueCompareMode.Contains);
      AssertAttribute (img, "width", "16px");
      AssertAttribute (img, "height", "16px");
      AssertAttribute (img, "alt", "");
      AssertStyleAttribute (img, "vertical-align", "middle");
      AssertStyleAttribute (img, "border-style", "none");
    }
  }
}