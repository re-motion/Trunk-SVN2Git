// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.Design.BindableObject
{
  [TestFixture]
  public class TypeTreeViewControllerTest
  {
    private TreeView _treeView;

    [SetUp]
    public void SetUp ()
    {
      _treeView = new TreeView();
    }

    [TearDown]
    public void TearDown ()
    {
      _treeView.Dispose();
    }

    [Test]
    public void GetTreeViewIcons ()
    {
      Assert.That (_treeView.ImageList, Is.Null);

      TypeTreeViewController controller = new TypeTreeViewController (_treeView);
      
      Assert.That (_treeView.ImageList, Is.Not.Null);
      Assert.That (_treeView.ImageList.Images.Count, Is.EqualTo (3));
    }
  }
}
