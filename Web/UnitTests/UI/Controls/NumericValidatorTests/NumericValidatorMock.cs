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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.NumericValidatorTests
{
  public class NumericValidatorMock : NumericValidator
  {
    private readonly Control _namingContainer;

    public NumericValidatorMock (Control namingContainer)
    {
      ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);
      _namingContainer = namingContainer;
    }

    public override Control NamingContainer
    {
      get { return _namingContainer; }
    }
  }
}
