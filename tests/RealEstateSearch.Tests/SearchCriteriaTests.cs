using Microsoft.VisualStudio.TestTools.UnitTesting;
using RealEstateSearch.SearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateSearch.Tests;

[TestClass]
public class SearchCriteriaTests
{
    [TestMethod]
    public void SearchCriteria_DefaultValues_AreNull()
    {
        var criteria = new SearchCriteria();

        Assert.IsNull(criteria.MinPrice);
        Assert.IsNull(criteria.MaxPrice);
        Assert.IsNull(criteria.MinSquareFootage);
        Assert.IsNull(criteria.MaxSquareFootage);
        Assert.IsNull(criteria.MinBedrooms);
        Assert.IsNull(criteria.MaxBedrooms);
        Assert.IsNull(criteria.Location);
        Assert.IsNull(criteria.PropertyType);
        Assert.IsNull(criteria.MaxAge);
        Assert.IsNull(criteria.MaxDistance);
        Assert.IsNull(criteria.Latitude);
        Assert.IsNull(criteria.Longitude);
        Assert.IsNull(criteria.RequiredAmenities);
        Assert.IsNull(criteria.Conditions);
        Assert.IsNull(criteria.DaysOnMarket);
    }
}
