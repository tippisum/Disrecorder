# Disrecorder

## Features

Fetches all messages in your discord channel and save them into readable text file.

## Requirements

* .NET Framework 4.5
* [System.Json](https://www.nuget.org/packages/System.Json/)

## Build

* If you haven't installed System.Json, you can just put the required assembly `System.Json.dll` inside `lib/` folder.
* Use Visual Studio, msbuild or xbuild to build the project.
* Built binary can be found in `bin/` folder.

## Usage

* Save all your configurations into `config.json`, put it next to the built binary.
* Invoke the built binary.

## Configuations

* `session` (object): session settings.
  * `authToken` (string): your Discord authorization token.
    You first login using your browser and you can get the token using your browser's debug tool.
    It is sent with the `Authorization` header in every API request.
  * `userAgent` (string, optional): the `User-Agent` header to use.
    Use the same string as your browser.
  * `serverId` (string) and `channelId` (string): the server ID and the channel ID.
    You can extract them from channel URL: `https://discordapp.com/channels/{serverId}/{channelId}`
  * `proxy` (string, optional): proxy setting.
    `host:port` if you use HTTP proxy.
* `snapshot` (string, optional): path for loading/saving snapshot file.
  A snapshot is a JSON file containing raw message data.
  You can save the snapshot for future convertion.
* `limit` (int, optional): limit the number of messages to be fetched.
  Omitted or -1 means no limit.
* `output` (object): output settings.
  * `text` (string, optional): path for the plain text output.
    If specified, a readable plain text file is generated.

## License

Disrecorder is licensed under the AGPL v3 License.
