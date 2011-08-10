using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class QueryCommandFactoryTest : StandardMappingTest
  {
    private IStorageTypeInformationProvider _storageTypeInformationProviderStrictMock;
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private IObjectReaderFactory _objectReaderFactoryStrictMock;
    private IObjectReader<DataContainer> _dataContainerReader1Stub;
    private QueryCommandFactory _factory;

    private QueryParameter _queryParameter1;
    private QueryParameter _queryParameter2;
    private QueryParameter _queryParameter3;
    private IQuery _queryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _storageTypeInformationProviderStrictMock = MockRepository.GenerateStrictMock<IStorageTypeInformationProvider>();
      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();

      _objectReaderFactoryStrictMock = MockRepository.GenerateStrictMock<IObjectReaderFactory>();

      _factory = new QueryCommandFactory (
          TestDomainStorageProviderDefinition,
          _storageTypeInformationProviderStrictMock,
          _objectReaderFactoryStrictMock,
          _dbCommandBuilderFactoryStrictMock);

      _dataContainerReader1Stub = MockRepository.GenerateStub<IObjectReader<DataContainer>>();

      _queryParameter1 = new QueryParameter ("first", DomainObjectIDs.Order1);
      _queryParameter2 = new QueryParameter ("second", DomainObjectIDs.Order2.Value);
      _queryParameter3 = new QueryParameter ("third", DomainObjectIDs.Official1);
      var collection = new QueryParameterCollection { _queryParameter1, _queryParameter2, _queryParameter3 };

      _queryStub = MockRepository.GenerateStub<IQuery>();
      _queryStub.Stub (stub => stub.Statement).Return ("statement");
      _queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection (collection, true));
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      var type1 = MockRepository.GenerateStub<IStorageTypeInformation>();
      var type2 = MockRepository.GenerateStub<IStorageTypeInformation>();
      var type3 = MockRepository.GenerateStub<IStorageTypeInformation>();
      _storageTypeInformationProviderStrictMock.Expect (mock => mock.GetStorageType (DomainObjectIDs.Order1.Value)).Return (type1);
      _storageTypeInformationProviderStrictMock.Expect (mock => mock.GetStorageType (DomainObjectIDs.Order2.Value)).Return (type2);
      _storageTypeInformationProviderStrictMock.Expect (mock => mock.GetStorageType (DomainObjectIDs.Official1.ToString())).Return (type3);
      _storageTypeInformationProviderStrictMock.Replay();

      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();
      var expectedParametersWithType =
          new[]
          {
              new QueryParameterWithType (
                  new QueryParameter (_queryParameter1.Name, DomainObjectIDs.Order1.Value, _queryParameter1.ParameterType),
                  type1),
              new QueryParameterWithType (_queryParameter2, type2),
              new QueryParameterWithType (
                  new QueryParameter (_queryParameter3.Name, DomainObjectIDs.Official1.ToString(), _queryParameter3.ParameterType),
                  type3)
          };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForQuery (Arg.Is ("statement"), Arg<IEnumerable<QueryParameterWithType>>.List.Equal (expectedParametersWithType)))
          .Return (commandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock.Expect (mock => mock.CreateDataContainerReader()).Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForDataContainerQuery (_queryStub);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();
      _objectReaderFactoryStrictMock.VerifyAllExpectations();

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var command = ((MultiObjectLoadCommand<DataContainer>) result);
      Assert.That (command.DbCommandBuildersAndReaders.Length, Is.EqualTo (1));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs (commandBuilderStub));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item2, Is.SameAs (_dataContainerReader1Stub));
    }
  }
}