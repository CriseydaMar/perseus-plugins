using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BaseLibS.Graph;
using BaseLibS.Param;
using PerseusApi.Document;
using PerseusApi.Generic;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Rearrange{
	public class RenameColumnsRegexp : IMatrixProcessing{
		public bool HasButton => false;
		public Bitmap2 DisplayImage => null;

		public string Description
			=> "Rename expression columns with the help of matching part of the name by a regular expression.";

		public string HelpOutput => "";
		public string[] HelpSupplTables => new string[0];
		public int NumSupplTables => 0;
		public string Name => "Rename columns [reg. ex.]";
		public string Heading => "Rearrange";
		public bool IsActive => true;
		public float DisplayRank => 1;
		public string[] HelpDocuments => new string[0];
		public int NumDocuments => 0;

		public string Url
			=> "http://coxdocs.org/doku.php?id=perseus:user:activities:MatrixProcessing:Rearrange:RenameColumnsRegexp";

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
			var vals = param.GetParam<Tuple<Regex, string>>("Regex").Value;
		    var pattern = vals.Item1;
		    string replacementStr = vals.Item2;
			for (int i = 0; i < mdata.ColumnCount; i++){
				mdata.ColumnNames[i] = pattern.Replace(mdata.ColumnNames[i], replacementStr);
			}
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			return
				new Parameters(new RegexReplaceParam("Regex", new Regex("Column (.*)"), "$1", mdata.ColumnNames));
		}
	}
}