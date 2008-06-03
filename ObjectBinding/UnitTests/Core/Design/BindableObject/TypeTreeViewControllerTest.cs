/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
