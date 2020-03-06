﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  /// <summary>
  /// Provides an interface for communicating with a Docker client.
  /// </summary>
  public interface IDockerClient
  {
    /// <summary>
    /// Pulls the given docker image.
    /// </summary>
    void Pull ([NotNull] string imageName);

    /// <summary>
    /// Runs the docker image with the given settings.
    /// </summary>
    /// <returns>The container ID of the started container.</returns>
    [NotNull]
    string Run (
        [NotNull] IDictionary<int, int> ports,
        [NotNull] IDictionary<string, string> mounts,
        [NotNull] string imageName,
        [CanBeNull] string hostname,
        bool detach,
        bool remove,
        [CanBeNull] string entryPoint,
        [CanBeNull] string args);

    /// <summary>
    /// Checks if a container with the specified ID exists.
    /// </summary>
    /// <returns>True if the container exists, false otherwise.</returns>
    bool ContainerExists ([NotNull] string containerName);

    /// <summary>
    /// Removes a container with the given ID.
    /// </summary>
    void Remove ([NotNull] string containerName, bool force = false);

    /// <summary>
    /// Stops a container with the given ID.
    /// </summary>
    void Stop ([NotNull] string containerName);
  }
}