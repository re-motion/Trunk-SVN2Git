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
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Fluent interface for <see cref="FinishInputWithAction"/>s which are directly supported by the framework.
  /// </summary>
  public static class FinishInput
  {
    public static readonly FinishInputWithAction Promptly = s => { };
    public static readonly FinishInputWithAction WithTab = s => s.SendKeysFixed (Keys.Tab);
    // Todo RM-6297: Why does PressEnter not trigger an auto postback in IE? Is this a bug? See BocListCO.GoToSpecificPage().
    public static readonly FinishInputWithAction WithEnter = s => s.SendKeysFixed (Keys.Enter);
  }
}