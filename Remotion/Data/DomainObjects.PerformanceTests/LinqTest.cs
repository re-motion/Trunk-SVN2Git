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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class LinqTest : DatabaseTest
  {
    private ISqlPreparationStage _sqlPreparationStage;
    private IMappingResolutionStage _mappingresolutionStage;
    private ISqlGenerationStage _sqlGenerationStage;
    private IMappingResolutionContext _mappingresolutionContext;
      
    [SetUp]
    public new void SetUp ()
    {
      base.SetUp();

      var methodCallTransformerRegistry = MethodCallTransformerRegistry.CreateDefault();
      var resultOperatorHandlerRegistry = ResultOperatorHandlerRegistry.CreateDefault();
      var generator = new UniqueIdentifierGenerator();
      _sqlPreparationStage = new DefaultSqlPreparationStage(methodCallTransformerRegistry, resultOperatorHandlerRegistry, generator);
      _mappingresolutionStage = new DefaultMappingResolutionStage (new MappingResolver(), generator);
      _sqlGenerationStage = new DefaultSqlGenerationStage();
      _mappingresolutionContext = new MappingResolutionContext();
   }

    [Test]
    [Ignore("TODO: RM-3009")]
    public void GetAllFiles()
    {
      Func<IQueryable<File>> queryGenerator = () => (from c in QueryFactory.CreateLinqQuery<File>() select c);
      var linqHelper = new LinqPerformanceTestHelper<File> (queryGenerator, _sqlPreparationStage, _mappingresolutionStage, _sqlGenerationStage, _mappingresolutionContext);

      PerformanceTestHelper.TimeAndOutput (1000, "Simple query with many results (QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (1000, "Simple query with many results (QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (1000, "Simple query with many results (QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        PerformanceTestHelper.TimeAndOutput (1000, "Simple query with many results (QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQuery);
      }
    }


  }
}