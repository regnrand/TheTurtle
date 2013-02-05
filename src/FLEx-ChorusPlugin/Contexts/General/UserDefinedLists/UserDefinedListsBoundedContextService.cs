﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FLEx_ChorusPlugin.Infrastructure;
using FLEx_ChorusPlugin.Infrastructure.DomainServices;
using Palaso.Xml;

namespace FLEx_ChorusPlugin.Contexts.General.UserDefinedLists
{
	internal class UserDefinedListsBoundedContextService
	{
		internal static void NestContext(string generalBaseDir,
			IDictionary<string, SortedDictionary<string, string>> classData,
			Dictionary<string, string> guidToClassMapping)
		{
			// Write out each user-defined list (unowned CmPossibilityList) in a separate file.
			var userDefinedLists = classData[SharedConstants.CmPossibilityList].Values.Where(listElement => XmlUtils.GetAttributes(listElement, new HashSet<string> {SharedConstants.OwnerGuid})[SharedConstants.OwnerGuid] == null).ToList();
			if (!userDefinedLists.Any())
				return; // Nothing to do.

			var userDefinedDir = Path.Combine(generalBaseDir, "UserDefinedLists");
			if (!Directory.Exists(userDefinedDir))
				Directory.CreateDirectory(userDefinedDir);

			foreach (var userDefinedListString in userDefinedLists)
			{
				var element = XElement.Parse(userDefinedListString);
				CmObjectNestingService.NestObject(
					false,
					element,
					classData,
					guidToClassMapping);
				FileWriterService.WriteNestedFile(
					Path.Combine(userDefinedDir, "UserList-" + element.Attribute(SharedConstants.GuidStr).Value.ToLowerInvariant() + "." + SharedConstants.List),
					new XElement("UserDefinedList", element));
			}
		}

		internal static void FlattenContext(
			SortedDictionary<string, XElement> highLevelData,
			SortedDictionary<string, XElement> sortedData,
			string generalBaseDir)
		{
			var userDefinedDir = Path.Combine(generalBaseDir, "UserDefinedLists");
			if (!Directory.Exists(userDefinedDir))
				return;

			foreach (var userDefinedListPathname in Directory.GetFiles(userDefinedDir, "*." + SharedConstants.List, SearchOption.TopDirectoryOnly))
			{
				// These are un-owned lists.
				var userDefinedListDoc = XDocument.Load(userDefinedListPathname);
				CmObjectFlatteningService.FlattenOwnerlessObject(userDefinedListPathname,
					sortedData,
					userDefinedListDoc.Root.Element(SharedConstants.CmPossibilityList));
			}
		}
	}
}
