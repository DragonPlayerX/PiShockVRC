# Disclaimer

Since 27/07/2022 VRChat implemented [Easy Anti-Cheat](https://easy.ac) into their game.
This decision is horrible and will totally harm VRChat more than it will help.

They basically removed all access to Quality of Life mods including serval protections mods against common types of crashing and also all the mods that added features to the game which the community very much wanted but VRChat takes a shit ton of time to implement barely anything of it.

If you think EAC will prevent malicious people from harming you, you're wrong. The Anti-Cheat can be bypassed and they will continue doing their stuff.

The community tried to stop this with as much effort as possible, including the most upvoted [Feedback Post](https://feedback.vrchat.com/open-beta/p/eac-in-a-social-vr-game-creates-more-problems-than-it-solves) with 22k votes, a [Petition](https://www.change.org/p/vrchat-delete-anticheat-system) with almost 14.000 signatures and serval YouTube videos, posts and general social media activity. But they haven't listened to us.

**If you're currently subscribed to VRC+ then please consider of cancelling the subscription and leaving the game entirely.**

Also check out [ChilloutVR](https://discord.gg/abi) and [NeosVR](https://discord.gg/NeosVR).

# PiShockVRC

## Disclaimer

**This mod is an unofficial project and I'm not a member of the PiShock team.**

## Requirements

- [MelonLoader 0.5.4+](https://melonwiki.xyz/)

## Description

This mod comes with a UnityPackage that allows you to put "PiShockPoints" on your VRChat avatar which will trigger your PiShock device through their API when other people touch it. It supports all kind of modification like type, strength, duration and touch radius.

## Installation

1. Download and install [MelonLoader 0.5.4+](https://melonwiki.xyz/)
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

## Avatar Dynamics

You can enable the Avatar Dynamics mode of the mod in the config. If you do that, the config entries for "Self Interaction", "Feet Interaction", "Friends Only" and "Default Radius" will be unavailable. In this case these settings have to be changed on the VRC Contact Receiver which is located on the PiShockPoint object.

## Avatar Parameters

You can enable the function “Use Avatar Parameters” with [UIExpansionKit](https://github.com/knah/VRCMods/releases/latest/download/UIExpansionKit.dll) or in the [MelonLoader](https://melonwiki.xyz/) preferences file. With this enabled, it will set an avatar parameter (bool type) to true for the given duration when your device gets touched. You can have a parameter for each device.

Parameter name is defined as the following: **PiShock_{device}**

**Example Parameters:**
<br>
![ParameterExample](https://i.imgur.com/myWVlDf.png)
