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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.HostingStrategies;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;
using Rhino.Mocks;

namespace Remotion.Web.Development.WebTesting.UnitTests.HostingStrategies
{
  [TestFixture]
  public class AspNetDockerContainerWrapperTest : FileSystemTestBase
  {
    [Test]
    public void BuildAndRun_ShouldNotThrowAndCallCorrectMethods ()
    {
      var dockerHelperMock = MockRepository.GenerateMock<IDockerHelper>();
      var dockerFileMock = MockRepository.GenerateMock<IDockerFile>();
      var dockerFileManagerMock = MockRepository.GenerateMock<IDockerFilePreparer>();
      dockerFileManagerMock.Expect (_ => _.Prepare (Arg<string>.Is.Equal (TemporaryDirectory))).Return (dockerFileMock);

      const bool is32BitProcess = false;
      var configurationParameter = new AspNetDockerContainerWrapperConfigurationParameters (TemporaryDirectory, 123, "TestDockerImageName", null, is32BitProcess);

      dockerHelperMock.Expect (_ => _.Pull (configurationParameter.DockerImageName));

      dockerHelperMock.Expect (
          _ => _.Build (
              Arg<string>.Is.Anything,
              Arg<Dictionary<string, string>>.Matches (
                  dictionary =>
                      dictionary["HostingBaseDockerImage"] == configurationParameter.DockerImageName
                      && dictionary["WebApplicationPort"] == configurationParameter.WebApplicationPort.ToString()
                      && dictionary["Is32BitProcess"] == "false"),
              Arg<IDockerFile>.Is.Equal (dockerFileMock)));

      dockerHelperMock.Expect (
          _ => _.Run (
              Arg<bool>.Is.Equal (true),
              Arg<bool>.Is.Equal (true),
              Arg<Dictionary<int, int>>.Matches (
                  dictionary => dictionary[configurationParameter.WebApplicationPort] == configurationParameter.WebApplicationPort),
              Arg<string>.Is.Anything,
              Arg<string>.Is.Equal (configurationParameter.Hostname),
              Arg<string>.Is.Anything));

      dockerFileMock.Expect (_ => _.Dispose());

      var aspNetDockerContainerWrapper = new AspNetDockerContainerWrapper (dockerHelperMock, dockerFileManagerMock, configurationParameter);

      Assert.That (() => aspNetDockerContainerWrapper.BuildAndRun(), Throws.Nothing);
      dockerHelperMock.VerifyAllExpectations();
      dockerFileManagerMock.VerifyAllExpectations();
      dockerFileMock.VerifyAllExpectations();
    }

    [Test]
    public void BuildAndRun_With32BitTrue_ShouldSet32BitFlag ()
    {
      var dockerHelperMock = MockRepository.GenerateMock<IDockerHelper>();
      var dockerFileMock = MockRepository.GenerateMock<IDockerFile>();
      var dockerFileManagerMock = MockRepository.GenerateMock<IDockerFilePreparer>();
      dockerFileManagerMock.Expect (_ => _.Prepare (Arg<string>.Is.Anything)).Return (dockerFileMock);

      var is32BitProcess = true;
      var configurationParameter = new AspNetDockerContainerWrapperConfigurationParameters (TemporaryDirectory, 123, "TestDockerImageName", null, is32BitProcess);

      dockerHelperMock.Expect (
          _ => _.Build (
              Arg<string>.Is.Anything,
              Arg<Dictionary<string, string>>.Matches (
                  dictionary =>
                      dictionary["HostingBaseDockerImage"] == configurationParameter.DockerImageName
                      && dictionary["WebApplicationPort"] == configurationParameter.WebApplicationPort.ToString()
                      && dictionary["Is32BitProcess"] == "true"),
              Arg<IDockerFile>.Is.Anything));

      var aspNetDockerContainerWrapper = new AspNetDockerContainerWrapper (dockerHelperMock, dockerFileManagerMock, configurationParameter);

      Assert.That (() => aspNetDockerContainerWrapper.BuildAndRun(), Throws.Nothing);

      dockerHelperMock.VerifyAllExpectations();
    }

    [Test]
    public void BuildAndRunDispose_ShouldUseTheSameImageAndContainerName ()
    {
      var dockerHelperSpy = new DockerHelperSpy();
      var dockerFileManagerMock = MockRepository.GenerateStub<IDockerFilePreparer>();
      var dockerFileMock = MockRepository.GenerateStub<IDockerFile>();
      dockerFileManagerMock.Expect (_ => _.Prepare (Arg<string>.Is.Anything)).Return (dockerFileMock);

      var configurationParameter = new AspNetDockerContainerWrapperConfigurationParameters (TemporaryDirectory, 123, "TestDockerImageName", null, false);
      var aspNetDockerContainerWrapper = new AspNetDockerContainerWrapper (dockerHelperSpy, dockerFileManagerMock, configurationParameter);

      aspNetDockerContainerWrapper.BuildAndRun();

      Assert.That (dockerHelperSpy.BuildTagValue, Is.EqualTo (dockerHelperSpy.RunImageNameValue));

      aspNetDockerContainerWrapper.Dispose();

      Assert.That (dockerHelperSpy.StopContainerNameValue, Is.EqualTo (dockerHelperSpy.RunContainerNameValue));
      Assert.That (dockerHelperSpy.RemoveImageImageNameValue, Is.EqualTo (dockerHelperSpy.RunImageNameValue));
    }

    private class DockerHelperSpy : IDockerHelper
    {
      public string BuildTagValue { get; private set; }
      public string RunContainerNameValue { get; private set; }
      public string RunImageNameValue { get; private set; }
      public string StopContainerNameValue { get; private set; }
      public string RemoveImageImageNameValue { get; private set; }

      public void Pull (string dockerImageName)
      {
      }

      public void Build (string tag, IReadOnlyDictionary<string, string> buildArgs, IDockerFile dockerFile)
      {
        BuildTagValue = tag;
      }

      public void Run (bool detached, bool removeContainer, IReadOnlyDictionary<int, int> publishedPorts, string containerName, string hostName, string imageName)
      {
        RunContainerNameValue = containerName;
        RunImageNameValue = imageName;
      }

      public void Stop (string containerName)
      {
        StopContainerNameValue = containerName;
      }

      public void RemoveImage (string imageName)
      {
        RemoveImageImageNameValue = imageName;
      }
    }
  }
}
