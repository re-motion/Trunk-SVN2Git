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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;
using System.Linq;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class MethodInfoFromRelatedTypesEqualityComparerTest
  {
    [Test]
    public void Get ()
    {
      Assert.That (MethodInfoFromRelatedTypesEqualityComparer.Get, Is.TypeOf (typeof (MethodInfoFromRelatedTypesEqualityComparer)));
    }

    [Test]
    public void Compare_VirtualMethods ()
    {
      var comparer = MethodInfoFromRelatedTypesEqualityComparer.Get;
      const string methodName = "OverrideMe";
      var proxiedMethod = ScriptingHelper.GetAnyPublicInstanceMethodArray (typeof (Proxied), methodName).Last ();
      var proxiedChildMethod = ScriptingHelper.GetAnyPublicInstanceMethodArray (typeof (ProxiedChild), methodName).Last ();
      var proxiedChildChildChildMethod = ScriptingHelper.GetAnyPublicInstanceMethodArray (typeof (ProxiedChildChildChild), methodName).Last ();
      Assert.That (comparer.Equals (proxiedMethod, proxiedMethod), Is.True);
      Assert.That (comparer.Equals (proxiedMethod, proxiedChildMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildMethod, proxiedMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildMethod, proxiedChildMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildChildChildMethod, proxiedChildChildChildMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildMethod, proxiedChildChildChildMethod), Is.True);
    }

    [Test]
    public void Compare_NewMethods ()
    {
      var comparer = MethodInfoFromRelatedTypesEqualityComparer.Get;
      const string methodName = "PrependName";
      var proxiedMethod = ScriptingHelper.GetAnyPublicInstanceMethodArray (typeof (Proxied), methodName).Last ();
      var proxiedChildMethod = ScriptingHelper.GetAnyPublicInstanceMethodArray (typeof (ProxiedChild), methodName).Last();
      var proxiedChildChildChildMethod = ScriptingHelper.GetAnyPublicInstanceMethodArray (typeof (ProxiedChildChildChild), methodName).Last ();
      Assert.That (comparer.Equals (proxiedMethod, proxiedMethod), Is.True);
      Assert.That (comparer.Equals (proxiedMethod, proxiedChildMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildMethod, proxiedMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildMethod, proxiedChildMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildChildChildMethod, proxiedChildChildChildMethod), Is.True);
      Assert.That (comparer.Equals (proxiedChildMethod, proxiedChildChildChildMethod), Is.True);
    }
  }
}
