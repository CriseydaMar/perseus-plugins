using System.Drawing;
using System.Text.RegularExpressions;
using BasicLib.Param;
using BasicLib.Util;
using PerseusApi;
using PerseusApi.Document;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Rearrange{
	public class RenameColumnsRegexp : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Image ButtonImage { get { return null; } }
		public string HelpDescription { get { return "Rename expression columns with the help of matching part of the name by a regular expression."; } }
		public string HelpOutput { get { return ""; } }
		public HelpType HelpDescriptionType { get { return HelpType.PlainText; } }
		public HelpType HelpOutputType { get { return HelpType.PlainText; } }
		public HelpType[] HelpSupplTablesType { get { return new HelpType[0]; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Name { get { return "Rename columns [reg. ex.]"; } }
		public string Heading { get { return "Matrix rearrangements"; } }
		public bool IsActive { get { return true; } }
		public float DisplayOrder { get { return 1; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public HelpType[] HelpDocumentTypes { get { return new HelpType[0]; } }
		public int NumDocuments { get { return 0; } }

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
			string regexStr = param.GetStringParam("Regular expression").Value;
			Regex regex = new Regex(regexStr);
			for (int i = 0; i < mdata.ExpressionColumnCount; i++){
				string newName = regex.Match(mdata.ExpressionColumnNames[i]).Groups[1].ToString();
				if (string.IsNullOrEmpty(newName)){
					processInfo.ErrString = "Applying parse rule to '" + mdata.ExpressionColumnNames[i] +
						"' results in an empty string.";
					return;
				}
				mdata.ExpressionColumnNames[i] = newName;
			}
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			return
				new Parameters(new Parameter[]{
					new StringParam("Regular expression"){
						Help =
							"The regular expression that determines how the new column names are created from the old " +
								"column names. As an example if you want to transform 'Ratio H/L Normalized Something' " +
								"into 'Something' the suitable regular expression is 'Ratio H/L Normalized (.*)'"
					}
				});
		}
	}
}