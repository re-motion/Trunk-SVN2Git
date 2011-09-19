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
using Remotion.Utilities;
using Remotion.Web;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  public class StubResourceUrl : IResourceUrl
  {
    public static void StubFactoryForAnyResourceUrl (IResourceUrlFactory resourceUrlFactoryStub)
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactoryStub", resourceUrlFactoryStub);

      resourceUrlFactoryStub
          .Stub (stub => stub.CreateResourceUrl (null, ResourceType.UI, null))
          .IgnoreArguments()
          .Return (null)
          .WhenCalled (invocation => invocation.ReturnValue = GetStubbedUrlFromMethodInvocation ("RU", invocation));
    }

    public static void StubFactoryForAnyThemedResourceUrl (IResourceUrlFactory resourceUrlFactoryStub)
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactoryStub", resourceUrlFactoryStub);

      resourceUrlFactoryStub
          .Stub (stub => stub.CreateThemedResourceUrl (null, ResourceType.UI, null))
          .IgnoreArguments()
          .Return (null)
          .WhenCalled (invocation => invocation.ReturnValue = GetStubbedUrlFromMethodInvocation ("TRU", invocation));
    }

    private static StubResourceUrl GetStubbedUrlFromMethodInvocation (string prefix, MethodInvocation invocation)
    {
      // ReSharper disable RedundantCast
      string url = string.Format (
          "{0}|{1}|{2}|{3}",
          prefix,
          (Type) invocation.Arguments[0],
          (ResourceType) invocation.Arguments[1],
          (string) invocation.Arguments[2]);
      // ReSharper restore RedundantCast
      return new StubResourceUrl (url);
    }

    private readonly string _url;

    public StubResourceUrl (string url)
    {
      _url = url;
    }

    public string GetUrl ()
    {
      return _url;
    }
  }
}