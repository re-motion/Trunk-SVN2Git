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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;
using Remotion.Diagnostics.ToText;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextTest
  {
    // ToTextProvider
    // ToTextProviderSettings

    public class ToTextProviderTypeHandler : ToTextSpecificTypeHandler<ToTextProvider>
    {
      public override void ToText (ToTextProvider t, IToTextBuilderBase ttb)
      {
        ttb.ib<ToTextProvider> ().e (t.Settings).ie ();
      }
    }



    [Test]
    public void SomeTest ()
    {
      To.ToTextProvider.RegisterSpecificTypeHandler<ToTextProviderSettings> (
        (s, tb) => {
          tb.ib<ToTextProviderSettings>();
          tb.ie(); 
        }
      );

      To.ToTextProvider.RegisterSpecificTypeHandler (typeof (ToTextProvider), new ToTextProviderTypeHandler());

      var someToTextProvider = To.ToTextProvider;
      var someToTextProviderSettings = someToTextProvider.Settings;
      var resultSettings = To.String.e (() => someToTextProviderSettings);
      To.Console.nl().s (resultSettings.CheckAndConvertToString ());

      var resultProvider = To.String.e (() => someToTextProvider);
      To.Console.nl().s (resultProvider.CheckAndConvertToString ());
    
    }



  }


}