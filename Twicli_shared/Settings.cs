using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Twicli
{
	public class Settings
	{
		#region Load and Save
		static readonly string _filename = "twitter.settings";
		static Newtonsoft.Json.JsonSerializer serializer = null;

		static private Settings _instance;
		static private object _lock = new object();
		static public Settings Get()
		{
			Debug.I("loading settings");
			lock (_lock)
			{
				if (_instance == null)
				{
					Stream stream = null;
					serializer = new JsonSerializer();
					try
					{
						Debug.I("reading " + Path.GetFullPath(_filename));
						using (stream = new FileStream(_filename,
							FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							var reader = new StreamReader(stream);
							_instance = (Settings)serializer.Deserialize(reader, typeof(Settings)); ;
						}
					}
					catch (Exception e)
					{
						Debug.D(e.ToString());
						_instance = new Settings();
					}

				}
			}
			return _instance;
		}

		static public void Save()
		{
			lock (_lock)
			{
				Stream stream = null;
				try
				{
					Debug.I("writing " + Path.GetFullPath(_filename));
					using (stream = new FileStream(_filename,
						FileMode.Create, FileAccess.Write, FileShare.None))
					{
						var writer = new StreamWriter(stream);
						serializer.Serialize(writer, _instance);
					}
				}
				catch (Exception e)
				{
					Debug.D(e.ToString());
				}
			}
		}

		private Settings()
		{
			Debug.I("creating new instance");
		}
		#endregion

		public string cached_access_key = "";
		public string cached_access_secret = "";
		public long cached_user_id = -1;
		public string cached_screen_name = "";
	}
}
