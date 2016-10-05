﻿// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System.IO;
using Chorus.sync;

namespace LibFLExBridgeChorusPlugin.Infrastructure
{
	internal static class FlexFolderSystem
	{
		internal static void ConfigureChorusProjectFolder(ProjectFolderConfiguration projectFolderConfiguration)
		{
			// Exclude has precedence, but these are redundant as long as we're using the policy
			// that we explicitly include all the files we understand. At least someday, when these
			// affect what happens in a more persistent way (e.g. be stored in the hgrc), these would protect
			// us a bit from other apps that might try to do a *.* include.
			projectFolderConfiguration.ExcludePatterns.Add("**" + FlexBridgeConstants.FwXmlExtension);
			projectFolderConfiguration.ExcludePatterns.Add("**" + FlexBridgeConstants.FwXmlExtension + "-replaced");
			projectFolderConfiguration.ExcludePatterns.Add("**" + FlexBridgeConstants.FwXmlExtension + "-x");
			projectFolderConfiguration.ExcludePatterns.Add("**.fwbackup");
			projectFolderConfiguration.ExcludePatterns.Add("**.fwstub");
			projectFolderConfiguration.ExcludePatterns.Add("**.orig");
			projectFolderConfiguration.ExcludePatterns.Add("**.zip");
			projectFolderConfiguration.ExcludePatterns.Add("**.oxes");
			projectFolderConfiguration.ExcludePatterns.Add("**.oxesa");
			projectFolderConfiguration.ExcludePatterns.Add("**.oxekt");
			projectFolderConfiguration.ExcludePatterns.Add("**.lint");
			projectFolderConfiguration.ExcludePatterns.Add("**.flextext");
			projectFolderConfiguration.ExcludePatterns.Add("**.bak");
			projectFolderConfiguration.ExcludePatterns.Add("**.bad");
			projectFolderConfiguration.ExcludePatterns.Add("**" + FlexBridgeConstants.FwXmlLockExtension);
			projectFolderConfiguration.ExcludePatterns.Add("**.tmp");
			projectFolderConfiguration.ExcludePatterns.Add("**.xml");
			projectFolderConfiguration.ExcludePatterns.Add("**.log");
			projectFolderConfiguration.ExcludePatterns.Add("**." + FlexBridgeConstants.dupid);
			projectFolderConfiguration.ExcludePatterns.Add(Path.Combine("Temp", "**.*"));
			projectFolderConfiguration.ExcludePatterns.Add(Path.Combine("BackupSettings", "**.*"));
			projectFolderConfiguration.ExcludePatterns.Add(Path.Combine("WritingSystemStore", "trash", "**.*"));
			projectFolderConfiguration.ExcludePatterns.Add(Path.Combine("WritingSystemStore", "WritingSystemsToIgnore.xml.ChorusNotes"));
			projectFolderConfiguration.ExcludePatterns.Add(Path.Combine(FlexBridgeConstants.OtherRepositories, "**.*")); // Folder for contined LIFT and PT-FLEx repos.
			if (!projectFolderConfiguration.ExcludePatterns.Contains("**.NewChorusNotes"))
				projectFolderConfiguration.ExcludePatterns.Add("**.NewChorusNotes"); // Not really needed, since Chorus adds them. But, knows for how long?
			ProjectFolderConfiguration.AddExcludedVideoExtensions(projectFolderConfiguration);

			projectFolderConfiguration.IncludePatterns.Add("FLExProject.ModelVersion"); // Hope this forces the version file to be done first.
			projectFolderConfiguration.IncludePatterns.Add("FLExProject.CustomProperties"); // Hope this forces the custom props to be done next.

			// Overhead files.
			projectFolderConfiguration.IncludePatterns.Add("do_not_share_project.txt");
			projectFolderConfiguration.IncludePatterns.Add(".hgignore");

			// Common at all levels.
			if (!projectFolderConfiguration.IncludePatterns.Contains("**.ChorusNotes"))
				projectFolderConfiguration.IncludePatterns.Add("**.ChorusNotes"); // Not really needed, since Chorus adds them. But, knows for how long?
			projectFolderConfiguration.IncludePatterns.Add("**.list");
			projectFolderConfiguration.IncludePatterns.Add("**.style");

			// Misc required files.
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("ConfigurationSettings", "*.fwlayout"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("ConfigurationSettings", "**.fwdictconfig"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("WritingSystemStore", "*.ldml"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("LinkedFiles", "AudioVisual", "**.*"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("LinkedFiles", "Others", "**.*"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("LinkedFiles", "Pictures", "**.*"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("SupportingFiles", "**.*"));

			// Linguistics
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "Reversals", "**.reversal"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "Lexicon", "*.lexdb"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "TextCorpus", "*.textincorpus"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "Inventory", "*.inventory"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "Discourse", FlexBridgeConstants.DiscourseChartFilename));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "**.featsys"));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "MorphologyAndSyntax", FlexBridgeConstants.AnalyzingAgentsFilename));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "MorphologyAndSyntax", FlexBridgeConstants.MorphAndSynDataFilename));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Linguistics", "Phonology", FlexBridgeConstants.PhonologicalDataFilename));

			// Anthropology
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Anthropology", FlexBridgeConstants.DataNotebookFilename));

			if (MetadataCache.MdCache.ModelVersion < 9000000)
			{
				// Scripture
				projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Other", FlexBridgeConstants.ScriptureReferenceSystemFilename));
				projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Other", "*.ArchivedDraft"));
				projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Other", FlexBridgeConstants.ScriptureTransFilename));
				projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Other", FlexBridgeConstants.ImportSettingsFilename));
				projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Other", "Books", "*.bookannotations"));
				projectFolderConfiguration.IncludePatterns.Add(Path.Combine("Other", "Books", "*.book"));
			}

			// Leftovers
			// Style file and user-defined lists ought to be covered, above.
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("General", FlexBridgeConstants.FLExFiltersFilename));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("General", FlexBridgeConstants.FLExAnnotationsFilename));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("General", FlexBridgeConstants.LanguageProjectFilename));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("General", FlexBridgeConstants.FLExUnownedPicturesFilename));
			projectFolderConfiguration.IncludePatterns.Add(Path.Combine("General", FlexBridgeConstants.FLExVirtualOrderingFilename));
		}
	}
}
