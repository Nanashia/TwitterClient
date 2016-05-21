using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twicli
{
	[Serializable]
	public class Settings
	{
		#region Load and Save
		static readonly string _filename = "twitter.settings";

		static private Settings _instance;
		static public Settings Get()
		{
			if (_instance == null)
			{
				Stream stream = null;
				try
				{
					System.Runtime.Serialization.IFormatter formatter = 
						new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					stream = new FileStream(_filename,
						FileMode.Open, FileAccess.Read, FileShare.Read);
					_instance = (Settings)formatter.Deserialize(stream);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					_instance = new Settings();
				}
				finally
				{
					stream.Close();
				}
			}
			return _instance;
		}

		static public void Save()
		{
			Stream stream = null;
			try
			{
				System.Runtime.Serialization.IFormatter formatter = 
					new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				stream = new FileStream(_filename,
					FileMode.Create, FileAccess.Write, FileShare.None);
				formatter.Serialize(stream, Get());
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			finally
			{
				stream.Close();
			}
		}

		private Settings()
		{
		}
#endregion

		public string cached_access_key = "";
		public string cached_access_secret = "";
		public long cached_user_id = -1;
		public string cached_screen_name = "";
	}
}
