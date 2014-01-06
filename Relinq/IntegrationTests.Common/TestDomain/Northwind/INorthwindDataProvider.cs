﻿// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Data.Linq;
using System.Linq;

namespace Remotion.Linq.IntegrationTests.Common.TestDomain.Northwind
{
  /// <summary>
  /// Defines all Properties and Methods needed by the 101 LinqSamples to access the Northwind Database
  /// </summary>
  public interface INorthwindDataProvider
  {
    IQueryable<Product> Products { get;  }
    IQueryable<Customer> Customers { get; }
    IQueryable<Employee> Employees { get; }
    IQueryable<Category> Categories { get; }
    IQueryable<Order> Orders { get;}
    IQueryable<OrderDetail> OrderDetails { get;}
    IQueryable<Contact> Contacts { get;}
    IQueryable<Invoices> Invoices { get; }
    IQueryable<QuarterlyOrder> QuarterlyOrders { get;}
    IQueryable<Shipper> Shippers { get;}
    IQueryable<Supplier> Suppliers { get;}

    NorthwindDataContext Functions { get; }

    decimal? TotalProductUnitPriceByCategory (int categoryID);
    decimal? MinUnitPriceByCategory (int? nullable);
    IQueryable<ProductsUnderThisUnitPriceResult> ProductsUnderThisUnitPrice (decimal @decimal);
    int CustomersCountByRegion (string wa);
    ISingleResult<CustomersByCityResult> CustomersByCity (string london);
    IMultipleResults WholeOrPartialCustomersSet (int p0);
    IMultipleResults GetCustomerAndOrders (string seves);
    void CustomerTotalSales (string customerID, ref decimal? totalSales);
  }
}