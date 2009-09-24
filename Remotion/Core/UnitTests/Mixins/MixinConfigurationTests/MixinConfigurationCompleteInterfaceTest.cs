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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationCompleteInterfaceTest
  {
    [Test]
    public void AddClassContextCausesInterfaceToBeRegistered ()
    {
      var ac = new MixinConfiguration ();
      ClassContext cc = new ClassContextBuilder (typeof (BaseType2))
          .AddCompleteInterface (typeof (IBaseType2))
          .BuildClassContext ();
      ac.ClassContexts.Add (cc);
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "There is an ambiguity in complete interfaces: interface "
        + "'System.IServiceProvider' refers to both class ", MatchType = MessageMatch.Contains)] // cannot write the full message, order is undefined
    public void RegisterCompleteInterfaceThrowsOnDuplicateInterface ()
    {
      var cc1 = new ClassContext (typeof (BaseType1), new MixinContext[0], new[] { typeof (IServiceProvider) });
      var cc2 = new ClassContext (typeof (BaseType2), new MixinContext[0], new[] { typeof (IServiceProvider) });

      new MixinConfiguration (new ClassContextCollection (cc1, cc2));
    }
  }
}
