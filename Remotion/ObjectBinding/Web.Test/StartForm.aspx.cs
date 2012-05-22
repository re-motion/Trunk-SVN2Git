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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using Microsoft.CSharp;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.MixerTools;
using Remotion.Utilities;

namespace OBWTest
{
  public class StartFormMixin
  {
  }

  [Uses (typeof (StartFormMixin))]
  [FileLevelControlBuilderAttribute (typeof (PageBuilder))]
  public class StartForm : Page
  {
  }

  public class PageBuilder : FileLevelPageControlBuilder // different base type for user control and master page
  {
    public override void ProcessGeneratedCode (
        CodeCompileUnit codeCompileUnit,
        CodeTypeDeclaration baseType,
        CodeTypeDeclaration derivedType,
        CodeMemberMethod buildMethod,
        CodeMemberMethod dataBindingMethod)
    {
      var results2 = MixTypes (AppDomain.CurrentDomain.DynamicDirectory, typeof (StartForm));
      var concreteType = results2[typeof (StartForm)];
      codeCompileUnit.ReferencedAssemblies.Add (
          concreteType.Assembly.GetModules().Single (m => m.ScopeName.StartsWith ("Mixins.AspNet.")).FullyQualifiedName);
      baseType.BaseTypes[0] = new CodeTypeReference (concreteType, baseType.BaseTypes[0].Options);


      base.ProcessGeneratedCode (codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
    }


    private Dictionary<Type, Type> MixTypes (string outputDirectory, params Type[] types)
    {
      var classContextFinder = new FixedClassContextFinder (types);
      var concreteTypeBuilderFactory = new ConcreteTypeBuilderFactory (
          new GuidNameProvider(),
          "Mixins.AspNet.Signed.{counter}",
          "Mixins.AspNet.Unsigned.{counter}");
      var mixer = new Mixer (classContextFinder, concreteTypeBuilderFactory, outputDirectory);
      mixer.ErrorOccurred += (sender, args) => { throw args.Exception; };
      mixer.ValidationErrorOccurred += (sender, args) => { throw args.ValidationException; };
      mixer.Execute (MixinConfiguration.ActiveConfiguration);
      return mixer.FinishedTypes;
    }
  }

  internal class FixedClassContextFinder : IClassContextFinder
  {
    private readonly Type[] _types;

    public FixedClassContextFinder (params Type[] types)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      _types = types;
    }

    public IEnumerable<ClassContext> FindClassContexts (MixinConfiguration configuration)
    {
      return _types.Select (configuration.GetContext).Where (c => c != null);
    }
  }

  public class RuntimeReferencesCSharpCodeDomProvider : CSharpCodeProvider
  {
    private readonly Set<string> _references = new Set<string>();

    public override void GenerateCodeFromCompileUnit (CodeCompileUnit compileUnit, TextWriter writer, CodeGeneratorOptions options)
    {
      _references.AddRange (compileUnit.ReferencedAssemblies.Cast<string>());
      base.GenerateCodeFromCompileUnit (compileUnit, writer, options);
    }

    public override CompilerResults CompileAssemblyFromFile (CompilerParameters options, params string[] fileNames)
    {
      options.ReferencedAssemblies.AddRange (_references.Except (options.ReferencedAssemblies.Cast<string>()).ToArray());
      _references.Clear();
      return base.CompileAssemblyFromFile (options, fileNames);
    }
  }
}