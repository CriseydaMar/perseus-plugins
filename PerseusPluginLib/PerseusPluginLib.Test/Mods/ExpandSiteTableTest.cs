﻿using System.Collections.Generic;
using System.Linq;
using BaseLibS.Param;
using NUnit.Framework;
using PerseusApi.Document;
using PerseusApi.Matrix;
using PerseusApi.Utils;
using PerseusPluginLib.Mods;

namespace PerseusPluginLib.Test.Mods
{
	[TestFixture]
	public class ExpandSiteTableTest : BaseTest
    {
	    [Test]
	    public void TestSmallExample()
	    {
	        var values = new[,]
	        {
	            {0.0f, 1.0f, 0f, 5f},
                {2.0f, 3.0f, 0f, 5f}
	        };
	        var mdata = PerseusFactory.CreateMatrixData(values, new List<string> {"Col___1" , "Col___2", "Col___3", "No expand"});
	        mdata.ColumnDescriptions = new List<string> {"Description Col", "Col", "Col", "Description No expand"};
	        var multiNum = new[]
	        {
	            new[] {0.0, 1.0},
	            new[] {2.0}
	        };
            mdata.AddMultiNumericColumn("MultiNum", "", multiNum);
	        var stringCol = new[] {"row1", "row2"};
            mdata.AddStringColumn("String", "", stringCol);
            var expand = new ExpandSiteTable();
	        IMatrixData[] supplData = null;
	        IDocumentData[] docs = null;
            expand.ProcessData(mdata, new Parameters(), ref supplData, ref docs, CreateProcessInfo());
            Assert.AreEqual(2, mdata.ColumnCount); 
            CollectionAssert.AreEqual(new [] {"No expand", "Col"}, mdata.ColumnNames.ToArray());
            Assert.AreEqual(2, mdata.ColumnDescriptions.Count);
            CollectionAssert.AreEqual(new [] {"Description No expand", "Description Col"}, mdata.ColumnDescriptions.ToArray());
            Assert.AreEqual(6, mdata.RowCount);
            Assert.AreEqual(2, mdata.StringColumnCount);
            CollectionAssert.AreEqual(new [] {"String", "Unique identifier"}, mdata.StringColumnNames);
            CollectionAssert.AreEqual(stringCol.Concat(stringCol).Concat(stringCol).ToArray(), mdata.StringColumns[0]);
            Assert.AreEqual(1, mdata.MultiNumericColumnCount);
            CollectionAssert.AreEqual(multiNum.Concat(multiNum).Concat(multiNum).ToArray(), mdata.MultiNumericColumns[0]);
	    }
	}
}