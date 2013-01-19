using BasicLib.Param;

namespace PerseusApi{
	public interface IMatrixExport : IMatrixActivity{
		void Export(Parameters parameters, IMatrixData data, ProcessInfo processInfo);

		/// <summary>
		/// Define here the parameters that determine the specifics of the export.
		/// </summary>
		/// <param name="mdata">The parameters might depend on the data matrix.</param>
		/// <param name="errString">Set this to a value != null if an error occured. The error string will be displayed to the user.</param>
		/// <returns>The set of parameters.</returns>
		Parameters GetParameters(IMatrixData mdata, ref string errString);
	}
}