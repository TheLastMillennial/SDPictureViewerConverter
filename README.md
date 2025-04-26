# Standard-Definition Picture Viewer 

*For the TI-84 Plus CE (-T) (Python) and the TI-83 Premium CE (Python)*

Take advantage of the full TI-84 Plus CE graph screen without jailbreaking! This converter turns any PNG picture into pure TI-Basic code.

![Example Picture](example-result.png "Example output")

*Photo of a puppy by Alex Glanville @Alex.takes.pics*

To display high resolution pictures that utilize the entire screen, use the High-Definition Picture Viewer: </br>
https://github.com/TheLastMillennial/HD-Picture-Viewer

### Requirements:
- Python 3.10+
    - tivars https://github.com/TI-Toolkit/tivars_lib_py/
    - pillow
    - numpy
    - pathlib

### Instructions:
1. Run SDPictureViewerConverterPy.py
2. Enter the file path to the picture you want to convert.
3. Press Enter.
    1. (alternative) run `python SDPictureViewerConverterPy.py <path/to/picture.png>`
5. Send all generated .8xl files and the .8xp file to your calculator.
    1. Preferably send the files to the Archive.
6. Ensure you have at least 50,000 bytes of RAM free.
    1. Check by pressing 2nd then + then 2
7. Run the generated program on your calculator. The program name should start with "DRAW".
8. The picture will render in about 7 minutes. 
    1. Note: Calculators made before 2019 will take over 13 minutes.

### Changelog:
v2.0.0
- Rewrote C# converter in Python.
- Converter now directly outputs .8x files.
- Rewrote calculator program to be 30% faster by using lists instead of strings.

v1.0.0
- Intial release.
