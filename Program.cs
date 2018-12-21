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
using System.IO;
using System.Json;
using System.Reflection;

[assembly: AssemblyTitle("Disrecorder")]
[assembly: AssemblyCopyright("Copyright (C) Tippisum 2018")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Disrecorder {
	static class Program {
		static void Main(string[] args) {
			JsonValue config = JsonValue.Parse(File.ReadAllText("config.json"));
			Session session = new Session(config["session"]);
			string snapshot = null;
			int limit = -1;
			if (config.ContainsKey("limit")) { limit = config["limit"]; }
			if (config.ContainsKey("snapshot")) {
				snapshot = config["snapshot"]; }
			if (!String.IsNullOrEmpty(snapshot) && File.Exists(snapshot)) { session.Data = (JsonArray)JsonValue.Parse(File.ReadAllText(snapshot)); }
			session.LoadMessages(limit);
			if (!String.IsNullOrEmpty(snapshot)) { File.WriteAllText(snapshot, session.Data.ToString()); }
			config = config["output"];
			if (config.ContainsKey("text")) {
				File.WriteAllText(config["text"], session.ToText());
			}
		}
	}
}
