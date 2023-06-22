/* Licensed under the Open Software License version 3.0 */

namespace ZeldaSaveTool;

internal class Message {
	public enum Level {
		I, // Information
		W, // Warning
		E, // Error
	}

	public static void New(Level lvl, string msg) {
		MessageBoxIcon msgIcon = lvl switch {
			Level.I => MessageBoxIcon.Information,
			Level.W => MessageBoxIcon.Warning,
			Level.E => MessageBoxIcon.Error,
			_ => MessageBoxIcon.None
		};

		MessageBox.Show(msg, T(lvl.ToString()),
		MessageBoxButtons.OK, msgIcon);
	}

	public static void Exception(Exception ex) {
		New
		(Level.E, T("Exception", ex.Message));
	}
}