#if !(NET35_OR_GREATER)
#pragma warning disable 1685
namespace System.Runtime.CompilerServices {
	[AttributeUsage(AttributeTargets.Method |
	AttributeTargets.Class | AttributeTargets.Assembly)]
	public sealed class ExtensionAttribute : Attribute {}
}
#endif

#if !(NET40_OR_GREATER)
public static class StringExtensions {
	// FIXME: Add the [NotNullWhen(false)]
	public static bool IsNullOrWhiteSpace(this string? value) {
		if (value == null) return true;
		return string.IsNullOrEmpty(value.Trim());
	}
}
#endif

//https://stackoverflow.com/a/45841432
/// <summary>
/// Split Container Control
/// </summary>
public class SplitContainer : System.Windows.Forms.SplitContainer

#if (NET20 || NET30 || NET35)
		, ISupportInitialize
#endif

{
	#region Constructor

	/// <summary>
	/// Constructor
	/// </summary>
	public SplitContainer() : base() { }

	#endregion Constructor

	#region ISupportInitialize Methods

#if (NET20 || NET30 || NET35)

	public void BeginInit() { }

	public void EndInit() { }

#endif

	#endregion ISupportInitialize Methods
}