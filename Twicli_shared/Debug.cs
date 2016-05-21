using System.Diagnostics;

namespace Twicli
{
    static class Debug
    {
		static string appName = "";
		static Stopwatch watch = Stopwatch.StartNew();

		static Debug() {
		}

		static public void WriteLine(string level, string fmt, params object[] arg)
		{
			var t = (double)watch.ElapsedTicks / Stopwatch.Frequency;

			System.Diagnostics.Debug.WriteLine("[{0,13:F6}][{1}]{2}", t, level, string.Format(fmt, arg));
		}

		static public void D(string fmt, params object[] arg)
		{
			//	WriteLine("D", text);
		}

		static public void I(string fmt, params object[] arg)
		{
			WriteLine("I", fmt, arg);
		}

		static public void W(string fmt, params object[] arg)
		{
			WriteLine("W", fmt, arg);
		}

		static public void E(string fmt, params object[] arg)
		{
			WriteLine("E", fmt, arg);
		}
	}
}
