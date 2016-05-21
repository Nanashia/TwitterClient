using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twicli
{
	static public class Twitter
	{
		static public string consumer_key =
				"";
		static public string consumer_secret = 
				"";

		static private OAuth.OAuthSession session;

		static public Uri AuthenticatePhase1()
		{
			session = OAuth.Authorize(consumer_key, consumer_secret);
			return session.AuthorizeUri;
		}

		static public Tokens AuthenticatePhase2(string pin)
		{
			var tokens = OAuth.GetTokens(session, pin);
			session = null;

			var setting = Settings.Get();
			setting.cached_access_key    = tokens.AccessToken; 
			setting.cached_access_secret = tokens.AccessTokenSecret;
			setting.cached_user_id       = tokens.UserId;
			setting.cached_screen_name   = tokens.ScreenName;
			Settings.Save();

			return tokens;
		}

		static public Tokens Authenticate(string accessKey, string accessSecret, long userId, string screenName)
		{
			session = null;
			return Tokens.Create(
				consumer_key,
				consumer_secret,
				accessKey,
				accessSecret,
				userId,
				screenName);
		}

	}
}
