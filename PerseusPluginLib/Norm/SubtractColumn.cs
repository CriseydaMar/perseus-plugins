using System.Drawing;
using BasicLib.Param;
using BasicLib.Util;
using PerseusApi;
using PerseusApi.Document;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Norm{
	public class SubtractColumn : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Image ButtonImage { get { return null; } }
		public string HelpDescription { get { return "Subtract the specified column from all other columns."; } }
		public string HelpOutput { get { return "Normalized expression matrix."; } }
		public HelpType HelpDescriptionType { get { return HelpType.PlainText; } }
		public HelpType HelpOutputType { get { return HelpType.PlainText; } }
		public HelpType[] HelpSupplTablesType { get { return new HelpType[0]; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Name { get { return "Subtract column"; } }
		public string Heading { get { return "Normalization"; } }
		public bool IsActive { get { return true; } }
		public float DisplayOrder { get { return 0; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public HelpType[] HelpDocumentTypes { get { return new HelpType[0]; } }
		public int NumDocuments { get { return 0; } }

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
			int colIndex = param.GetSingleChoiceParam("Control column").Value;
			if (colIndex < mdata.ExpressionColumnCount){
				DivideByColumn(mdata, colIndex);
			} else{
				DivideByColumnNum(mdata, colIndex - mdata.ExpressionColumnCount);
			}
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			string[] controlChoice = ArrayUtils.Concat(mdata.ExpressionColumnNames, mdata.NumericColumnNames);
			return new Parameters(new Parameter[]{new SingleChoiceParam("Control column"){Values = controlChoice}});
		}

		public static void DivideByColumnNum(IMatrixData data, int index){
			int p = data.RowCount;
			int n = data.ExpressionColumnCount;
			float[,] newEx = new float[p,n];
			double[] numCol = data.NumericColumns[index];
			for (int i = 0; i < p; i++){
				for (int j = 0; j < n; j++){
					newEx[i, j] = (float) (data[i, j] - numCol[index]);
				}
			}
			data.ExpressionValues = newEx;
		}

		public static void DivideByColumn(IMatrixData data, int index){
			int p = data.RowCount;
			int n = data.ExpressionColumnCount;
			float[,] newEx = new float[p,n - 1];
			for (int i = 0; i < p; i++){
				for (int j = 0; j < index; j++){
					newEx[i, j] = data[i, j] - data[i, index];
				}
				for (int j = index + 1; j < n; j++){
					newEx[i, j - 1] = data[i, j] - data[i, index];
				}
			}
			bool[,] newImp = new bool[p,n - 1];
			for (int i = 0; i < p; i++){
				for (int j = 0; j < index; j++){
					newImp[i, j] = data.IsImputed[i, j] || data.IsImputed[i, index];
				}
				for (int j = index + 1; j < n; j++){
					newImp[i, j - 1] = data.IsImputed[i, j] || data.IsImputed[i, index];
				}
			}
			data.ExpressionValues = newEx;
			data.IsImputed = newImp;
			data.ExpressionColumnNames.RemoveAt(index);
			data.ExpressionColumnDescriptions.RemoveAt(index);
			for (int i = 0; i < data.CategoryRowCount; i++){
				data.SetCategoryColumnAt(ArrayUtils.RemoveAtIndex(data.GetCategoryColumnAt(i), index), i);
			}
			for (int i = 0; i < data.NumericRowCount; i++){
				data.NumericColumns[i] = ArrayUtils.RemoveAtIndex(data.NumericColumns[i], index);
			}
		}
	}
}