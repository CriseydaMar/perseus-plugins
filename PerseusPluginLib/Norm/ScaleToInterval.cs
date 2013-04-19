﻿using System.Collections.Generic;
using System.Drawing;
using BasicLib.Param;
using BasicLib.Util;
using PerseusApi;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Norm{
	public class ScaleToInterval : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Image ButtonImage { get { return null; } }
		public string Name { get { return "Scale to interval"; } }
		public string Heading { get { return "Normalization"; } }
		public bool IsActive { get { return true; } }
		public float DisplayOrder { get { return -7; } }
		public string HelpOutput { get { return ""; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public HelpType HelpDescriptionType { get { return HelpType.PlainText; } }
		public HelpType HelpOutputType { get { return HelpType.PlainText; } }
		public HelpType[] HelpSupplTablesType { get { return new HelpType[0]; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public HelpType[] HelpDocumentTypes { get { return new HelpType[0]; } }
		public int NumDocuments { get { return 0; } }
		public string HelpDescription {
			get{
				return
					"A linear transformation is applied to the values in each row/column such that the minima and maxima coincide with the specified values.";
			}
		}

		public int GetMaxThreads(Parameters parameters){
			return 1;
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables, ref IDocumentData[] documents, ProcessInfo processInfo) {
			bool rows = param.GetSingleChoiceParam("Matrix access").Value == 0;
			double min = param.GetDoubleParam("Minimum").Value;
			double max = param.GetDoubleParam("Maximum").Value;
			MapToInterval1(rows, mdata, min, max);
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			return
				new Parameters(new Parameter[]{
					new SingleChoiceParam("Matrix access"){
						Values = new[]{"Rows", "Columns"},
						Help = "Specifies if the analysis is performed on the rows or the columns of the matrix."
					},
					new DoubleParam("Minimum", 0), new DoubleParam("Maximum", 1)
				});
		}

		public static void MapToInterval1(bool rows, IMatrixData data, double min, double max){
			if (rows){
				for (int i = 0; i < data.RowCount; i++){
					List<double> vals = new List<double>();
					for (int j = 0; j < data.ExpressionColumnCount; j++){
						double q = data[i, j];
						if (!double.IsNaN(q) && !double.IsInfinity(q)){
							vals.Add(q);
						}
					}
					double mind;
					double maxd;
					ArrayUtils.MinMax(vals, out mind, out maxd);
					for (int j = 0; j < data.ExpressionColumnCount; j++){
						data[i, j] = (float) (min + (max - min)/(maxd - mind)*(data[i, j] - mind));
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
					double mind;
					double maxd;
					ArrayUtils.MinMax(vals, out mind, out maxd);
					for (int i = 0; i < data.RowCount; i++){
						data[i, j] = (float) (min + (max - min)/(maxd - mind)*(data[i, j] - mind));
					}
				}
			}
		}
	}
}