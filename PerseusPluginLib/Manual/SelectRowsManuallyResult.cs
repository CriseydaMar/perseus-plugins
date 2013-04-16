﻿using System;
using System.Windows.Forms;
using PerseusApi;

namespace PerseusPluginLib.Manual{
	[Serializable]
	public class SelectRowsManuallyResult : IMatrixAnalysisResult{
		private readonly IMatrixData mdata;

		public SelectRowsManuallyResult(IMatrixData mdata){
			this.mdata = mdata;
		}

		public Control GetControl(Action<string> updateStatusLabel, Action<IMatrixData> newMatrix){
			return new SelectRowsManuallyControl(mdata, newMatrix);
		}

		public string Heading { get { return "Select rows manually"; } }
	}
}