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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public class AspNetDockerContainerWrapperConfigurationParameters
  {
    [NotNull]
    public string AbsoluteWebApplicationPath { get; }

    public int WebApplicationPort { get; }

    [NotNull]
    public string DockerImageName { get; }

    [CanBeNull]
    public string Hostname { get; }

    public bool Is32BitProcess { get; }

    public AspNetDockerContainerWrapperConfigurationParameters (
        [NotNull] string absoluteWebApplicationPath,
        int webApplicationPort,
        [NotNull] string dockerImageName,
        [CanBeNull] string hostname,
        bool is32BitProcess)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("absoluteWebApplicationPath", absoluteWebApplicationPath);
      ArgumentUtility.CheckNotNullOrEmpty ("dockerImageName", dockerImageName);
      ArgumentUtility.CheckNotEmpty ("hostname", hostname);

      AbsoluteWebApplicationPath = absoluteWebApplicationPath;
      WebApplicationPort = webApplicationPort;
      DockerImageName = dockerImageName;
      Hostname = hostname;
      Is32BitProcess = is32BitProcess;
    }
  }
}