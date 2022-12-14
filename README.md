# KeepToEvernote
This project provides a simple command-line application for converting Google Keep notes into notes that can be imported into Evernote. It was created by me, TheSameCat in order to import my own notes, and I thought it would be useful.

Right now the program supports importing text notes, and to-do style lists.

I tested this against Evernote 10.49.4

# How to get your notes out of Google Keep
Use Google Takeout. It is a simple process. Just uncheck all of the other account information, and leave Keep checked. You will get a download for a zip file containing all of your notes in HTML and JSON format. It is the JSON format we are interested in.

# How to run the program
I suggest copying the executable into the folder with the JSON files, opening a command line, and running:

```KeepToEvernote.exe *.json```

The ENEX files can then be imported into EverNote using the File->Import command.

(Note, you should have the notebook you wish to import to open in Evernote before beginning import.)

# How to contribute
This is not a huge piece of code, but if you see something that could make it better, please submit a PR! If you would like to expand upon this guide, provide screenshots, etc., you can submit a PR for that as well. All requests will be answered promptly.

Thank you for your interest in KeepToEvernote!