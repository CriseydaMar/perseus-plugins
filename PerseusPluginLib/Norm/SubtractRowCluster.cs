using System.Collections.Generic;
using System.Drawing;
using BasicLib.Param;
using BasicLib.Util;
using PerseusApi;
using PerseusApi.Document;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Norm{
	public class SubtractRowCluster : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Image ButtonImage { get { return null; } }
		public string HelpDescription { get { return "Subtract the average pattern of the selected rows from all rows."; } }
		public string HelpOutput { get { return "Normalized expression matrix."; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Name { get { return "Subtract row cluster"; } }
		public string Heading { get { return "Normalization"; } }
		public bool IsActive { get { return true; } }
		public float DisplayOrder { get { return 0; } }
		public DocumentType HelpDescriptionType { get { return DocumentType.PlainText; } }
		public DocumentType HelpOutputType { get { return DocumentType.PlainText; } }
		public DocumentType[] HelpSupplTablesType { get { return new DocumentType[0]; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public DocumentType[] HelpDocumentTypes { get { return new DocumentType[0]; } }
		public int NumDocuments { get { return 0; } }

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
			string[][] col = mdata.GetCategoryColumnAt(param.GetSingleChoiceParam("Indicator column").Value);
			string term = param.GetStringParam("Value").Value;
			List<int> inds = new List<int>();
			for (int i = 0; i < col.Length; i++){
				if (Contains(col[i], term)){
					inds.Add(i);
				}
			}
			float[][] profiles = new float[inds.Count][];
			for (int i = 0; i < profiles.Length; i++){
				profiles[i] = mdata.GetExpressionRow(inds[i]);
				float mean = (float) ArrayUtils.Mean(profiles[i]);
				for (int j = 0; j < profiles[i].Length; j++){
					profiles[i][j] -= mean;
				}
			}
			float[] totalProfile = new float[mdata.ExpressionColumnCount];
			for (int i = 0; i < totalProfile.Length; i++){
				List<float> vals = new List<float>();
				foreach (float[] t in profiles){
					float val = t[i];
					if (float.IsNaN(val) || float.IsInfinity(val)){
						continue;
					}
					vals.Add(val);
				}
				totalProfile[i] = vals.Count > 0 ? ArrayUtils.Median(vals) : float.NaN;
			}
			for (int i = 0; i < mdata.RowCount; i++){
				for (int j = 0; j < mdata.ExpressionColumnCount; j++){
					mdata[i, j] -= totalProfile[j];
				}
			}
		}

		private static bool Contains(IEnumerable<string> strings, string term){
			foreach (string s in strings){
				if (s.Equals(term)){
					return true;
				}
			}
			return false;
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			return
				new Parameters(new Parameter[]{
					new SingleChoiceParam("Indicator column"){Values = mdata.CategoryColumnNames},
					new StringParam("Value", "+")
					{Help = "Rows matching this term in the indicator column will be used as control for the normalization."}
				});
		}
	}
}