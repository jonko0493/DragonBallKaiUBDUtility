This is a utility that is meant to "decompile" the NDS game, Dragon Ball Kai Ultimate Butouden.
Major thanks and credits to @jonko0493 for writing the most of the code found here (basically all I only added 2 files that are based in the stuff he's written).
Hope anyone finds this useful.

To use:
  1. Specify the files you want to use inside the debug properties for the files you want to run, or run the command through command prompt.
  2. To unpack a dsa file, use the command: dsa-unpack -i FILE\YOU\WANT\TO\UNPACK -o DESTINATION\FOLDER
  3. To decompress a .dso file, use the command: convert-dso -i FILE\YOU\WANT\TO\DECOMPRESS -o CREATED\FILE\NAME\AND\DESTINATION
  4. To compress a .dso file, use the command: compress-dso -i FILE\YOU\WANT\TO\COMPRESS -o CREATED\FILE\NAME\AND\DESTINATION

And these are all the functions currently implemented. Enjoy!
