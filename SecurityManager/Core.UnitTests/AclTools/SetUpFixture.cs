/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
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

    static public List<AccessControlList> aclList { get; private set; }

    public ObjectID InvoiceClassID { get; set; }


    [SetUp]
    public void SetUp ()
    {
      try
      {
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

          //Culture cultureDe = Culture.NewObject ("de");


          var invoiceClass = testHelper.CreateInvoiceClassDefinition();
          InvoiceClassID = invoiceClass.ID;
          //LocalizedName.NewObject ("Rechnung", cultureDe, invoiceClass);

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


    private void LocalizeMetadateObjectEnDe (MetadataObject metadataObject, string nameEnglish, string nameGerman)
    {
      Culture cultureDe = Culture.NewObject ("de-DE");
      Culture cultureEn = Culture.NewObject ("en-US");
      LocalizedName.NewObject (nameGerman, cultureDe, metadataObject);
      LocalizedName.NewObject (nameEnglish, cultureEn, metadataObject);
    }
  
    private void LocalizeClassEnDe (SecurableClassDefinition classDefinition, string nameEnglish, string nameGerman)
    {
      LocalizeMetadateObjectEnDe (classDefinition, nameEnglish, nameGerman);
    }

    private StatePropertyDefinition LocalizeStatePropertyEnDe (SecurableClassDefinition classDefinition, 
      string statePropertyName, string nameEnglish, string nameGerman)
    {
      var stateProperty = classDefinition.GetStateProperty (statePropertyName);
      LocalizeMetadateObjectEnDe (stateProperty, nameEnglish, nameGerman);
      return stateProperty;
    }

    private StateDefinition LocalizeStateEnDe (SecurableClassDefinition classDefinition,
      string statePropertyName, int stateEnumValue, string nameEnglish, string nameGerman)
    {
      var stateProperty = classDefinition.GetStateProperty (statePropertyName);
      var state = stateProperty.GetState (stateEnumValue);
      LocalizeMetadateObjectEnDe (state, nameEnglish, nameGerman);
      return state;
    }
  }
}