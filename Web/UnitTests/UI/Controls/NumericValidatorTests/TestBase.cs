/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.NumericValidatorTests
{
  public class TestBase
  {
    private NumericValidator _validator;
    private TextBox _textBox;
    private MockRepository _mockRepository;
    private CultureInfo _cultureBackup;


    [SetUp]
    public virtual void SetUp ()
    {
      _mockRepository = new MockRepository();
      _textBox = new TextBox();
      _textBox.ID = "TextBox";
      Control namingContainer = _mockRepository.StrictMultiMock<Control> (typeof (INamingContainer));
      SetupResult.For (namingContainer.FindControl ("TextBox")).Return (_textBox);
      _validator = new NumericValidatorMock (namingContainer);
      _validator.ControlToValidate = _textBox.ID;

      _mockRepository.ReplayAll();
      
      _cultureBackup = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [TearDown]
    public virtual void TearDown ()
    {
      Thread.CurrentThread.CurrentCulture = _cultureBackup;
    }

    protected NumericValidator Validator
    {
      get { return _validator; }
    }

    protected TextBox TextBox
    {
      get { return _textBox; }
    }

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }
  }
}
