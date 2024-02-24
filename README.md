# FontConverterTFT
Command line tool for Windows that converts TrueType and Bitmap fonts to GFX fonts for Arduino, ESP8266, ESP32 or similar projects. GFX fonts are used for TFT displays.

Most available fonts provide only the 7 bits ASCII character set. This converter has been developed to support other languages than english. Additionally, the converter supports Windows bitmap fonts (FON). In this case, the original bitmaps of the font files are directly converted into the GFX bitmaps.

When converting TTF, the fonts are rendered using the specified size in points into internal device bitmaps. These bitmaps will be converted into the GFX bitmaps.

The converter's results are C header files as they are provided by Adafruit as GFX fonts. They can be included into Arduino scratches just bei #include them.
