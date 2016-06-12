**The Turtle** is a stand alone program that is used with FieldWorks (http://fieldworks.sil.org; https://github.com/sillsdev/FwDocumentation/wiki)
that supports using Chorus (https://github.com/sillsdev/chorus) to allow multiple users to share data.

## Build notes:
The Turtle depends on several assemblies from Chorus and Palaso.
Versions of these assemblies are no longer in the repo.
Therefore, to build The Turtle, you must get the latest versions of these assemblies by running this in a Bash window:

Windows
download_dependencies_windows.sh

Linux
download_dependencies_linux.sh (out dated)

### Special Mono dependencies:
        $ cp ../libpalaso/lib/Debug/icu.net.dll* ../libpalaso/lib/DebugMono
	$ PATH=/usr/bin:$PATH make [debug|release] #This will prefer the System Mono over fieldworks-mono

### Mercurial
To run The Turtle you must unzip `chorus/lib/common/Mercurial.zip` to the root of TheTurtle.  Then, edit the `mercurial.ini`
file in the Mercurial folder. Add a line like this (with the appropriate path for your TheTurtle folder):

	fixutf8 = C:\Dev\TheTurtle\MercurialExtensions\fixutf8\fixutf8.py

NB: This is in addition to unzipping this folder per the Chorus ReadMe.