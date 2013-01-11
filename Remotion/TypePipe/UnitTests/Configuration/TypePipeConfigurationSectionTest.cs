// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.TypePipe.Configuration;

namespace Remotion.TypePipe.UnitTests.Configuration
{
  [TestFixture]
  public class TypePipeConfigurationSectionTest
  {
    private TypePipeConfigurationSection _section;

    [SetUp]
    public void SetUp ()
    {
      _section = new TypePipeConfigurationSection();
    }

    [Test]
    public void ExampleConfiguration ()
    {
      DeserializeSection (TypePipeConfigurationSection.ExampleConfiguration);

      Assert.That (_section.ForceStrongNaming.ElementInformation.IsPresent, Is.True);
      Assert.That (_section.ForceStrongNaming.KeyFilePath, Is.EqualTo ("keyFile.snk"));
    }

    [Test]
    public void ForceStrongNaming ()
    {
      var xmlFragment = @"<typePipe {xmlns}><forceStrongNaming/></typePipe>";
      DeserializeSection (xmlFragment);

      Assert.That (_section.ForceStrongNaming.ElementInformation.IsPresent, Is.True);
    }

    [Test]
    public void ForceStrongNaming_Empty ()
    {
      var xmlFragment = @"<typePipe {xmlns} />";
      DeserializeSection (xmlFragment);
      
      Assert.That (_section.ForceStrongNaming.ElementInformation.IsPresent, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "Example configuration:", MatchType = MessageMatch.Contains)]
    public void ForceStrongNaming_InvalidSection ()
    {
      var xmlFragment = "<typePipe {xmlns}><invalid /></typePipe>";
      DeserializeSection (xmlFragment);
    }

    private void DeserializeSection (string xmlFragment)
    {
      xmlFragment = xmlFragment.Replace ("{xmlns}", "xmlns=\"" + _section.XmlNamespace + "\"");
      var xsdPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, @"Configuration\TypePipeConfigurationSchema.xsd");
      ConfigurationHelper.DeserializeSection (_section, xmlFragment, xsdPath);
    }
  }
}