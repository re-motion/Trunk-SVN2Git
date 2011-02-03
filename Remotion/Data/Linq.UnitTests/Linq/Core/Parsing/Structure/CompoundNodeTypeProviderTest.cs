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
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Parsing.Structure;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.Core.Parsing.Structure
{
  [TestFixture]
  public class CompoundNodeTypeProviderTest
  {
    private INodeTypeProvider _nodeTypeProviderMock1;
    private INodeTypeProvider _nodeTypeProviderMock2;
    private CompoundNodeTypeProvider _compoundProvider;
    private MethodInfo _methodInfo;

    [SetUp]
    public void SetUp ()
    {
      _nodeTypeProviderMock1 = MockRepository.GenerateStrictMock<INodeTypeProvider>();
      _nodeTypeProviderMock2 = MockRepository.GenerateStrictMock<INodeTypeProvider> ();
      _compoundProvider = new CompoundNodeTypeProvider (new[] { _nodeTypeProviderMock1, _nodeTypeProviderMock2 });
      _methodInfo = typeof (object).GetMethod ("ToString");
    }

    [Test]
    public void InnerProviders ()
    {
      var result = _compoundProvider.InnerProviders;

      Assert.That (result, Is.EqualTo (new[] { _nodeTypeProviderMock1, _nodeTypeProviderMock2 }));
    }

    [Test]
    public void IsRegistered_FirstReturnsTrue ()
    {
      _nodeTypeProviderMock1
          .Expect (mock => mock.IsRegistered (_methodInfo))
          .Return (true);
      _nodeTypeProviderMock1.Replay();
      _nodeTypeProviderMock2.Replay();

      var result = _compoundProvider.IsRegistered (_methodInfo);

      _nodeTypeProviderMock1.VerifyAllExpectations();
      _nodeTypeProviderMock2.AssertWasNotCalled (mock => mock.IsRegistered (Arg<MethodInfo>.Is.Anything));
      Assert.That (result, Is.True);
    }

    [Test]
    public void IsRegistered_SecondReturnsTrue ()
    {
      _nodeTypeProviderMock1
          .Expect (mock => mock.IsRegistered (_methodInfo))
          .Return (false);
      _nodeTypeProviderMock2
          .Expect (mock => mock.IsRegistered (_methodInfo))
          .Return (true);
      _nodeTypeProviderMock1.Replay ();
      _nodeTypeProviderMock2.Replay ();

      var result = _compoundProvider.IsRegistered (_methodInfo);

      _nodeTypeProviderMock1.VerifyAllExpectations ();
      _nodeTypeProviderMock2.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void IsRegistered_NoneReturnsTrue ()
    {
      _nodeTypeProviderMock1
          .Expect (mock => mock.IsRegistered (_methodInfo))
          .Return (false);
      _nodeTypeProviderMock2
          .Expect (mock => mock.IsRegistered (_methodInfo))
          .Return (false);
      _nodeTypeProviderMock1.Replay ();
      _nodeTypeProviderMock2.Replay ();

      var result = _compoundProvider.IsRegistered (_methodInfo);

      _nodeTypeProviderMock1.VerifyAllExpectations ();
      _nodeTypeProviderMock2.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void GetNodeType_FirstReturnsType ()
    {
      _nodeTypeProviderMock1
          .Expect (mock => mock.GetNodeType (_methodInfo))
          .Return (typeof (string));
      _nodeTypeProviderMock1.Replay ();
      _nodeTypeProviderMock2.Replay ();

      var result = _compoundProvider.GetNodeType (_methodInfo);

      _nodeTypeProviderMock1.VerifyAllExpectations ();
      _nodeTypeProviderMock2.AssertWasNotCalled (mock => mock.GetNodeType (Arg<MethodInfo>.Is.Anything));
      Assert.That (result, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetNodetype_SecondReturnsType ()
    {
      _nodeTypeProviderMock1
          .Expect (mock => mock.GetNodeType (_methodInfo))
          .Return (null);
      _nodeTypeProviderMock2
          .Expect (mock => mock.GetNodeType (_methodInfo))
          .Return (typeof(string));
      _nodeTypeProviderMock1.Replay ();
      _nodeTypeProviderMock2.Replay ();

      var result = _compoundProvider.GetNodeType (_methodInfo);

      _nodeTypeProviderMock1.VerifyAllExpectations ();
      _nodeTypeProviderMock2.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs(typeof(string)));
    }

    [Test]
    public void GetNodeType_NoneReturnsType ()
    {
      _nodeTypeProviderMock1
          .Expect (mock => mock.GetNodeType (_methodInfo))
          .Return (null);
      _nodeTypeProviderMock2
          .Expect (mock => mock.GetNodeType (_methodInfo))
          .Return (null);
      _nodeTypeProviderMock1.Replay ();
      _nodeTypeProviderMock2.Replay ();

      var result = _compoundProvider.GetNodeType (_methodInfo);

      _nodeTypeProviderMock1.VerifyAllExpectations();
      _nodeTypeProviderMock2.VerifyAllExpectations();
      Assert.That (result, Is.Null);
    }
  }
}