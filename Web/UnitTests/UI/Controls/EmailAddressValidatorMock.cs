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
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="EmailAddressValidator"/> type. </summary>
[ToolboxItem (false)]
public class EmailAddressValidatorMock: EmailAddressValidator
{
	public new bool IsMatchComplete (string text)
  {
    return base.IsMatchComplete (text);
  }

	public new bool IsMatchUserPart (string text)
  {
    return base.IsMatchUserPart (text);
  }

	public new bool IsMatchDomainPart (string text)
  {
    return base.IsMatchDomainPart (text);
  }
}

}
