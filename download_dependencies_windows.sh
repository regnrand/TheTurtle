#!/bin/bash
# server=build.palaso.org
# build_type=FLExBridge_FLExBridgeDevelopWin32Continuous
# root_dir=.
# Auto-generated by https://github.com/chrisvire/BuildUpdate.
# Do not edit this file by hand!

cd "$(dirname "$0")"

# *** Functions ***
force=0
clean=0

while getopts fc opt; do
case $opt in
f) force=1 ;;
c) clean=1 ;;
esac
done

shift $((OPTIND - 1))

copy_auto() {
if [ "$clean" == "1" ]
then
echo cleaning $2
rm -f ""$2""
else
where_curl=$(type -P curl)
where_wget=$(type -P wget)
if [ "$where_curl" != "" ]
then
copy_curl $1 $2
elif [ "$where_wget" != "" ]
then
copy_wget $1 $2
else
echo "Missing curl or wget"
exit 1
fi
fi
}

copy_curl() {
echo "curl: $2 <= $1"
if [ -e "$2" ] && [ "$force" != "1" ]
then
curl -# -L -z $2 -o $2 $1
else
curl -# -L -o $2 $1
fi
}

copy_wget() {
echo "wget: $2 <= $1"
f1=$(basename $1)
f2=$(basename $2)
cd $(dirname $2)
wget -q -L -N $1
# wget has no true equivalent of curl's -o option.
# Different versions of wget handle (or not) % escaping differently.
# A URL query is the only reason why $f1 and $f2 should differ.
if [ "$f1" != "$f2" ]; then mv $f2\?* $f2; fi
cd -
}


# *** Results ***
# build: FLEx Bridge-develop-Win32-Continuous (FLExBridge_FLExBridgeDevelopWin32Continuous)
# project: FLEx Bridge
# URL: http://build.palaso.org/viewType.html?buildTypeId=FLExBridge_FLExBridgeDevelopWin32Continuous
# VCS: https://github.com/sillsdev/flexbridge.git [develop]
# dependencies:
# [0] build: Chorus-Documentation (bt216)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt216
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Chorus_Help.chm"=>"lib/common"}
#     VCS: https://github.com/sillsdev/chorushelp.git [master]
# [1] chorus-win32-master-nostrongname Continuous (bt437)
#     project: Chorus
#     URL: URL: http://build.palaso.org/viewType.html?buildTypeId=bt437
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"MercurialExtensions"=>"MercurialExtensions", "Autofac.dll"=>"lib/common", "Chorus.exe"=>"lib/Debug", "LibChorus.dll"=>"lib/Debug", "ChorusMerge.exe"=>"lib/Debug", "Mercurial.zip"=>"lib/Debug", "LibChorus.TestUtilities.dll"=>"lib/Debug", "*.pdb"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/chorus.git [master]
# [2] build: Helpprovider (bt225)
#     project: Helpprovider
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt225
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Vulcan.Uczniowie.HelpProvider.dll"=>"lib/common"}
#     VCS: http://hg.palaso.org/helpprovider []
# [5] build: L10NSharp continuous (bt196)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt196
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Release", "L10NSharp.pdb"=>"lib/Release"}
#     VCS: https://bitbucket.org/sillsdev/l10nsharp []
# [6] build: L10NSharp continuous (bt196)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt196
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Debug", "L10NSharp.pdb"=>"lib/Debug"}
#     VCS: https://bitbucket.org/sillsdev/l10nsharp []
# [7] build: icucil-win32-default Continuous (bt14)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt14
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu*.dll"=>"lib/Release", "icu*.config"=>"lib/Release"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [8] build: icucil-win32-default Continuous (bt14)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt14
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu*.dll"=>"lib/Debug", "icu*.config"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [9] build: palaso-win32-master-nostrongname Continuous (bt436)
#     project: libpalaso
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt436
#     revision: latest.lastSuccessful
#     paths: {"Palaso.BuildTasks.dll"=>"lib/Release", "SIL.Core.dll"=>"lib/Release", "SIL.Core.pdb"=>"lib/Release", "SIL.TestUtilities.dll"=>"lib/Release", "SIL.TestUtilities.pdb"=>"lib/Release", "SIL.Windows.Forms.dll"=>"lib/Release", "SIL.Windows.Forms.pdb"=>"lib/Release", "SIL.Lift.dll"=>"lib/Release", "SIL.Lift.pdb"=>"lib/Release", "debug/Palaso.BuildTasks.dll"=>"lib/Debug", "debug/SIL.Core.dll"=>"lib/Debug", "debug/SIL.Core.pdb"=>"lib/Debug", "debug/SIL.TestUtilities.dll"=>"lib/Debug", "debug/SIL.TestUtilities.pdb"=>"lib/Debug", "debug/SIL.Windows.Forms.dll"=>"lib/Debug", "debug/SIL.Windows.Forms.pdb"=>"lib/Debug", "debug/SIL.Lift.dll"=>"lib/Debug", "debug/SIL.Lift.pdb"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/libpalaso.git []

# make sure output directories exist
mkdir -p ./MercurialExtensions
mkdir -p ./lib/Debug
mkdir -p ./lib/Release
mkdir -p ./lib/common

# download common artifact dependencies
copy_auto http://build.palaso.org/guestAuth/repository/download/bt216/latest.lastSuccessful/Chorus_Help.chm ./lib/common/Chorus_Help.chm
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/MercurialExtensions ./MercurialExtensions/MercurialExtensions
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/Mercurial.zip ./lib/common/Mercurial.zip

copy_auto http://build.palaso.org/guestAuth/repository/download/bt225/latest.lastSuccessful/Vulcan.Uczniowie.HelpProvider.dll ./lib/common/Vulcan.Uczniowie.HelpProvider.dll

copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icu.net.dll ./lib/common/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icudt54.dll ./lib/common/icudt54.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuin54.dll ./lib/common/icuin54.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuuc54.dll ./lib/common/icuuc54.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icu.net.dll.config ./lib/common/icu.net.dll.config

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/ChorusMergeModule.msm ./lib/ChorusMergeModule.msm


# Download release versions
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/Autofac.dll ./lib/Release/Autofac.dll

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/Chorus.exe ./lib/Release/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/Chorus.pdb ./lib/Release/Chorus.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.dll ./lib/Release/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.pdb ./lib/Release/LibChorus.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/ChorusMerge.exe ./lib/Release/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/ChorusMerge.pdb ./lib/Release/ChorusMerge.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.TestUtilities.dll ./lib/Release/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.TestUtilities.pdb ./lib/Release/LibChorus.TestUtilities.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Core.dll ./lib/Release/SIL.Core.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Core.pdb ./lib/Release/SIL.Core.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Windows.Forms.dll ./lib/Release/SIL.Windows.Forms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Windows.Forms.pdb ./lib/Release/SIL.Windows.Forms.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Lift.dll ./lib/Release/SIL.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Lift.pdb ./lib/Release/SIL.Lift.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.TestUtilities.dll ./lib/Release/SIL.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.TestUtilities.pdb ./lib/Release/SIL.TestUtilities.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/Palaso.BuildTasks.dll ./lib/Release/Palaso.BuildTasks.dll

copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.dll ./lib/Release/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.pdb ./lib/Release/L10NSharp.pdb

# Download debug versions
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/Autofac.dll ./lib/Debug/Autofac.dll

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/Chorus.exe ./lib/Debug/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/Chorus.pdb ./lib/Debug/Chorus.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.dll ./lib/Debug/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.pdb ./lib/Debug/LibChorus.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/ChorusMerge.exe ./lib/Debug/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/ChorusMerge.pdb ./lib/Debug/ChorusMerge.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.TestUtilities.dll ./lib/Debug/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt437/latest.lastSuccessful/LibChorus.TestUtilities.pdb ./lib/Debug/LibChorus.TestUtilities.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Core.dll ./lib/Debug/SIL.Core.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Core.pdb ./lib/Debug/SIL.Core.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Windows.Forms.dll ./lib/Debug/SIL.Windows.Forms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Windows.Forms.pdb ./lib/Debug/SIL.Windows.Forms.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Lift.dll ./lib/Debug/SIL.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Lift.pdb ./lib/Debug/SIL.Lift.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.TestUtilities.dll ./lib/Debug/SIL.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.TestUtilities.pdb ./lib/Debug/SIL.TestUtilities.pdb

copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/Palaso.BuildTasks.dll ./lib/Debug/Palaso.BuildTasks.dll

copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.dll ./lib/Debug/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.pdb ./lib/Debug/L10NSharp.pdb
# End of script
