# OSD Sidekick plugin for Stream Deck

## Usage

- Warning: This plugin is only tested for the Gigabyte G27QC Monitor (OSD Sidekick uses 3 ways of communication: VIA, Realtek and Genesys, only the genesys mode is implemented in this plugin, so some monitors may not be compatible)
- Build the project with Visual Studio
- Copy the files `GenLib.dll` `GlFlash_v1.16.fl` `GLHub.dll` `GLHub.ini` `GLHubAPI.dll` from the OSD Sidekick folder in the `com.drosocode.osdsidekick.sdPlugin` folder
- Copy the `com.drosocode.osdsidekick.sdPlugin` folder in `%appdata%\Elgato\StreamDeck\Plugins`

## Configuration

- The Monitor ID is the ID of the monitor that you want to change, it starts at 0, if negative, all monitors will be affected
- input and colortemp are between 0 and 2
- gamma is between 0 and 5
- sharpness and lowbluelight are between 0 and 10
- colorvibrance and blackequalizer are between 0 and 20
- brightness and contrast are between 0 and 100
