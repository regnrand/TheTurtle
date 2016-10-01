// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SIL.Code;
using SIL.Progress;
using SIL.Xml;
using LibFLExBridgeChorusPlugin.Contexts;
using LibFLExBridgeChorusPlugin.Infrastructure;

namespace LibFLExBridgeChorusPlugin
{
	/// <summary>
	/// Break up the main fwdata file into multiple files:
	///		A. One file for the custom property declarations (even if there are no custom properties), and
	///		B. One file for the model version
	///		C. Various files for the Domain data.
	/// </summary>
	internal static class FLExProjectSplitter
	{
		internal static void PushHumptyOffTheWall(IProgress progress, bool writeVerbose, string mainFilePathname)
		{
			Guard.AgainstNull(progress, "progress");
			FileWriterService.CheckFilename(mainFilePathname);

			var rootDirectoryName = Path.GetDirectoryName(mainFilePathname);
			// NB: This is strictly an ordered list of method calls.
			// Don't even 'think' of changing any of them.
			LibFLExBridgeUtilities.CheckForUserCancelRequested(progress);
			DeleteOldFiles(rootDirectoryName);
			LibFLExBridgeUtilities.CheckForUserCancelRequested(progress);
			LibFLExBridgeUtilities.WriteVersionFile(mainFilePathname);
			// Outer Dict has the class name for its key and a sorted (by guid) dictionary as its value.
			// The inner dictionary has a caseless guid as the key and the byte array as the value.
			// (Only has current concrete classes.)
			var classData = GenerateBasicClassData();
			var wellUsedElements = new Dictionary<string, XElement>
			{
				{FlexBridgeConstants.LangProject, null},
				{FlexBridgeConstants.LexDb, null}
			};
			var guidToClassMapping = WriteOrCacheProperties(mainFilePathname, classData, wellUsedElements);
			LibFLExBridgeUtilities.CheckForUserCancelRequested(progress);
			BaseDomainServices.PushHumptyOffTheWall(progress, writeVerbose, rootDirectoryName, wellUsedElements, classData, guidToClassMapping);
		}

		private static void DeleteOldFiles(string pathRoot)
		{
			// Wipe out custom props file, as it will be re-created, even if it only has the root element in it.
			var customPropPathname = Path.Combine(pathRoot, FlexBridgeConstants.CustomPropertiesFilename);
			if (File.Exists(customPropPathname))
				File.Delete(customPropPathname);
			// Delete ModelVersion file, but it gets rewritten soon.
			var modelVersionPathname = Path.Combine(pathRoot, FlexBridgeConstants.ModelVersionFilename);
			if (File.Exists(modelVersionPathname))
				File.Delete(modelVersionPathname);

			// Deletes all files in new locations, except the current ChorusNotes files.
			BaseDomainServices.RemoveDomainData(pathRoot);
		}

		private static Dictionary<string, string> WriteOrCacheProperties(string mainFilePathname,
			Dictionary<string, SortedDictionary<string, byte[]>> classData,
			Dictionary<string, XElement> wellUsedElements)
		{
			var pathRoot = Path.GetDirectoryName(mainFilePathname);
			var mdc = MetadataCache.MdCache;
			// Key is the guid of the object, and value is the class name.
			var guidToClassMapping = new Dictionary<string, string>();
			using (var fastSplitter = new FastXmlElementSplitter(mainFilePathname))
			{
				var haveWrittenCustomFile = false;
				bool foundOptionalFirstElement;
				// NB: The main input file *does* have to deal with the optional first element.
				foreach (var record in fastSplitter.GetSecondLevelElementBytes(FlexBridgeConstants.AdditionalFieldsTag, FlexBridgeConstants.RtTag, out foundOptionalFirstElement))
				{
					if (foundOptionalFirstElement)
					{
						// 2. Write custom properties file with custom properties.
						FileWriterService.WriteCustomPropertyFile(mdc, pathRoot, record);
						foundOptionalFirstElement = false;
						haveWrittenCustomFile = true;
					}
					else
					{
						CacheDataRecord(record, wellUsedElements, classData, guidToClassMapping);
					}
				}
				if (!haveWrittenCustomFile)
				{
					// Write empty custom properties file.
					FileWriterService.WriteCustomPropertyFile(Path.Combine(pathRoot, FlexBridgeConstants.CustomPropertiesFilename), null);
				}
			}
			return guidToClassMapping;
		}

		private static Dictionary<string, SortedDictionary<string, byte[]>> GenerateBasicClassData()
		{
			return MetadataCache.MdCache.AllConcreteClasses.ToDictionary(fdoClassInfo => fdoClassInfo.ClassName, fdoClassInfo => new SortedDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase));
		}

		private static void CacheDataRecord(byte[] record,
			IDictionary<string, XElement> wellUsedElements,
			IDictionary<string, SortedDictionary<string, byte[]>> classData,
			IDictionary<string, string> guidToClassMapping)
		{
			var attrValues = XmlUtils.GetAttributes(record, new HashSet<string>
			{
				FlexBridgeConstants.Class,
				FlexBridgeConstants.GuidStr
			});
			var className = attrValues[FlexBridgeConstants.Class];
			var guid = attrValues[FlexBridgeConstants.GuidStr].ToLowerInvariant();
			guidToClassMapping.Add(guid, className);

			// Theory has it the FW data is sorted.
			//// 1. Sort <rt>
			//DataSortingService.SortMainRtElement(rtElement);

			// 2. Cache it.
			switch (className)
			{
				default:
					classData[className].Add(guid, record);
					break;
				case FlexBridgeConstants.LangProject:
					wellUsedElements[FlexBridgeConstants.LangProject] = LibFLExBridgeUtilities.CreateFromBytes(record);
					//classData.Remove(LibTriboroughBridgeConstants.LangProject);
					break;
				case FlexBridgeConstants.LexDb:
					wellUsedElements[FlexBridgeConstants.LexDb] = LibFLExBridgeUtilities.CreateFromBytes(record);
					//classData.Remove(LibTriboroughBridgeConstants.LexDb);
					break;
			}
		}
	}
}
