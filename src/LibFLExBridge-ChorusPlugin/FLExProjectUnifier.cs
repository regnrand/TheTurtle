// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SIL.Code;
using SIL.IO;
using SIL.Progress;
using LibFLExBridgeChorusPlugin.Contexts;
using LibFLExBridgeChorusPlugin.Infrastructure;

namespace LibFLExBridgeChorusPlugin
{
	/// <summary>
	/// Put the multiple files back together into the main fwdata file.
	/// </summary>
	/// <remarks>
	/// NB: The client of the unifier service decides if new information was found,
	/// and that client decides to call the service, or not.
	/// </remarks>
	internal static class FLExProjectUnifier
	{
		internal static void PutHumptyTogetherAgain(IProgress progress, bool writeVerbose, string mainFilePathname)
		{
			Guard.AgainstNull(progress, "progress");
			FileWriterService.CheckPathname(mainFilePathname);

			using (var tempFile = new TempFile())
			{
				using (var writer = XmlWriter.Create(tempFile.Path, new XmlWriterSettings
					{
						// NB: These are the FW bundle of settings, not the canonical settings.
						OmitXmlDeclaration = false,
						CheckCharacters = true,
						ConformanceLevel = ConformanceLevel.Document,
						Encoding = new UTF8Encoding(false),
						Indent = true,
						IndentChars = (""),
						NewLineOnAttributes = false
					}))
				{
					var pathRoot = Path.GetDirectoryName(mainFilePathname);
					// NB: The method calls are strictly ordered.
					// Don't even think of changing them.
					if (writeVerbose)
					{
						progress.WriteVerbose("Processing data model version number....");
					}
					else
					{
						progress.WriteMessage("Processing data model version number....");
					}
					UpgradeToVersion(writer, pathRoot);
					if (writeVerbose)
					{
						progress.WriteVerbose("Processing custom properties....");
					}
					else
					{
						progress.WriteMessage("Processing custom properties....");
					}
					WriteOptionalCustomProperties(writer, pathRoot);

					var sortedData = BaseDomainServices.PutHumptyTogetherAgain(progress, writeVerbose, pathRoot);

					if (writeVerbose)
					{
						progress.WriteVerbose("Writing temporary fwdata file....");
					}
					else
					{
						progress.WriteMessage("Writing temporary fwdata file....");
					}
					foreach (var rtElement in sortedData.Values)
					{
						FileWriterService.WriteElement(writer, rtElement);
					}
					writer.WriteEndElement();
				}
				if (writeVerbose)
				{
					progress.WriteVerbose("Copying temporary fwdata file to main file....");
				}
				else
				{
					progress.WriteMessage("Copying temporary fwdata file to main file....");
				}
				File.Copy(tempFile.Path, mainFilePathname, true);
			}

			if (!NeedsToBeSplitAgain(Path.GetDirectoryName(mainFilePathname)))
			{
				return;
			}
			var projName = Path.GetFileName(mainFilePathname);
			progress.WriteMessage("Split up project file: {0} (again)", projName);
			FLExProjectSplitter.PushHumptyOffTheWall(progress, writeVerbose, mainFilePathname);
			progress.WriteMessage("Finished splitting up project file: {0} (again)", projName);
		}

		/// <summary>
		/// Check to see if the fwdata file needs to be resplit.
		/// </summary>
		/// <returns>'true', if there are any temp files that mark incompatible moves exists (has 'dupid' extension), otherwise 'false'.</returns>
		private static bool NeedsToBeSplitAgain(string pathRoot)
		{
			var baseFoldersThatHaveNestedData = new HashSet<string>
				{
					"Anthropology",
					"General",
					"Linguistics"
				};
			var dupidPathnames = new List<string>();
			foreach (var nestedFolderBase in baseFoldersThatHaveNestedData)
			{
				var nestedFolder = Path.Combine(pathRoot, nestedFolderBase);
				if (Directory.Exists(nestedFolder))
				{
					dupidPathnames.AddRange(Directory.GetFiles(nestedFolder, "*." + FlexBridgeConstants.dupid, SearchOption.AllDirectories));
				}
			}
			if (!dupidPathnames.Any())
			{
				return false;
			}
			foreach (var dupidPathname in dupidPathnames)
			{
				File.Delete(dupidPathname);
			}
			return true;
		}

		private static void UpgradeToVersion(XmlWriter writer, string pathRoot)
		{
			writer.WriteStartElement("languageproject");

			// Write out version number from the ModelVersion file.
			var version = LibFLExBridgeUtilities.GetFlexModelVersion(pathRoot);
			writer.WriteAttributeString("version", version);

			var mdc = MetadataCache.MdCache; // This may really need to be a reset
			mdc.UpgradeToVersion(int.Parse(version));
		}

		private static void WriteOptionalCustomProperties(XmlWriter writer, string pathRoot)
		{
			// Write out optional custom property data to the fwdata file.
			// The foo.CustomProperties file will exist, even if it has nothing in it, but the "AdditionalFields" root element.
			var optionalCustomPropFile = Path.Combine(pathRoot, FlexBridgeConstants.CustomPropertiesFilename);
			var doc = XDocument.Load(optionalCustomPropFile);
			var customFieldElements = doc.Root.Elements(FlexBridgeConstants.CustomField).ToList();
			if (!customFieldElements.Any())
			{
				return;
			}

			var mdc = MetadataCache.MdCache;
			foreach (var cf in customFieldElements)
			{
				// Remove 'key' attribute from CustomField elements, before writing to main file.
				cf.Attribute("key").Remove();
				// Restore type attr for object values.
				var propType = cf.Attribute("type").Value;
				cf.Attribute("type").Value = MetadataCache.RestoreAdjustedTypeValue(propType);

				mdc.GetClassInfo(cf.Attribute(FlexBridgeConstants.Class).Value).AddProperty(new FdoPropertyInfo(cf.Attribute(FlexBridgeConstants.Name).Value, propType, true));
			}
			mdc.ResetCaches();
			FileWriterService.WriteElement(writer, doc.Root);
		}
	}
}
