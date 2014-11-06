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
using System.Web.UI.WebControls;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="Label"/> and all its derivatives (e.g. <see cref="T:Remotion.Web.UI.Controls.SmartLabel"/>).
  /// </summary>
  [UsedImplicitly]
  public class LabelControlObject : WebFormsControlObject, IControlObjectWithText
  {
    public LabelControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetText ()
    {
      return Scope.Text.Trim();
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      throw new NotSupportedException ("The LabelControlObject does not support any interaction.");
    }
  }
}