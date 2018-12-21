/*
 * Copyright (C) 2018 Tippisum. All rights reserved.
 * 
 * This file is part of Disrecorder.
 * 
 * Disrecorder is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 * 
 * Disrecorder is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the license along with Disrecorder.
 * If not, see <http://www.gnu.org/licenses/agpl.txt>.
 */

using System;
using System.Collections.Generic;
using System.Json;
using System.Net;
using System.Text;

namespace Disrecorder {
	public class Session {
		public string AuthToken { get { return authToken; } set { authToken = value; } }
		public string ChannelId { get { return channelId; } set { channelId = value; } }
		public JsonArray Data { get { return data; } set { data = value; } }
		public IWebProxy Proxy { get { return proxy; } set { proxy = value; } }
		public string ServerId { get { return serverId; } set { serverId = value; } }
		public string UserAgent { get { return userAgent; } set { userAgent = value; } }

		public Session(JsonValue config) {
			cookies = new CookieContainer();
			authToken = config["authToken"];
			serverId = config["serverId"];
			channelId = config["channelId"];
			if (config.ContainsKey("proxy")) { proxy = new WebProxy((string)config["proxy"]); }
			if (config.ContainsKey("userAgent")) { userAgent = config["userAgent"]; }
		}

		public void LoadMessages(int limit) {
			if (data == null) { data = new JsonArray(); }
			while (limit < 0 || data.Count < limit) {
				HttpWebRequest request = WebRequest.CreateHttp(data.Count > 0 ? String.Format("https://discordapp.com/api/v6/channels/{0}/messages?before={1}&limit=50", channelId, (string)data[data.Count - 1]["id"]) : String.Format("https://discordapp.com/api/v6/channels/{0}/messages?limit=50", channelId));
				request.Referer = String.Format("https://discordapp.com/channels/{0}/{1}", serverId, channelId);
				request.CookieContainer = cookies;
				if (proxy != null) { request.Proxy = proxy; }
				if (userAgent != null) { request.UserAgent = userAgent; }
				request.Headers[HttpRequestHeader.Authorization] = authToken;
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) { throw new WebException(String.Format("HTTP {0:d} {0}", response.StatusCode)); }
					JsonArray res = (JsonArray)JsonValue.Load(response.GetResponseStream());
					if (res.Count == 0) { break; }
					data.AddRange((IEnumerable<JsonValue>)res);
				}
			}
		}

		public string ToText() {
			StringBuilder builder = new StringBuilder();
			JsonValue message;
			JsonValue v, w;
			int i, j, k;
			string content, id, name;
			string prev = null;
			for (i = data.Count - 1; i >= 0; --i) {
				message = data[i];
				content = message["content"];
				v = message["author"];
				id = v["id"];
				if (id != prev) {
					builder.AppendFormat("\r\n{0} {1}\r\n", (string)v["username"], DateTime.Parse(message["timestamp"]));
					prev = id;
				}
				v = message["mentions"];
				for (j = 0; j < v.Count; ++j) {
					w = v[j];
					id = w["id"];
					name = w["username"];
					content = content.Replace(String.Format("<@{0}>", id), String.Format("<@{0}>", name)).Replace(String.Format("<@!{0}>", id), String.Format("<@!{0}>", name));
				}
				builder.AppendFormat("{0}\r\n", content);
				v = message["attachments"];
				for (j = 0; j < v.Count; ++j) {
					builder.AppendFormat("<ATTACH: {0}>\r\n", (string)v[j]["filename"]);
				}
				v = message["embeds"];
				for (j = 0; j < v.Count; ++j) {
					w = v[j];
					builder.AppendFormat("<EMBED ({0})>\r\n", (string)w["type"]);
					if (w.ContainsKey("title")) { builder.AppendFormat("Title: {0}\r\n", (string)w["title"]); }
					if (w.ContainsKey("author")) { builder.AppendFormat("Author: {0}\r\n", (string)w["author"]["name"]); }
					if (w.ContainsKey("provider")) { builder.AppendFormat("Provider: {0}\r\n", (string)w["provider"]["name"]); }
					if (w.ContainsKey("description")) { builder.AppendFormat("Description: {0}\r\n", (string)w["description"]); }
					if (w.ContainsKey("url")) { builder.AppendFormat("URL: {0}\r\n", (string)w["url"]); }
					builder.Append("</EMBED>\r\n");
				}
			}
			return builder.ToString();
		}

		private string authToken;
		private string channelId;
		private readonly CookieContainer cookies;
		private JsonArray data;
		private IWebProxy proxy;
		private string serverId;
		private string userAgent;

		private static readonly string[] embedFields = { "title", "author", "provider", "description", "url" };
	}
}
