﻿using BaseLib.ParamWf;
using PerseusApi.Generic;
using PerseusApi.Matrix;

namespace PerseusApi.Document {
	public interface IDocumentProcessing : IDocumentActivity, IProcessing {
		void ProcessData(IDocumentData mdata, ParametersWf param, ref IMatrixData[] supplTables, ref IDocumentData[] documents,
			ProcessInfo processInfo);

		/// <summary>
		/// Define here the parameters that determine the specifics of the processing.
		/// </summary>
		/// <param name="mdata">The parameters might depend on the data matrix.</param>
		/// <param name="errString">Set this to a value != null if an error occured. The error string will be displayed to the user.</param>
		/// <returns>The set of parameters.</returns>
		ParametersWf GetParameters(IDocumentData mdata, ref string errString);
	}
}
