using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BasicLib.Param;
using BasicLib.Util;
using PerseusApi;

namespace PerseusPluginLib.Norm{
	public class Subtract : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Image ButtonImage { get { return null; } }
		public string HelpDescription { get { return "The specified quantity calculated on each row/column is subtracted from each value."; } }
		public string HelpOutput { get { return "Normalized expression matrix."; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Name { get { return "Subtract"; } }
		public string Heading { get { return "Normalization"; } }
		public bool IsActive { get { return true; } }
		public float DisplayOrder { get { return -6; } }
		public HelpType HelpDescriptionType { get { return HelpType.PlainText; } }
		public HelpType HelpOutputType { get { return HelpType.PlainText; } }
		public HelpType[] HelpSupplTablesType { get { return new HelpType[0]; } }

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables, ProcessInfo processInfo){
			SingleChoiceWithSubParams access = param.GetSingleChoiceWithSubParams("Matrix access");
			bool rows = access.Value == 0;
			int groupInd;
			if (rows){
				groupInd = access.GetSubParameters().GetSingleChoiceParam("Grouping").Value - 1;
			} else{
				groupInd = -1;
			}
			int what = param.GetSingleChoiceParam("Subtract what").Value;
			if (groupInd < 0){
				SubtractValues(rows, GetFunc(what), mdata);
			} else{
				string[][] catRow = mdata.CategoryRows[groupInd];
				foreach (string[] t in catRow){
					if (t.Length > 1){
						processInfo.ErrString = "The groups are overlapping.";
						return;
					}
				}
				SubtractGroups(mdata, catRow, GetFunc(what));
			}
		}

		private static void SubtractGroups(IMatrixData mdata, IList<string[]> catRow, Func<double[], double> func){
			string[] groupVals = ArrayUtils.UniqueValuesPreserveOrder(catRow);
			foreach (int[] inds in groupVals.Select(groupVal => ZScore.GetIndices(catRow, groupVal))){
				SubtractGroup(mdata, inds, func);
			}
		}

		private static void SubtractGroup(IMatrixData data, IList<int> inds, Func<double[], double> func){
			for (int i = 0; i < data.RowCount; i++){
				double[] vals = new double[inds.Count];
				for (int j = 0; j < inds.Count; j++){
					double q = data[i, inds[j]];
					vals[j] = q;
				}
				double mean = func(vals);
				foreach (int t in inds){
					data[i, t] = (float) ((data[i, t] - mean));
				}
			}
		}

		private static Func<double[], double> GetFunc(int what){
			switch (what){
				case 0:
					return ArrayUtils.Mean;
				case 1:
					return ArrayUtils.Median;
				case 2:
					return ArrayUtils.MostFrequentValue;
				case 3:
					return ArrayUtils.TukeyBiweight;
				case 4:
					return ArrayUtils.TukeyBiweightSe;
				default:
					throw new Exception("Never get here.");
			}
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			return
				new Parameters(new Parameter[]{
					new SingleChoiceWithSubParams("Matrix access"){
						Values = new[]{"Rows", "Columns"},
						SubParams =
							new[]{
								new Parameters(new SingleChoiceParam("Grouping")
								{Values = ArrayUtils.Concat(new[]{"<No grouping>"}, mdata.CategoryRowNames)}),
								new Parameters()
							},
						Help = "Specifies if the subtraction is performed on the rows or the columns of the matrix."
					},
					new SingleChoiceParam("Subtract what")
					{Values = new[]{"Mean", "Median", "Most frequent value", "Tukey's biweight", "Tukey's biweight se"}, Value = 1}
				});
		}

		public static void SubtractValues(bool rows, Func<double[], double> summarize, IMatrixData data){
			if (rows){
				for (int i = 0; i < data.RowCount; i++){
					List<double> vals = new List<double>();
					for (int j = 0; j < data.ExpressionColumnCount; j++){
						double q = data[i, j];
						if (!double.IsNaN(q) && !double.IsInfinity(q)){
							vals.Add(q);
						}
					}
					double med = summarize(vals.ToArray());
					for (int j = 0; j < data.ExpressionColumnCount; j++){
						data[i, j] -= (float) med;
					}
				}
			} else{
				for (int j = 0; j < data.ExpressionColumnCount; j++){
					List<double> vals = new List<double>();
					for (int i = 0; i < data.RowCount; i++){
						double q = data[i, j];
						if (!double.IsNaN(q) && !double.IsInfinity(q)){
							vals.Add(q);
						}
					}
					double med = summarize(vals.ToArray());
					for (int i = 0; i < data.RowCount; i++){
						data[i, j] -= (float) med;
					}
				}
			}
		}
	}
}