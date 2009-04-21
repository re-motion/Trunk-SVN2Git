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
      MixinConfiguration ac = new MixinConfiguration ();
      ClassContext cc = new ClassContextBuilder (typeof (BaseType2))
          .AddCompleteInterface (typeof (IBaseType2))
          .BuildClassContext ();
      ac.ClassContexts.Add (cc);
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));
    }

    [Test]
    public void RegisterAndResolveCompleteInterfaceExplicitly ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ClassContext cc = new ClassContextBuilder (typeof (BaseType2))
          .BuildClassContext();

      ac.ClassContexts.Add (cc);
      Assert.IsNull (ac.ResolveInterface (typeof (IBaseType2)));
      ac.RegisterInterface (typeof (IBaseType2), cc);
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));

      ac.ClassContexts.Add (new ClassContext (typeof (BaseType3)));
      ac.ClassContexts.GetWithInheritance (typeof (BaseType3));
      ac.RegisterInterface (typeof (IBaseType31), typeof (BaseType3));
      Assert.AreSame (ac.ClassContexts.GetWithInheritance (typeof (BaseType3)), ac.ResolveInterface (typeof (IBaseType31)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The argument is not an interface.", MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnNonInterface ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.ClassContexts.Add (cc);
      ac.RegisterInterface (typeof (BaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The class context hasn't been added to this configuration.",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext1 ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.RegisterInterface (typeof (IBaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The class context hasn't been added to this configuration.",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext2 ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.ClassContexts.Add (cc);
      ac.RegisterInterface (typeof (IBaseType2), new ClassContext (typeof (BaseType2)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no class context for the given type",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext3 ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ac.RegisterInterface (typeof (IBaseType2), typeof (BaseType2));
    }

    [Test]
    public void RemoveClassContextCausesInterfaceRegistrationToBeRemoved ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.ClassContexts.Add (cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);

      Assert.IsNotNull (ac.ResolveInterface (typeof (IBaseType2)));
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));

      ac.ClassContexts.RemoveExact (typeof (BaseType2));

      Assert.IsNull (ac.ResolveInterface (typeof (IBaseType2)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "already been associated with a class context",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnDuplicateInterface ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.ClassContexts.Add (cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The argument is not an interface.", MatchType = MessageMatch.Contains)]
    public void ResolveCompleteInterfaceThrowsOnNonInterface ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ac.ResolveInterface (typeof (BaseType2));
    }
  }
}
