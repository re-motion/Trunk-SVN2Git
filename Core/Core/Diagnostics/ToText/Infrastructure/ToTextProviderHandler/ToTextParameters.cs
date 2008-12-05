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
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Parameter class used to pass the instance to convert, type of the instance and <see cref="ToTextBuilder"/> to <see cref="ToTextProviderHandler.ToTextIfTypeMatches"/>.
  /// </summary>
  public class ToTextParameters
  {
    public object Object { get; set; }
    public Type Type { get; set; }
    public IToTextBuilder ToTextBuilder { get; set; }

    public ToTextProvider ToTextProvider
    {
      get { return ToTextBuilder.ToTextProvider; }
    }

    public ToTextProviderSettings Settings
    {
      get { return ToTextProvider.Settings; }
    }


    public static void CheckNotNull (ToTextParameters toTextParameters)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters", toTextParameters);
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);
    }
  }
}