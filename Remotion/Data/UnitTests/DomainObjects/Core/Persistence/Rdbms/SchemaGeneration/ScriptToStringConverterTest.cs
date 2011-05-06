// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ScriptToStringConverterTest : SchemaGenerationTestBase
  {
    private IScriptBuilder _scriptBuilderStub;
    private ScriptElementCollection _fakeCreateElements;
    private ScriptElementCollection _fakeDropElements;
    private ScriptToStringConverter _converter;
    private ScriptStatement _scriptElement1;
    private ScriptStatement _scriptElement2;

    public override void SetUp ()
    {
      base.SetUp ();

      _converter = new ScriptToStringConverter();
      _fakeCreateElements = new ScriptElementCollection ();
      _fakeDropElements = new ScriptElementCollection ();
      _scriptBuilderStub = MockRepository.GenerateStub<IScriptBuilder>();
      _scriptBuilderStub.Stub (stub => stub.GetCreateScript ()).Return (_fakeCreateElements);
      _scriptBuilderStub.Stub (stub => stub.GetDropScript ()).Return (_fakeDropElements);

      _scriptElement1 = new ScriptStatement ("Test1");
      _scriptElement2 = new ScriptStatement ("Test2");
    }

    [Test]
    public void Convert_OneScriptElement ()
    {
      _fakeCreateElements.AddElement (_scriptElement1);
      _fakeDropElements.AddElement (_scriptElement2);

      var result = _converter.Convert (_scriptBuilderStub);

      Assert.That (result.CreateScript, Is.EqualTo("Test1\r\n"));
      Assert.That (result.DropScript, Is.EqualTo("Test2\r\n"));
    }

    [Test]
    public void Convert_SeveralScriptElements ()
    {
      _fakeCreateElements.AddElement (_scriptElement1);
      _fakeCreateElements.AddElement (_scriptElement2);
      _fakeDropElements.AddElement (_scriptElement2);
      _fakeDropElements.AddElement (_scriptElement1);

      var result = _converter.Convert (_scriptBuilderStub);

      Assert.That (result.CreateScript, Is.EqualTo ("Test1\r\nTest2\r\n"));
      Assert.That (result.DropScript, Is.EqualTo ("Test2\r\nTest1\r\n"));
    }
  }
}