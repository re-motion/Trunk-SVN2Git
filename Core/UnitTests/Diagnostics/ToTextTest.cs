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

    //public class ToTextProviderTypeHandler : ToTextSpecificTypeHandler<ToTextProvider>
    //{
    //  public override void ToText (ToTextProvider t, IToTextBuilderBase ttb)
    //  {
    //    ttb.ib<ToTextProvider> ().nl ().e (t.Settings).nl ().e (t._typeHandlerMap).ie ();
    //  }
    //}


    [Test]
    public void SomeTest ()
    {
      To.ToTextProvider.RegisterSpecificTypeHandler<ToTextProviderSettings> (
        (s, tb) => {
          tb.AllowNewline = false;
          tb.ib<ToTextProviderSettings>();
          tb.e (() => s.UseAutomaticObjectToText).nl().e (() => s.EmitPublicProperties).nl().e (() => s.EmitPublicFields).nl().e (
            () => s.EmitPrivateProperties).nl().e (() => s.EmitPrivateFields).nl().e (() => s.UseAutomaticStringEnclosing).nl().e (
            () => s.UseAutomaticCharEnclosing).nl().e (() => s.UseInterfaceHandlers).nl().e (() => s.ParentHandlerSearchDepth).nl().e (
            () => s.ParentHandlerSearchUpToRoot).nl().e (() => s.UseParentHandlers);
          tb.ie(); 
        }
      );

      //To.ToTextProvider.RegisterSpecificTypeHandler (typeof (ToTextProvider), new ToTextProviderTypeHandler());

      //To.ToTextProvider.Settings.UseAutomaticObjectToText = true;

      var someToTextProvider = To.ToTextProvider;
      var someToTextProviderSettings = someToTextProvider.Settings;
      var resultSettings = To.String.e (() => someToTextProviderSettings);
      To.Console.nl().s (resultSettings.CheckAndConvertToString ());

      var resultProvider = To.String.e (() => someToTextProvider);
      To.Console.nl().s (resultProvider.CheckAndConvertToString ());
    
    }



  }


}