# FontConverterTFT
Command line tool for Windows that converts TrueType and Bitmap fonts to GFX fonts for Arduino, ESP8266, ESP32 or similar projects. GFX fonts are used for TFT displays.

Most available fonts provide only the 7 bits ASCII character set, so that was the reason why I wrote this program, because I wanted german umlauts. Additionally, the converter supports Windows bitmap fonts (FON). In this case, the original bitmaps of the font files are directly converted into the GFX bitmaps.

When converting TTF, the fonts are rendered using the specified size in points into internal device bitmaps. These bitmaps will be converted into the GFX bitmaps.

The converter's results are C header files as they are provided by Adafruit as GFX fonts. They can be included into Arduino scratches just bei #include them.

## Usage

FontConverterTFT /p:<path> {/f:<family>|/n:<ttf>} [/s:<size>] [/a:<style>] [/r:<first-last>]

Creates a GFXfont header file that can be used for Arduino IDE sketches.

The name of the GFXfont header file is the name of the font + .h.

/p:<path>        : Folder where the resulting code file (*.h) will be stored.

/f:<family>      : Optional name of the installed font family to convert, "Monospace", "Serif" or "SansSerif". One of the three options /f or /n or /b must be specified.

/n:<ttf>         : Optional full path of the TTF font file to convert. One of the three options /f or /n or /b must be specified.

/b:<fon>         : Optional full path of the FON bitmap font file to convert. One of the three options /f or /n or /b must be specified.

/s:<size>        : Size in pt (Point). May be a floating point value. Default is 7.0. Ignored when converting a bitmap font (FON).

/a:<style>       : Optional style (Bold, Italic, Regular, Strikeout, Underline). Combine with '+' (i.e Bold+Italic). Default is Regular.

/r:<7|8>         : Optional ASCII (7 bits) or ANSI (8 bits) range of characters.

## How it works
The converter is able to convert three different types of fonts:
1. Installed fonts of the Windows operating system (/f)
2. Explicitely specified TTF (TrueType Font) files (/n)
3. Explicitely specified FON (Windows bitmap font) files with fixed character sizes (/b)

When converting TrueType fonts the size must be specified by using the /s option. The style /a option can also be used for TrueType fonts.

Each TTF font will be rendered into temporary device bitmaps. These bitmaps are transformed into byte arrays and written into the GFXfont bitmap array. The quality depends on the rendering quality of the font for the specific size. TTF fonts are usually proportional fonts, so also the resulting GFXfonts are proportional. The rendering of the TTF fonts is the reason why the program is running currently only with Windows.

The FON bitmap font files are not rendered. Their bitmaps are read directly from the font resource and written into the GFXfont bitmap array. Their sizes are fixed.
