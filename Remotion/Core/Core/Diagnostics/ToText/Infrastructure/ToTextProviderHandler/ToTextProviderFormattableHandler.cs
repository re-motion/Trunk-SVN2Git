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
using System.Globalization;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles instances implementing the <see cref="IFormattable"/> interface in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// This handler takes care of all primitive data types and prevents them from being treated by the <see cref="ToTextProviderAutomaticObjectToTextHandler"/> handler,
  /// which should always come after it.
  /// </summary>
  public class ToTextProviderFormattableHandler : ToTextProviderHandler
  {
    private static readonly CultureInfo s_cultureInfoInvariant = CultureInfo.InvariantCulture;

    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);


      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if(obj is IFormattable)
      {
        toTextBuilder.WriteRawElementBegin ();
        IFormattable formattable = (IFormattable) obj;
        //toTextBuilder.AppendString (StringUtility.Format (obj, s_cultureInfoInvariant));
        toTextBuilder.WriteRawString (formattable.ToString (null, s_cultureInfoInvariant));
        toTextProviderHandlerFeedback.Handled = true;
        toTextBuilder.WriteRawElementEnd ();
      }
    }
  }
}
