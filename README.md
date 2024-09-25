# Site-14 (SCP Fork)
Site 14 is a fork based on the SCP universe set within the Robust/Space Station 14 game, using the Einstein Engines fork as an upstream (see below).  Visit the discord at discord.gg/4gGSWyNbQF for more information on how to contribute or play the game.


# Einstein Engines

<p align="center"><img src="https://raw.githubusercontent.com/Simple-Station/Einstein-Engines/master/Resources/Textures/Logo/splashlogo.png" width="512px" /></p>

---

Einstein Engines is a hard fork of [Space Station 14](https://github.com/space-wizards/space-station-14) built around the ideals and design inspirations of the Baystation family of servers from Space Station 13 with a focus on having modular code that anyone can use to make the RP server of their dreams.
Our founding organization is based on a democratic system whereby our mutual contributors and downstreams have a say in what code goes into their own upstream.
If you are a representative of a former downstream of Delta-V, we would like to invite you to contact us for an opportunity to represent your fork in this new upstream.

Space Station 14 is inspired heavily by Space Station 13 and runs on [Robust Toolbox](https://github.com/space-wizards/RobustToolbox), a homegrown engine written in C#.

As a hard fork, any code sourced from a different upstream cannot ever be merged directly here, and must instead be ported.
All code present in this repository is subject to change as desired by the council of maintainers.

## Links

[Website](https://simplestation.org) | [Discord](https://discord.gg/X4QEXxUrsJ) | [Steam(SSMV Launcher)](https://store.steampowered.com/app/2585480/Space_Station_Multiverse/) | [Steam(WizDen Launcher)](https://store.steampowered.com/app/1255460/Space_Station_14/) | [Standalone](https://spacestationmultiverse.com/downloads/)

## Building

Refer to [the Space Wizards' guide](https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html) on setting up a development environment for general information, but keep in mind that Einstein Engines is not the same and many things may not apply.
We provide some scripts shown below to make the job easier.

### Build dependencies

> - Git
> - .NET SDK 8.0.100 (check the repo for latest requirements).

### Windows

> 1. Clone this repository
> 2. Run `git submodule update --init --recursive` in a terminal to download the engine
> 3. Run `Scripts/bat/buildAllDebug.bat` after making any changes to the source
> 4. Run `Scripts/bat/runQuickAll.bat` to launch the client and the server
> 5. Connect to localhost in the client and play

### Linux

> 1. Clone this repository
> 2. Run `git submodule update --init --recursive` in a terminal to download the engine
> 3. Run `Scripts/sh/buildAllDebug.sh` after making any changes to the source
> 4. Run `Scripts/sh/runQuickAll.sh` to launch the client and the server
> 5. Connect to localhost in the client and play


## License

Please read the [LEGAL.md](./LEGAL.md) file for information on the licenses of the code and assets in this repository.
