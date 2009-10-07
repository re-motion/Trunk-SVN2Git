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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTool;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy;
using Rhino.Mocks;
using ErrorEventArgs=Remotion.Mixins.MixerTool.ErrorEventArgs;

namespace Remotion.UnitTests.Mixins.MixerTool
{
  [Serializable]
  [TestFixture]
  public class MixerTest
  {
    private string _assemblyOutputDirectoy;
    private string _signedModulePath;
    private string _unsignedModulePath;

    private IClassContextFinder _classContextFinderStub;
    private IConcreteTypeBuilderFactory _concreteTypeBuilderFactoryStub;
    private IConcreteTypeBuilder _concreteTypeBuilderStub;
    private Mixer _mixer;
    private MixinConfiguration _configuration;
    private ClassContext _context;

    [SetUp]
    public void SetUp ()
    {
      _assemblyOutputDirectoy = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "MixerTest");
      _signedModulePath = Path.Combine (_assemblyOutputDirectoy, "Signed.dll");
      _unsignedModulePath = Path.Combine (_assemblyOutputDirectoy, "Unsigned.dll");
      
      if (Directory.Exists (_assemblyOutputDirectoy))
        Directory.Delete (_assemblyOutputDirectoy, true);

      _configuration = new MixinConfiguration ();
      _context = new ClassContext (typeof (object));

      _classContextFinderStub = MockRepository.GenerateStub<IClassContextFinder> ();
      _concreteTypeBuilderFactoryStub = MockRepository.GenerateStub<IConcreteTypeBuilderFactory> ();
      _concreteTypeBuilderStub = MockRepository.GenerateStub<IConcreteTypeBuilder> ();

      _classContextFinderStub.Stub (stub => stub.FindClassContexts (_configuration)).Return (new[] { _context });

      _concreteTypeBuilderFactoryStub.Stub (stub => stub.GetSignedModulePath (_assemblyOutputDirectoy)).Return (_signedModulePath);
      _concreteTypeBuilderFactoryStub.Stub (stub => stub.GetUnsignedModulePath (_assemblyOutputDirectoy)).Return (_unsignedModulePath);
      _concreteTypeBuilderFactoryStub.Stub (stub => stub.CreateTypeBuilder (_assemblyOutputDirectoy)).Return (_concreteTypeBuilderStub);

      _concreteTypeBuilderStub.Stub (stub => stub.SaveAndResetDynamicScope ()).Return (new string[0]);

      _mixer = new Mixer (_classContextFinderStub, _concreteTypeBuilderFactoryStub, _assemblyOutputDirectoy);
    }

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists (_assemblyOutputDirectoy))
        Directory.Delete (_assemblyOutputDirectoy, true);
    }

    [Test]
    public void PrepareOutputDirectory_DirectoryDoesNotExist ()
    {
      Assert.That (Directory.Exists (_assemblyOutputDirectoy), Is.False);

      _mixer.PrepareOutputDirectory ();

      Assert.That (Directory.Exists (_assemblyOutputDirectoy), Is.True);
    }

    [Test]
    public void PrepareOutputDirectory_DirectoryDoesExist ()
    {
      Directory.CreateDirectory (_assemblyOutputDirectoy);
      Assert.That (Directory.Exists (_assemblyOutputDirectoy), Is.True);

      _mixer.PrepareOutputDirectory ();

      Assert.That (Directory.Exists (_assemblyOutputDirectoy), Is.True);
    }

    [Test]
    public void PrepareOutputDirectory_SignedModuleIsDeleted ()
    {
      Directory.CreateDirectory (_assemblyOutputDirectoy);
      CreateEmptyFile(_signedModulePath);

      Assert.That (File.Exists (_signedModulePath), Is.True);

      _mixer.PrepareOutputDirectory ();

      Assert.That (File.Exists (_signedModulePath), Is.False);
    }

    [Test]
    public void PrepareOutputDirectory_UnsignedModuleIsDeleted ()
    {
      Directory.CreateDirectory (_assemblyOutputDirectoy);
      CreateEmptyFile (_unsignedModulePath);

      Assert.That (File.Exists (_unsignedModulePath), Is.True);

      _mixer.PrepareOutputDirectory ();

      Assert.That (File.Exists (_unsignedModulePath), Is.False);
    }

    [Test]
    public void Execute_FindsClassContexts ()
    {
      var classContextFinderMock = new MockRepository().StrictMock<IClassContextFinder> ();
      classContextFinderMock.Expect (mock => mock.FindClassContexts (_configuration)).Return (new ClassContext[0]);
      classContextFinderMock.Replay ();

      var mixer = new Mixer (classContextFinderMock, _concreteTypeBuilderFactoryStub, _assemblyOutputDirectoy);
      mixer.Execute (_configuration);

      classContextFinderMock.VerifyAllExpectations ();
    }

    [Test]
    public void Execute_RaisesClassContextBeingProcessed ()
    {
      object eventSender = null;
      ClassContextEventArgs eventArgs = null;

      _mixer.ClassContextBeingProcessed += (sender, args) => { eventSender = sender; eventArgs = args; };
      _mixer.Execute (_configuration);

      Assert.That (eventSender, Is.SameAs (_mixer));
      Assert.That (eventArgs.ClassContext, Is.SameAs (_context));
    }

    [Test]
    public void Execute_GetsConcreteType ()
    {
      var concreteTypeBuilderMock = new MockRepository ().StrictMock<IConcreteTypeBuilder> ();
      concreteTypeBuilderMock.Expect (mock => mock.GetConcreteType (_context)).Return (typeof (FakeConcreteMixedType));
      concreteTypeBuilderMock.Expect (mock => mock.SaveAndResetDynamicScope()).Return (new string[0]);
      concreteTypeBuilderMock.Replay ();

      RedefineFactoryStub (concreteTypeBuilderMock);

      _mixer.Execute (_configuration);

      concreteTypeBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void Execute_ValidationError ()
    {
      var validationException = new ValidationException ("x", MockRepository.GenerateMock<IValidationLog>());

      var concreteTypeBuilderMock = new MockRepository ().StrictMock<IConcreteTypeBuilder> ();
      concreteTypeBuilderMock.Expect (mock => mock.GetConcreteType (_context)).Throw (validationException);
      concreteTypeBuilderMock.Expect (mock => mock.SaveAndResetDynamicScope ()).Return (new string[0]);
      concreteTypeBuilderMock.Replay ();

      RedefineFactoryStub (concreteTypeBuilderMock);

      object eventSender = null;
      ValidationErrorEventArgs eventArgs = null;

      _mixer.ValidationErrorOccurred += (sender, args) => { eventSender = sender; eventArgs = args; };
      _mixer.Execute (_configuration);

      concreteTypeBuilderMock.VerifyAllExpectations ();

      Assert.That (eventSender, Is.SameAs (_mixer));
      Assert.That (eventArgs.ValidationException, Is.SameAs (validationException));
    }

    [Test]
    public void Execute_OtherError ()
    {
      var exception = new Exception ("x");

      var concreteTypeBuilderMock = new MockRepository ().StrictMock<IConcreteTypeBuilder> ();
      concreteTypeBuilderMock.Expect (mock => mock.GetConcreteType (_context)).Throw (exception);
      concreteTypeBuilderMock.Expect (mock => mock.SaveAndResetDynamicScope ()).Return (new string[0]);
      concreteTypeBuilderMock.Replay ();

      RedefineFactoryStub (concreteTypeBuilderMock);

      object eventSender = null;
      ErrorEventArgs eventArgs = null;

      _mixer.ErrorOccurred += (sender, args) => { eventSender = sender; eventArgs = args; };
      _mixer.Execute (_configuration);

      concreteTypeBuilderMock.VerifyAllExpectations ();

      Assert.That (eventSender, Is.SameAs (_mixer));
      Assert.That (eventArgs.Exception, Is.SameAs (exception));
    }

    [Test]
    public void Execute_Saves ()
    {
      var concreteTypeBuilderMock = new MockRepository ().StrictMock<IConcreteTypeBuilder> ();
      concreteTypeBuilderMock.Expect (mock => mock.GetConcreteType (_context)).Return (typeof (FakeConcreteMixedType));
      concreteTypeBuilderMock.Expect (mock => mock.SaveAndResetDynamicScope ()).Return (new[] { "a", "b" });
      concreteTypeBuilderMock.Replay ();

      RedefineFactoryStub (concreteTypeBuilderMock);

      _mixer.Execute (_configuration);

      concreteTypeBuilderMock.VerifyAllExpectations ();
    }

    private void RedefineFactoryStub (IConcreteTypeBuilder concreteTypeBuilderMock)
    {
      _concreteTypeBuilderFactoryStub.BackToRecord ();
      _concreteTypeBuilderFactoryStub.Stub (stub => stub.CreateTypeBuilder (_assemblyOutputDirectoy)).Return (concreteTypeBuilderMock);
      _concreteTypeBuilderFactoryStub.Replay ();
    }

    private void CreateEmptyFile (string path)
    {
      using (File.Create (path))
      {
      }
    }
  }
}
