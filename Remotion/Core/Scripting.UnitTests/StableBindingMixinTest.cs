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
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Reflection;
using Remotion.Scripting.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingMixinTest
  {
    [SetUp]
    public void SetUp ()
    {
      ScriptContextTestHelper.ClearScriptContexts ();
    }


    // Create Script Context which filters IAmbigous1
    // Create class MixinTest which explicitely implements IAmbigous1 & 2
    // Check that MixinTest is ambigous in script
    // Create MixinTestChild derived from MixinTest which is mixed w StableBindingMixin
    // Check that MixinTestChild is not ambigous in script
    
    [Test]
    [ExpectedException (ExceptionType = typeof (MissingMemberException), ExpectedMessage = "'MixinTest' object has no attribute 'StringTimes'")]
    public void MixinTest_IsAmbigous ()
    {
      var mixinTest = new MixinTest();
      ScriptingHelper.ExecuteScriptExpression<string> ("p0.StringTimes('intj',3)", mixinTest);
    }

    [Test]
    [Ignore ("Mixing of GetCustomMember not working. Check w FS.")]
    public void MixinTestChild_IsNotAmbigous ()
    {
      ScriptContext scriptContext = ScriptContext.Create ("StableBindingMixinTest.MixinTestChild_IsNotAmbigous", 
        new TypeLevelTypeFilter (typeof(IAmbigous1)));
      ScriptContext.SwitchAndHoldScriptContext (scriptContext);

      //var mixinTestChild = new MixinTestChild ();

      var mixinTestChild = ObjectFactory.Create<MixinTestChild> (ParamList.Empty);
      //To.ConsoleLine.e ("mixinTestChild.GetType().GetAllMethods()", mixinTestChild.GetType ().GetAllMethods ().Where (mi => mi.Name == "GetCustomMember"));

      var result = ScriptingHelper.ExecuteScriptExpression<string> ("p0.StringTimes('intj',3)", mixinTestChild);
      Assert.That (result, Is.EqualTo ("IAmbigous1.StringTimesintjintjintj"));
      
      ScriptContext.ReleaseScriptContext (scriptContext);
    }  
  }

  public class MixinTest : IAmbigous1, IAmbigous2
  {
    string IAmbigous1.StringTimes (string text, int number)
    {
      return "IAmbigous1.StringTimes" + StringTimes (text, number);
    }
    
    string IAmbigous2.StringTimes (string text, int number)
    {
      return "IAmbigous2.StringTimes" + StringTimes (text, number);
    }

    private string StringTimes (string text, int number)
    {
      return text.ToSequence (number).Aggregate ((sa, s) => sa + s);
    }
  }

  public class MixinTestChild : MixinTest
  {
    //[SpecialName]
    //public object GetCustomMember (string name)
    //{
    //  return ScriptContext.Current.StableBindingProxyProvider.GetMemberProxy (this, name);
    //}
  }

  [Extends (typeof (MixinTestChild))]
  public class StableBindingMixinForMixinTestChild //: StableBindingMixin<MixinTestChild>, IStableBindingMixin
  {
    [SpecialName]
    public object GetCustomMember (string name)
    {
      To.ConsoleLine.nl(2).s("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!").e (() => name).nl(2);
      return ScriptContext.Current.StableBindingProxyProvider.GetMemberProxy (this, name);
    }
  }
}