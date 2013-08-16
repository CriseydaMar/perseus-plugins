﻿using System;
using System.Collections.Generic;
using System.Drawing;
using BasicLib.Param;
using BasicLib.Util;
using PerseusApi;
using PerseusApi.Document;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Group {
	public class JoinTermsInCategoricalRow : IMatrixProcessing {
		public bool HasButton { get { return false; } }
		public Image ButtonImage { get { return null; } }
		public string HelpDescription { get { return "The selected terms in the categorical row will be joined to one term."; } }
		public string HelpOutput { get { return "The filtered matrix."; } }
		public HelpType HelpDescriptionType { get { return HelpType.PlainText; } }
		public HelpType HelpOutputType { get { return HelpType.PlainText; } }
		public HelpType[] HelpSupplTablesType { get { return new HelpType[0]; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Name { get { return "Join terms in categorical row"; } }
		public string Heading { get { return "Annotation rows"; } }
		public bool IsActive { get { return true; } }
		public float DisplayOrder { get { return 20; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public HelpType[] HelpDocumentTypes { get { return new HelpType[0]; } }
		public int NumDocuments { get { return 0; } }

		public int GetMaxThreads(Parameters parameters) {
			return 1;
		}

		public Parameters GetParameters(IMatrixData mdata, ref string errorString) {
			Parameters[] subParams = new Parameters[mdata.CategoryRowCount];
			for (int i = 0; i < mdata.CategoryRowCount; i++) {
				string[] values = mdata.GetCategoryRowValuesAt(i);
				int[] sel = values.Length == 1 ? new[] { 0 } : new int[0];
				subParams[i] =
					new Parameters(new Parameter[]{
						new MultiChoiceParam("Values", sel)
						{Values = values, Help = "The value that should be present to discard/keep the corresponding row."}
					});
			}
			return
				new Parameters(new Parameter[]{
					new SingleChoiceWithSubParams("Row"){
						Values = mdata.CategoryRowNames, SubParams = subParams,
						Help = "The categorical row that the filtering should be based on.", ParamNameWidth = 50, TotalWidth = 731
					},
					new StringParam("New term") 
				});
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo) {
			SingleChoiceWithSubParams p = param.GetSingleChoiceWithSubParams("Row");
			int colInd = p.Value;
			if (colInd < 0) {
				processInfo.ErrString = "No categorical rows available.";
				return;
			}
			MultiChoiceParam mcp = p.GetSubParameters().GetMultiChoiceParam("Values");
			int[] inds = mcp.Value;
			if (inds.Length < 1) {
				processInfo.ErrString = "Please select at least two terms for merging.";
				return;
			}
			string newTerm = param.GetStringParam("New term").Value;
			if (newTerm.Length == 0){
				processInfo.ErrString = "Please specify a new term.";
				return;
			}

			string[] values = new string[inds.Length];
			for (int i = 0; i < values.Length; i++) {
				values[i] = mdata.GetCategoryRowValuesAt(colInd)[inds[i]];
			}
			HashSet<string> value = new HashSet<string>(values);
			string[][] cats = mdata.GetCategoryRowAt(colInd);
			string[][] newCat = new string[cats.Length][];
			for (int i = 0; i < cats.Length; i++){
				string[] w = cats[i];
				bool changed = false;
				for (int j = 0; j < w.Length; j++){
					if (value.Contains(w[j])){
						w[j] = newTerm;
						changed = true;
					}
				}
				if (changed){
					Array.Sort(w);
				}
				newCat[i] = w;
			}
			mdata.SetCategoryRowAt(newCat, colInd);
		}
	}
}
