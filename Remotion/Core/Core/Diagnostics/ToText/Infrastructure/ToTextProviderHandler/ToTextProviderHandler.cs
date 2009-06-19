// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;
using Remotion.Utilities;


namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
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
    /// the <see cref="ToTextProviderHandlerFeedback.Handled"/> property of the passed argument to <see langword="true" />.</param>
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