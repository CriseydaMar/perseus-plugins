using System.Drawing;
using BasicLib.Param;
using BasicLib.Util;
using PerseusApi;
using PerseusApi.Document;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Rearrange{
	public class SortByColumn : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Image ButtonImage { get { return null; } }
		public string HelpDescription { get { return "Simple sorting by a column."; } }
		public DocumentType HelpDescriptionType { get { return DocumentType.PlainText; } }
		public string HelpOutput { get { return "The same matrix but sorted by the specified column."; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public DocumentType HelpOutputType { get { return DocumentType.PlainText; } }
		public DocumentType[] HelpSupplTablesType { get { return new DocumentType[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Name { get { return "Sort by column"; } }
		public string Heading { get { return "Matrix rearrangements"; } }
		public bool IsActive { get { return true; } }
		public float DisplayOrder { get { return 6; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public DocumentType[] HelpDocumentTypes { get { return new DocumentType[0]; } }
		public int NumDocuments { get { return 0; } }

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
			int ind = param.GetSingleChoiceParam("Column").Value;
			bool descending = param.GetBoolParam("Descending").Value;
			if (ind < mdata.ExpressionColumnCount){
				float[] v = mdata.GetExpressionColumn(ind);
				int[] o = ArrayUtils.Order(v);
				if (descending){
					ArrayUtils.Revert(o);
				}
				mdata.ExtractExpressionRows(o);
			} else{
				double[] v = mdata.NumericColumns[ind - mdata.ExpressionColumnCount];
				int[] o = ArrayUtils.Order(v);
				if (descending){
					ArrayUtils.Revert(o);
				}
				mdata.ExtractExpressionRows(o);
			}
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			string[] choice = ArrayUtils.Concat(mdata.ExpressionColumnNames, mdata.NumericColumnNames);
			return
				new Parameters(new Parameter[]{
					new SingleChoiceParam("Column"){Values = choice, Help = "Select here the column that should be used for sorting."},
					new BoolParam("Descending"){Help = "If checked the values will be sorted largest to smallest."}
				});
		}
	}
}