using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Twicli
{
	static public class Twitter
	{
		static public readonly string consumerKey =
				"";
		static public readonly string consumerSecret =
				"";

		static public readonly int pinLength = 7;

		static private Dictionary<Uri, OAuth.OAuthSession> tableSessions = new Dictionary<Uri, OAuth.OAuthSession>();

		static async public Task<Uri> AuthenticatePhase1()
		{
			Debug.I("requesting auth url");
			var session = await OAuth.AuthorizeAsync(consumerKey, consumerSecret);

			lock (tableSessions)
				tableSessions.Add(session.AuthorizeUri, session);

			Debug.I("auth url=[" + session.AuthorizeUri + "]");
			return session.AuthorizeUri;
		}

		static async public Task<Tokens> AuthenticatePhase2(Uri uri, string pin)
		{
			Debug.I("requesting auth token");
			OAuth.OAuthSession session = null;
			lock (tableSessions)
			{
				session = tableSessions[uri];
				if (session == null)
				{
					throw new InvalidProgramException("not registered uri=[" + uri + "]");
				}
				tableSessions.Remove(uri);
			}

			var tokens = await OAuth.GetTokensAsync(session, pin);
			session = null;
			Debug.I("auth succeeded");

			Debug.I("restore tokens v=[{0}, {1}, {2}, {3}, ]",
				tokens.AccessToken,
				tokens.AccessTokenSecret,
				tokens.UserId,
				tokens.ScreenName);

			var setting = Settings.Get();
			setting.cached_access_key = tokens.AccessToken;
			setting.cached_access_secret = tokens.AccessTokenSecret;
			setting.cached_user_id = tokens.UserId;
			setting.cached_screen_name = tokens.ScreenName;
			Settings.Save();

			return tokens;
		}

		static public Tokens RestoreTokens()
		{
			var setting = Settings.Get();
			Debug.I("restore tokens v=[{0}, {1}, {2}, {3}, ]",
				setting.cached_access_key,
				setting.cached_access_secret,
				setting.cached_user_id,
				setting.cached_screen_name);
			return RestoreTokens(
				setting.cached_access_key,
				setting.cached_access_secret,
				setting.cached_user_id,
				setting.cached_screen_name);
		}

		static public Tokens RestoreTokens(string accessKey, string accessSecret, long userId, string screenName)
		{
			return Tokens.Create(
				consumerKey,
				consumerSecret,
				accessKey,
				accessSecret,
				userId,
				screenName);
		}

		static public async Task<bool> isVaildTokens(Tokens tokens)
		{
			Debug.I("validating token");
			try
			{
				var res = await tokens.Account.VerifyCredentialsAsync(include_email: false);
			}
			catch (Exception e)
			{
				Debug.D(e.ToString());
				return false;
			}
			return true;
		}
	}
}
