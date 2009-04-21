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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class CollectionSetterTest : ClientTransactionBaseTest
  {
    private IndustrialSector _newIndustrialSector;
    private Company _newCompany1;
    private Company _newCompany2;

    public override void SetUp ()
    {
      base.SetUp();

      _newIndustrialSector = IndustrialSector.NewObject ();
      _newCompany1 = Company.NewObject ();
      _newCompany2 = Company.NewObject ();
    }

    [Test]
    public void ReplaceCollectionProperty ()
    {
      var oldCompanies = _newIndustrialSector.Companies;
      var newCompanies = new ObjectList<Company> ();
      _newIndustrialSector.Companies = newCompanies;

      Assert.That (_newIndustrialSector.Companies, Is.SameAs (newCompanies));
      _newIndustrialSector.Companies.Add (_newCompany1);
      _newCompany2.IndustrialSector = _newIndustrialSector;

      Assert.That (oldCompanies, Is.Empty);
      Assert.That (newCompanies, Is.EqualTo (new[] { _newCompany1, _newCompany2 }));
    }

    [Test]
    public void ReplaceCollectionProperty_HasChanged ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope())
      {
        Assert.That (_newIndustrialSector.State, Is.EqualTo (StateType.Unchanged));
        var newCompanies = new ObjectList<Company> ();
        _newIndustrialSector.Companies = newCompanies;
        Assert.That (_newIndustrialSector.State, Is.EqualTo (StateType.Changed));
        _newIndustrialSector.Companies.Add (_newCompany1);
        Assert.That (_newIndustrialSector.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void ReplaceCollectionProperty_Commit ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var oldCompanies = _newIndustrialSector.Companies;
        var newCompanies = new ObjectList<Company> ();
        _newIndustrialSector.Companies = newCompanies;
        _newIndustrialSector.Companies.Add (_newCompany1);
        _newIndustrialSector.Companies.Add (_newCompany2);
        Assert.That (_newIndustrialSector.State, Is.EqualTo (StateType.Changed));
        ClientTransaction.Current.Commit ();

        Assert.That (_newIndustrialSector.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (newCompanies, Is.EquivalentTo (new[] { _newCompany1, _newCompany2 }));
        Assert.That (oldCompanies, Is.Empty);
      }
      Assert.That (_newIndustrialSector.Companies, Is.EquivalentTo (new[] { _newCompany1, _newCompany2 }));
    }

    [Test]
    public void ReplaceCollectionProperty_Rollback ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (_newIndustrialSector.State, Is.EqualTo (StateType.Unchanged));
        var oldCompanies = _newIndustrialSector.Companies;
        var newCompanies = new ObjectList<Company> ();
        _newIndustrialSector.Companies = newCompanies;
        _newIndustrialSector.Companies.Add (_newCompany1);
        _newIndustrialSector.Companies.Add (_newCompany2);
        ClientTransaction.Current.Rollback();
        
        Assert.That (_newIndustrialSector.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_newIndustrialSector.Companies, Is.Empty);
        Assert.That (_newIndustrialSector.Companies, Is.SameAs (oldCompanies));
        Assert.That (newCompanies, Is.EquivalentTo (new[] { _newCompany1, _newCompany2 }), "list is detached during commit, but keeps its values");
      }
    }

    [Test]
    public void ReplaceCollectionProperty_WithOldElements ()
    {
      var oldCompanies = _newIndustrialSector.Companies;

      _newIndustrialSector.Companies.Add (_newCompany1);
      _newIndustrialSector.Companies.Add (_newCompany2);
      _newIndustrialSector.Companies = new ObjectList<Company> ();

      Assert.That (_newCompany1.IndustrialSector, Is.Null);
      Assert.That (_newCompany2.IndustrialSector, Is.Null);
      Assert.That (oldCompanies, Is.EqualTo (new[] { _newCompany1, _newCompany2 }));
    }

    [Test]
    public void ReplaceCollectionProperty_WithNewElements ()
    {
      var oldCompanies = _newIndustrialSector.Companies;

      var newCompanies = new ObjectList<Company> {_newCompany1, _newCompany2};
      _newIndustrialSector.Companies = newCompanies;

      Assert.That (_newCompany1.IndustrialSector, Is.SameAs (_newIndustrialSector));
      Assert.That (_newCompany2.IndustrialSector, Is.SameAs (_newIndustrialSector));
      Assert.That (oldCompanies, Is.Empty);
    }
  }
}
