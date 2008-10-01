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
using Remotion.Diagnostics.ToText.Internal.ToTextProviderHandler;
using Remotion.Utilities;


namespace Remotion.Diagnostics.ToText.Internal.ToTextProviderHandler
{
  /// <summary>
  /// Base class for all handlers which can be used by <see cref="ToTextProvider"/> in its ToText-fallback-cascade.
  /// Handlers are registered in order of precedence with <see cref="ToTextProvider"/> by calling its (<see cref="ToTextProvider.RegisterToTextProviderHandler{T}"/>-method.
  /// </summary>
  public abstract class ToTextProviderHandler : IToTextProviderHandler
  {
    protected ToTextProviderHandler ()
    {
      Disabled = false;
    }

    protected void Log (string s)
    {
      //Console.WriteLine ("[ToTextProviderHandler]: " + s);
    }

    /// <summary>
    /// Abstract method whose concrete implementations supply conversion into human readable text form (<see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> method) 
    /// of the passed instance for specific classes of types (e.g. types implementing IEnumerable or IFormattable, Strings, etc).
    /// </summary>
    /// <param name="toTextParameters">The instance to convert, type of the instance and <see cref="ToTextBuilder"/> to add the human readable text to.</param>
    /// <param name="toTextProviderHandlerFeedback">If the type was handled by the method, it must set 
    /// the <see cref="ToTextProviderHandlerFeedback.Handled"/> property of the passed argument to <c>true</c>.</param>
    public abstract void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback);

    /// <summary>
    /// Enables/disables the handler. Disabled handlers are skipped by the <see cref="ToTextProvider"/> fallback cascade).
    /// </summary>
    public bool Disabled { get; set; }


    public static void CheckNotNull (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextParameters.CheckNotNull (toTextParameters);
      ArgumentUtility.CheckNotNull ("toTextProviderHandlerFeedback", toTextProviderHandlerFeedback);
    }
  }
}