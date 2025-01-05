# GPF Editor
An editor for GPF font files for Grand Prix 4.

## Description

This is a simple editor for GPF font files used in Grand Prix 4. It allows to easily convert to and from TGA files and edit the font character table. It also allows to save the necessary patch files for the game to use fonts at different resolutions than the default 256x256.

## Getting Started

### Dependencies

* [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)
* [ini-parser](https://github.com/rickyah/ini-parser)

### Prerequisites

* Grand Prix 4
* CSM/ZazTools (optional)
* GP4 Memory Access (optional)

### Installing

Simply extract the executable to your preferred folder/location. The editor can be launched directly from the folder.

### Usage

The editor is farily simple to use. The first step is to load a GPF file. This can be done by clicking on "Open GPF" and selecting the desired GPF file.

The GPF format is composed of a texture and a character table. The texture is displayed on the left side of the editor, and the character table is displayed on the right side. The character table is a list of characters, each with a width (in pixels). The row height (in pixels), is the same for all rows.

The texture can be exported to a TGA file by clicking on the "Export TGA" button. To replace the texture, click on the "Import TGA" button and select the desired TGA file. The TGA file must be square and grayscale. If the resolution is different than the one from the GPF file currently loaded, the editor will prompt to resize the character table to fit the new resolution. 

To display the grid of the character table, click on the "Show Character Grid" button. The grid can be used to easily identify the characters in the texture and its boundaries. The grid is automatically updated when the character table is modified.

To save the GPF file, click on the "Save GPF" button.

Grand Prix 4 natively supports GPF fonts with a resolution of 256x256. To use fonts with a different resolution, the game must be patched. The editor can generate the necessary patch files by clicking on the "Export Patch" button. Three equivalent patch files are generated: 
* CAP file - to be used with CSM/ZazTools
* patch.ini file - to be used with CSM/ZazTools
* target.ini file - to be used with GP4 Memory Access


## Authors

Diego "Öggo" Noriega

## Version History

* 1.0
    * First Release
