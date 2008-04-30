using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  [TestFixture]
  public class SqlProviderTest : SqlProviderBaseTest
  {
    [Test]
    public void LoadConcreteSingle ()
    {
      DataContainer customerContainer = Provider.LoadDataContainer (DomainObjectIDs.Customer);
      Assert.IsNotNull (customerContainer);
      Assert.AreEqual (DomainObjectIDs.Customer, customerContainer.ID);
      Assert.AreEqual ("UnitTests", customerContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain.DomainBase.CreatedBy"));
      Assert.AreEqual ("Zaphod", customerContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain.Person.FirstName"));
      Assert.AreEqual (CustomerType.Premium, customerContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain.Customer.CustomerType"));
    }

    [Test]
    public void LoadDataContainersByRelatedIDWithAbstractBaseClass ()
    {
      ClassDefinition domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      DataContainerCollection loadedDataContainers = Provider.LoadDataContainersByRelatedID (domainBaseClass, "Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain.DomainBase.Client", DomainObjectIDs.Client);

      Assert.IsNotNull (loadedDataContainers);
      Assert.AreEqual (4, loadedDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, loadedDataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, loadedDataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, loadedDataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, loadedDataContainers[3].ID);
    }

    [Test]
    public void GetColumnsFromSortExpression ()
    {
      RdbmsProvider rdbmsProvider = Provider;
      Assert.AreEqual ("Column", rdbmsProvider.GetColumnsFromSortExpression ("Column"));
      Assert.AreEqual ("Column", rdbmsProvider.GetColumnsFromSortExpression (" Column"));
      Assert.AreEqual ("Asc", rdbmsProvider.GetColumnsFromSortExpression ("Asc"));
      Assert.AreEqual ("Asc", rdbmsProvider.GetColumnsFromSortExpression (" Asc"));
      Assert.AreEqual ("asc", rdbmsProvider.GetColumnsFromSortExpression (" asc"));
      Assert.AreEqual ("[Asc]", rdbmsProvider.GetColumnsFromSortExpression ("[Asc]"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1, Column2"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("    Column1   ,    Column2   "));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1,Column2"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 asc,Column2 desc"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 asc, Column2 desc"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 ASC, Column2 DESC"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 \tASC, Column2  \nDESC"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1\tASC,\r\nColumn2\nDESC"));
      Assert.AreEqual ("[ASC], [desc]", rdbmsProvider.GetColumnsFromSortExpression ("[ASC] ASC, [desc] DESC"));
      Assert.AreEqual ("[Collate]", rdbmsProvider.GetColumnsFromSortExpression ("[Collate] asc"));
      Assert.AreEqual ("AscColumnAsc1Asc, DescColumn2Desc", rdbmsProvider.GetColumnsFromSortExpression ("AscColumnAsc1Asc ASC, DescColumn2Desc DESC"));
      Assert.IsNull (rdbmsProvider.GetColumnsFromSortExpression (null));
      Assert.AreEqual (string.Empty, rdbmsProvider.GetColumnsFromSortExpression (string.Empty));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Collations cannot be used in sort expressions. Sort expression: 'Column1 collate German_PhoneBook_CI_AI'.\r\nParameter name: sortExpression")]
    public void GetColumnsWithCollate ()
    {
      Provider.GetColumnsFromSortExpression ("Column1 collate German_PhoneBook_CI_AI");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Collations cannot be used in sort expressions. Sort expression: 'Column1\t\tcollate German_PhoneBook_CI_AI'.\r\nParameter name: sortExpression")]
    public void GetColumnsWithCollateAfterMultipleTabs ()
    {
      Provider.GetColumnsFromSortExpression ("Column1\t\tcollate German_PhoneBook_CI_AI");
    }

  }
}
