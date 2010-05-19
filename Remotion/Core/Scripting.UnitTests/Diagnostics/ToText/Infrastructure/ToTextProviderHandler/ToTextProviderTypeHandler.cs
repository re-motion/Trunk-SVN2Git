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
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles <see cref="Type"/> instances in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// </summary>
  public class ToTextProviderTypeHandler : Infrastructure.ToTextProviderHandler.ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      Infrastructure.ToTextProviderHandler.ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      // Catch Type|s to avoid endless recursion. 
      if (obj is Type) 
      {
        toTextBuilder.WriteRawElementBegin ();
        toTextBuilder.WriteRawString (obj.ToString ());
        toTextBuilder.WriteRawElementEnd ();
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}
