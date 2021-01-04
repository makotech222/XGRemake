## Introduction

This is a communal, non-profit, just-for-fun project to create a clean-room implementation for the game Xenogears (XG) from the Playstation console. This project contains absolutely no copyrighted code or assets. In order to develop or use this project, one must own the Xenogears NA iso file.

## Architecture

This project is built on .Net 5.0, using the Stride3d game engine. The ResourceRipper tool is built on top of scripts written by reddit user rufaswan, which are written in PHP. A MSSQL database is used to store data related to the game, and is accessed using EF Core 5.0. The GUI is planned to be implemented in html/js + vue.js, with Ultralight as the renderer.

Xenogears.Core singleton class is the main entry point into the actual game logic; All game updates are done from the update loop in this class.

## Developer Environment

* Windows 10 x64.
* Install PHP 7.0 or higher (VS16 x64 Thread Safe) and add installation directory to system PATH environment variable.
* Visual Studio 2019 v16.8
* .NET Core SDK 5.0
* Globally installed EF tools (In Powershell, run 'dotnet tool install --global dotnet-ef')
* Stride3d 4.0.0.1 beta04-1366 (https://stride3d.net/download/)

After setting up the environment, open the solution and build. Then, run the ResourceRipper tool, pass it your XG Iso, and let the tool run (should take 30min-1hour). When that is finished, you should be able to run the game from the solution file in visual studio or the Stride Game studio.

## Current Progress

As it stands, this project is at a barebones level of development. There is a basic implementation of the 'Field' environment, with working animations, movement, and scene transitions. There is a dedicated program for ripping all of the necessary sprites from the .ISO file.

Here is a rather low-res video of what it currently looks like: https://webmshare.com/40j19

## To Do List

###### Immediate Term
- [ ] Move SpriteSheet creation from runtime (In BaseFieldCharacter) to the resourceripper tool, to make it usable in Stride Gamestudio

###### General
- [ ] Figure out a process for creating 3d scenes in blender and opening them in Stride
  - There is currently a very rough .blender file for the first scene in game. I had some issues getting it to import into Stride, and also to prevent it from saving copyrighted asset files in order to load properly. Will need to work out a way to have it load without creating copyright files.
  - Alternatively, we need to update the resource ripper to be able to pull 3d assets and scenes from the ISO file, so we don't have to create them ourselves.
- [ ] Refactor the ResourceRipper project. It wasn't originally intended to grow to the size it did. There are lots of opportunities for cleanup and speed improvements.
- [ ] Move PHP implementation of sprite ripper into C# implementation. Partially started already with one of the scripts being replaced.

###### Field
- [ ] Implement the Dialogue system. Needs Database classes/tables, and likely ResourceRipper support for ripping localization from the ISO. Otherwise, enter in text by hand.
- [ ] Implement a GUI system. There is currently barebones code for loading a GUI using the Ultralight library (which renders a gui from html/js).
- [ ] Implement support for climbing ladders. Should be pretty easy.
- [ ] Update movement/jump speed values. They are somewhat close to real values, but could be tighter.

###### Battle
- [ ] Implement entire battle system. Nothing done here at the moment.

###### Data
- [ ] Need to input into database all the NPCs and their related Animations/Sprites/Dialogues. Currently only Fei/Elly animations are really done.
- [ ] Import all localization from ISO, or enter by hand
