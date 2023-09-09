/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal static class Program {
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main() {
		SetLocalizedStrings(null);

#if (NET6_0_OR_GREATER)
		ApplicationConfiguration.Initialize();
#else
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
#endif
		Application.Run(new ToolForm());
	}
}