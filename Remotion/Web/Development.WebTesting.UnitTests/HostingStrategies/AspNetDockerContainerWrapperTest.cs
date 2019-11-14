using System;
using System.Collections.Generic;
using System.IO;
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
      var dockerHelperMock = MockRepository.GenerateMock<IDockerHelper> ();
      var dockerFileMock = MockRepository.GenerateMock<IDockerFile> ();
      var dockerFileManagerMock = MockRepository.GenerateMock<IDockerFileManager> ();
      dockerFileManagerMock.Expect (_ => _.Prepare (Arg<string>.Is.Equal (TemporaryDirectory))).Return (dockerFileMock);

      var is32BitProcess = false;
      var configurationParameter = new AspNetDockerContainerWrapperConfigurationParameters (TemporaryDirectory, 123, "TestDockerImageName", null, is32BitProcess);

      dockerHelperMock.Expect (_ => _.Pull (Arg<string>.Is.Equal (configurationParameter.DockerImageName)));

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

      dockerFileMock.Expect (_ => _.Dispose ());

      var aspNetDockerContainerWrapper = new AspNetDockerContainerWrapper (dockerHelperMock, dockerFileManagerMock, configurationParameter);

      Assert.That (() => aspNetDockerContainerWrapper.BuildAndRun(), Throws.Nothing);
      dockerHelperMock.VerifyAllExpectations();
      dockerFileManagerMock.VerifyAllExpectations();
      dockerFileMock.VerifyAllExpectations();
    }

    [Test]
    public void BuildAndRun_With32BitTrue_ShouldSet32BitFlag ()
    {
      var dockerHelperMock = MockRepository.GenerateMock<IDockerHelper> ();
      var dockerFileMock = MockRepository.GenerateMock<IDockerFile> ();
      var dockerFileManagerMock = MockRepository.GenerateMock<IDockerFileManager> ();
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
      var dockerFileManagerMock = MockRepository.GenerateStub<IDockerFileManager> ();
      var dockerFileMock = MockRepository.GenerateStub<IDockerFile> ();
      dockerFileManagerMock.Expect (_ => _.Prepare (Arg<string>.Is.Anything)).Return (dockerFileMock);
      
      var configurationParameter = new AspNetDockerContainerWrapperConfigurationParameters (TemporaryDirectory, 123, "TestDockerImageName", null, false);
      var aspNetDockerContainerWrapper = new AspNetDockerContainerWrapper (dockerHelperSpy, dockerFileManagerMock, configurationParameter);

      aspNetDockerContainerWrapper.BuildAndRun();
      
      Assert.That (dockerHelperSpy.BuildTagValue, Is.EqualTo (dockerHelperSpy.RunImageNameValue));

      aspNetDockerContainerWrapper.Dispose();

      Assert.That (dockerHelperSpy.StopContainerNameValue, Is.EqualTo (dockerHelperSpy.RunContainerNameValue));
      Assert.That (dockerHelperSpy.RemoveImageImageNameValue, Is.EqualTo (dockerHelperSpy.RunImageNameValue));
    }

    public class DockerHelperSpy : IDockerHelper
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
