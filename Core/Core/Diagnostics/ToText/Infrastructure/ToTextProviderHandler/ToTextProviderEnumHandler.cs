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
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;
using Remotion.Text.StringExtensions;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles <see cref="Enum"/>s in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText(IToTextBuilder)"/> fallback cascade.
  /// </summary>
  public class ToTextProviderEnumHandler : Infrastructure.ToTextProviderHandler.ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      Infrastructure.ToTextProviderHandler.ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      Enum objAsEnum = obj as Enum;

      if (objAsEnum != null)
      {
        toTextBuilder.WriteRawElementBegin();
        string enumName = objAsEnum.ToString().LeftUntilChar ('|');
        toTextBuilder.WriteRawString (enumName);
        toTextBuilder.WriteRawElementEnd ();
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }
}