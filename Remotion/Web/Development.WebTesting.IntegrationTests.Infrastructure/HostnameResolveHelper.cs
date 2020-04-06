// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  public static class HostnameResolveHelper
  {
    /// <summary>
    /// Resolves the hostname of the given <paramref name="uri"/>. If it contains an IP address, the original URI is returned.
    /// </summary>
    public static Uri ResolveHostname (Uri uri)
    {
      if (uri.HostNameType != UriHostNameType.Dns)
        return uri;

      var host = new RetryUntilTimeout<IPHostEntry> (() => Dns.GetHostEntry (uri.Host), TimeSpan.FromSeconds (30), TimeSpan.FromSeconds (1)).Run();
      var address = host.AddressList.First (a => a.AddressFamily == AddressFamily.InterNetwork).MapToIPv4();
      var uriBuilder = new UriBuilder (uri);
      uriBuilder.Host = address.ToString();
      return uriBuilder.Uri;
    }
  }
}