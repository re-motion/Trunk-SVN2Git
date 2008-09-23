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
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
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