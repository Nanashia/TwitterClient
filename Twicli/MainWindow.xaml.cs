using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Twicli
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Task fetchingTask = null;

		public MainWindow()
		{
			InitializeComponent();

			fetchingTask = new Task(()=>
			{
				Tokens tokens = null;
				var setting = Settings.Get();
				if (setting.cached_access_key != "" &&
					setting.cached_access_secret != "")
				{
					try
					{
						tokens = Twitter.Authenticate(
							setting.cached_access_key,
							setting.cached_access_secret,
							setting.cached_user_id,
							setting.cached_screen_name);
						var respond = tokens.Account.VerifyCredentials(false, false, false);
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						tokens = null;
					}
				}

				if (tokens == null)
				{
					var uri = Twitter.AuthenticatePhase1();
					System.Diagnostics.Process.Start(uri.AbsoluteUri);

					string pin = Console.ReadLine();
					tokens = Twitter.AuthenticatePhase2(pin);
				}

				Console.Write("Connected. ");

				var stream = tokens.Streaming.User()
					.OfType<CoreTweet.Streaming.StatusMessage>()
					.Take(100);

				int i = 0;
				foreach (var m in stream)
				{
					Dispatcher.Invoke(
						() => AddStatus(m),
						System.Windows.Threading.DispatcherPriority.Normal
					);

					Console.WriteLine("{0}: {1}: {2}", m.Timestamp, m.Status.User.ScreenName, m.Status.Text);
					if (m.Status.ExtendedEntities != null)
					{
						foreach (var med in m.Status.ExtendedEntities.Media)
						{
							Console.WriteLine(" => " + med.MediaUrlHttps);
						}
					}
				}
			});
			fetchingTask.Start();
		}

		private void AddStatus(CoreTweet.Streaming.StatusMessage m)
		{
			var status = new Status();
			status.DataContext = m.Status;
			list.Children.Insert(0, status);
		}
	}
}
