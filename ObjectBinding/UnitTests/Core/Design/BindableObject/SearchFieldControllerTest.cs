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
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.Design.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.Design.BindableObject
{
  [TestFixture]
  public class SearchFieldControllerTest
  {
    private TextBox _textBox;
    private Button _button;

    [SetUp]
    public void SetUp ()
    {
      _textBox = new TextBox();
      _button = new Button();
    }

    [TearDown]
    public void TearDown ()
    {
      _textBox.Dispose();
      _button.Dispose();
    }

    [Test]
    public void GetButtonIcon ()
    {
      Assert.That (_button.ImageList, Is.Null);

      SearchFieldController controller = new SearchFieldController (_textBox, _button);

      Assert.That (_button.ImageList, Is.Not.Null);
      Assert.That (_button.ImageList.Images.Count, Is.EqualTo (1));
      Assert.That (_button.ImageKey, Is.EqualTo (SearchFieldController.SearchIcons.Search.ToString()));
    }
  }
}
