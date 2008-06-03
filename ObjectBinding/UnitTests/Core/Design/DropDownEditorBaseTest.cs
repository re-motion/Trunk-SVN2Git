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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.Design;

namespace Remotion.ObjectBinding.UnitTests.Core.Design
{
  [TestFixture]
  public class DropDownEditorBaseTest
  {
    private MockRepository _mockRepository;
    private ITypeDescriptorContext _mockTypeDescriptorContext;
    private IServiceProvider _mockServiceProvider;
    private IWindowsFormsEditorService _mockWindowsFormsEditorService;
    private MockDropDownEditorBase _mockDropDownEditorBase;
    private EditorControlBase _mockEditorControlBase;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _mockTypeDescriptorContext = _mockRepository.CreateMock<ITypeDescriptorContext>();
      _mockServiceProvider = _mockRepository.CreateMock<IServiceProvider>();
      _mockWindowsFormsEditorService = _mockRepository.CreateMock<IWindowsFormsEditorService>();
      _mockDropDownEditorBase = _mockRepository.PartialMock<MockDropDownEditorBase>();
      _mockEditorControlBase = _mockRepository.PartialMock<EditorControlBase> (_mockServiceProvider, _mockWindowsFormsEditorService);
    }

    [Test]
    public void EditValue ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object());
      Expect.Call (_mockServiceProvider.GetService (typeof (IWindowsFormsEditorService))).Return (_mockWindowsFormsEditorService);
      Expect.Call (_mockDropDownEditorBase.NewCreateEditorControl (_mockTypeDescriptorContext, _mockWindowsFormsEditorService))
          .Return (_mockEditorControlBase);
      using (_mockRepository.Ordered())
      {
        _mockEditorControlBase.Value = "The input value";
        _mockWindowsFormsEditorService.DropDownControl (_mockEditorControlBase);
        Expect.Call (_mockEditorControlBase.Value).Return ("The output value");
      }
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The output value"));
    }

    [Test]
    public void EditValue_WithoutTypeDescriptorContextInstance ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (null);
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WithoutTypeDescriptorContext ()
    {
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (null, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WithoutServiceProvider ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object());
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, null, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void EditValue_WitoutEditorSerivce ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object());
      Expect.Call (_mockServiceProvider.GetService (typeof (IWindowsFormsEditorService))).Return (null);
      _mockRepository.ReplayAll();

      object actual = _mockDropDownEditorBase.EditValue (_mockTypeDescriptorContext, _mockServiceProvider, "The input value");

      _mockRepository.VerifyAll();

      Assert.That (actual, Is.EqualTo ("The input value"));
    }

    [Test]
    public void GetEditStyle ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (new object ());
      _mockRepository.ReplayAll ();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.GetEditStyle (_mockTypeDescriptorContext);

      _mockRepository.VerifyAll ();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.DropDown));
    }

    [Test]
    public void GetEditStyle_WithoutTypeDescriptorContextInstance ()
    {
      Expect.Call (_mockTypeDescriptorContext.Instance).Return (null);
      _mockRepository.ReplayAll ();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.GetEditStyle (_mockTypeDescriptorContext);

      _mockRepository.VerifyAll ();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.None));
    }

    [Test]
    public void GetEditStyle_WithoutTypeDescriptorContext ()
    {
      _mockRepository.ReplayAll ();

      UITypeEditorEditStyle actual = _mockDropDownEditorBase.GetEditStyle (null);

      _mockRepository.VerifyAll ();

      Assert.That (actual, Is.EqualTo (UITypeEditorEditStyle.None));
    }
  }
}
