// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Globalization.AclTools.Expansion;
using Remotion.SecurityManager.UnitTests.Domain;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.AclTools
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private DatabaseFixtures _dbFixtures;

    public static ObjectID OrderClassID { get; private set; }
    public static  List<AccessControlList> aclList { get; private set; }
    public ObjectID InvoiceClassID { get; set; }

    [SetUp]
    public void SetUp ()
    {
      try
      {
        // Use default localization for tests
        AclToolsExpansion.Culture = new CultureInfo ("");

        AccessControlTestHelper testHelper = new AccessControlTestHelper();
        using (testHelper.Transaction.EnterDiscardingScope())
        {
          _dbFixtures = new DatabaseFixtures();
          _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.Current);

          SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
          OrderClassID = orderClass.ID;

          testHelper.AttachAccessType (orderClass, Guid.NewGuid (), "FirstAccessType", 0);
          testHelper.AttachAccessType (orderClass, Guid.NewGuid(), "FirstAccessType2", 2);
          testHelper.AttachAccessType (orderClass, Guid.NewGuid(), "FirstAccessType3", 3);
          aclList = testHelper.CreateAclsForOrderAndPaymentAndDeliveryStates (orderClass);
          var ace = aclList[0].CreateAccessControlEntry ();
          ace.Permissions[0].Allowed = true; // FirstAccessType

          var invoiceClass = testHelper.CreateInvoiceClassDefinition();
          InvoiceClassID = invoiceClass.ID;

          LocalizeClassEnDe (orderClass, "Order", "Bestellung");
          
          LocalizeStatePropertyEnDe (orderClass, "Payment", "Payment", "Bezahlstatus");
          LocalizeStateEnDe (orderClass, "Payment", (int) PaymentState.None, "None", "Offen");
          LocalizeStateEnDe (orderClass, "Payment", (int) PaymentState.Paid, "Paid", "Bezahlt");
          
          LocalizeStatePropertyEnDe (orderClass, "State", "Order State", "Bestellstatus");
          LocalizeStateEnDe (orderClass, "State", (int) OrderState.Delivered, "Delivered", "Ausgelifert");
          LocalizeStateEnDe (orderClass, "State", (int) OrderState.Received, "Received", "Erhalten");

          LocalizeStatePropertyEnDe (orderClass, "Delivery", "Delivery Provider", "Auslieferer");
          LocalizeStateEnDe (orderClass, "Delivery", (int) Delivery.Dhl, "DHL", "DHL");
          LocalizeStateEnDe (orderClass, "Delivery", (int) Delivery.Post, "Mail", "Post");

          ClientTransaction.Current.Commit();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
        throw;
      }
    }


    private void LocalizeMetadataObjectEnDe (MetadataObject metadataObject, string nameEnglish, string nameGerman)
    {
      Culture cultureDe = Culture.NewObject ("de-DE");
      Culture cultureEn = Culture.NewObject ("en-US");
      LocalizedName.NewObject (nameGerman, cultureDe, metadataObject);
      LocalizedName.NewObject (nameEnglish, cultureEn, metadataObject);
    }
  
    private void LocalizeClassEnDe (SecurableClassDefinition classDefinition, string nameEnglish, string nameGerman)
    {
      LocalizeMetadataObjectEnDe (classDefinition, nameEnglish, nameGerman);
    }

    private void LocalizeStatePropertyEnDe (SecurableClassDefinition classDefinition, 
      string statePropertyName, string nameEnglish, string nameGerman)
    {
      var stateProperty = classDefinition.GetStateProperty (statePropertyName);
      LocalizeMetadataObjectEnDe (stateProperty, nameEnglish, nameGerman);
    }

    private void LocalizeStateEnDe (SecurableClassDefinition classDefinition,
      string statePropertyName, int stateEnumValue, string nameEnglish, string nameGerman)
    {
      var stateProperty = classDefinition.GetStateProperty (statePropertyName);
      var state = stateProperty.GetState (stateEnumValue);
      LocalizeMetadataObjectEnDe (state, nameEnglish, nameGerman);
    }
  }
}
