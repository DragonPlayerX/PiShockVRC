# PiShockVRC

## Disclaimer

**This mod is an unofficial project and I'm not a member of the PiShock team.**

## Requirements

- [MelonLoader 0.5.3+](https://melonwiki.xyz/)

## Description

This mod comes with a UnityPackage that allows you to put "PiShockPoints" on your VRChat avatar which will trigger your PiShock device through their API when other people touch it. It supports all kind of modification like type, strength, duration and touch radius.

## Installation

1. Download and install [MelonLoader 0.5.3+](https://melonwiki.xyz/)
2. Download the ZIP file of [PiShockVRC](https://github.com/DragonPlayerX/PiShockVRC/releases/latest)
3. Open the ZIP file and move the **PiShockVRC.dll** to your **"VRChat/Mods"** folder
4. Start your game to generate all files/settings
5. Insert your settings (see [Configuration Help](https://github.com/DragonPlayerX/PiShockVRC#configuration-help)) to the mod configuration. It's accessible in **"VRChat/UserData/MelonPreferences.cfg"** or otherwise I recommend ingame modification with [UIExpansionKit](https://github.com/knah/VRCMods/releases/latest/download/UIExpansionKit.dll)
6. Follow the instructions given in the **"PiShockUnityInstructions.pdf"** which included in the download to prepare your avatar

## Configuration Help

|Setting|Description|Used for|
|-|-|-|
|Username|Name of your PiShock account|Online API|
|ApiKey|ApiKey found on the website|Online API|
|Local Address|This is the IP address of your PiShock|Local WebSocket|
|Local PiShock ID|This is the ID of your PiShock account|Local WebSocket|

## Avatar Parameters

You can enable the function “Use Avatar Parameters” with [UIExpansionKit](https://github.com/knah/VRCMods/releases/latest/download/UIExpansionKit.dll) or in the [MelonLoader](https://melonwiki.xyz/) preferences file. With this enabled, it will set an avatar parameter (bool type) to true for the given duration when your device gets touched. You can have a parameter for each device.

Parameter name is defined as the following: **PiShock_{device}**

**Example Parameters:**
<br>
![Test](https://i.imgur.com/myWVlDf.png)