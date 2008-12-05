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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextTest
  {

    //public class ToTextProviderTypeHandler : ToTextSpecificTypeHandler<ToTextProvider>
    //{
    //  public override void ToText (ToTextProvider t, IToTextBuilder ttb)
    //  {
    //    ttb.ib<ToTextProvider> ().nl ().e (t.Settings).nl ().e (t._typeHandlerMap).ie ();
    //  }
    //}


    [Test]
    [Ignore]
    public void SomeTest ()
    {
      To.ToTextProvider.RegisterSpecificTypeHandler<ToTextProviderSettings> (
        (s, tb) =>
        {
          tb.AllowNewline = false;
          tb.ib<ToTextProviderSettings> ();
          tb.e (() => s.UseAutomaticObjectToText).nl ().e (() => s.EmitPublicProperties).nl ().e (() => s.EmitPublicFields).nl ().e (
            () => s.EmitPrivateProperties).nl ().e (() => s.EmitPrivateFields).nl ().e (() => s.UseAutomaticStringEnclosing).nl ().e (
            () => s.UseAutomaticCharEnclosing).nl ().e (() => s.UseInterfaceHandlers).nl ().e (() => s.ParentHandlerSearchDepth).nl ().e (
            () => s.ParentHandlerSearchUpToRoot).nl ().e (() => s.UseParentHandlers);
          tb.ie ();
        }
      );

      //To.ToTextProvider.RegisterSpecificTypeHandler (typeof (ToTextProvider), new ToTextProviderTypeHandler());

      //To.ToTextProvider.Settings.UseAutomaticObjectToText = true;

      var someToTextProvider = To.ToTextProvider;
      var someToTextProviderSettings = someToTextProvider.Settings;
      //var resultSettings = To.String.e (() => someToTextProviderSettings);
      //To.Console.nl().s (resultSettings.CheckAndConvertToString ());


      //var resultProvider = To.String.e (() => someToTextProvider);
      var resultProvider = To.String.e (someToTextProvider);
      //throw new ArgumentException ();
      To.Console.nl ().s (resultProvider.CheckAndConvertToString ());
    
    }



  }


}
