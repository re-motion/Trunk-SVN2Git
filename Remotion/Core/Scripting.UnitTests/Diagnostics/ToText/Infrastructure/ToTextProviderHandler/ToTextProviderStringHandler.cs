// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles <see cref="String"/>s in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// If <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/> is true it wraps strings in double quotes.
  /// </summary>
  public class ToTextProviderStringHandler : Infrastructure.ToTextProviderHandler.ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      Infrastructure.ToTextProviderHandler.ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (type == typeof (string))
      {
        //toTextBuilder.HandlerBeforeAppendElement ();
        toTextBuilder.WriteRawElementBegin();
        string s= (string) obj;
        if (settings.UseAutomaticStringEnclosing)
        {
          toTextBuilder.WriteRawChar ('"');
          toTextBuilder.WriteRawString (s);
          toTextBuilder.WriteRawChar ('"');
        }
        else
        {
          toTextBuilder.WriteRawString(s);
        }
        //toTextBuilder.HandlerAfterAppendElement ();
        toTextBuilder.WriteRawElementEnd ();
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }
}
