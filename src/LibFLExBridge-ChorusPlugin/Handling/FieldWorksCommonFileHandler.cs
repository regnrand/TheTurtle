// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Chorus.FileTypeHandlers;
using Chorus.merge;
using Chorus.VcsDrivers.Mercurial;
using Chorus.merge.xml.generic;
using SIL.IO;
using SIL.Progress;
using LibFLExBridgeChorusPlugin.DomainServices;
using LibFLExBridgeChorusPlugin.Handling.ModelVersion;
using LibFLExBridgeChorusPlugin.Infrastructure;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace LibFLExBridgeChorusPlugin.Handling
{
	[Export(typeof(IChorusFileTypeHandler))]
	internal sealed class FieldWorksCommonFileHandler : IChorusFileTypeHandler
	{
		[Import(typeof(UnknownFileTypeHandlerStrategy))]
		private IFieldWorksFileHandler _unknownFileTypeHandler;

		[ImportMany]
		private readonly List<IFieldWorksFileHandler> _completeFieldWorksFileHandlers_OldAndNew = new List<IFieldWorksFileHandler>();
		/// <summary>
		/// Holds the handlers for the current model version,
		/// with obsolete ones removed and new ones added
		/// (new and old are both relative to the current model version).
		/// </summary>
		private readonly List<IFieldWorksFileHandler> _currentWorkingSetOfFieldWorksFileHandlers = new List<IFieldWorksFileHandler>();
		private int _currentlySetForModelVersion;
		private bool _isChorusMergeTheExe = false;

		internal FieldWorksCommonFileHandler()
		{
			var currentExe = Assembly.GetEntryAssembly();
			if (currentExe != null && currentExe.ManifestModule.Name.ToLowerInvariant() == "chorusmerge.exe")
			{
				_isChorusMergeTheExe = true;
			}
			using (var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
			using (var container = new CompositionContainer(catalog))
			{
				container.ComposeParts(this);
			}
		}

		private IEnumerable<IFieldWorksFileHandler> WorkingSetOfHandlers
		{
			get
			{
				if (_currentWorkingSetOfFieldWorksFileHandlers.Count > 0)
				{
					if (_isChorusMergeTheExe)
					{
						if (_currentlySetForModelVersion != MetadataCache.MdCache.ModelVersion)
						{
							// Hmm. Got reset between first trip through this method and now.
							throw new InvalidOperationException(string.Format("Model version was '{0}', but it is now '{1}'.", _currentlySetForModelVersion, MetadataCache.MdCache.ModelVersion));
						}
					}
					else
					{
						// It is fine for tests to reset the model version more than once.
					}
					return _currentWorkingSetOfFieldWorksFileHandlers;
				}

				// Start with all of them that are imported.
				_currentWorkingSetOfFieldWorksFileHandlers.AddRange(_completeFieldWorksFileHandlers_OldAndNew);
				_currentlySetForModelVersion = MetadataCache.MdCache.ModelVersion;
				foreach (var handler in _completeFieldWorksFileHandlers_OldAndNew)
				{
					if (_currentlySetForModelVersion >= 9000000)
					{
						// Remove Scripture handlers, which went away in 9000000.
						var goners9000000 = new HashSet<string>
							{
								FlexBridgeConstants.ArchivedDraft,
								FlexBridgeConstants.ImportSetting,
								FlexBridgeConstants.bookannotations,
								FlexBridgeConstants.book,
								FlexBridgeConstants.Srs,
								FlexBridgeConstants.Trans
							};
						if (goners9000000.Contains(handler.Extension))
						{
							_currentWorkingSetOfFieldWorksFileHandlers.Remove(handler);
						}
					}
					if (_currentlySetForModelVersion < 9000001)
					{
						// Leave out new dictionary configuration handler, which came in 9000001.
						var newbies9000001 = new HashSet<string>
							{
								FlexBridgeConstants.fwdictconfig
							};
						if (newbies9000001.Contains(handler.Extension))
						{
							_currentWorkingSetOfFieldWorksFileHandlers.Remove(handler);
						}
					}
				}
				return _currentWorkingSetOfFieldWorksFileHandlers;
			}
		}

		private IFieldWorksFileHandler GetHandlerfromExtension(string extension)
		{
			return WorkingSetOfHandlers.FirstOrDefault(handlerCandidate => handlerCandidate.Extension == extension) ?? _unknownFileTypeHandler;
		}

		/// <summary>
		/// All callers merging FieldWorks data need to pass 'true', so the MDC will know about any custom properties for their classes.
		///
		/// Non-object callers (currently only the merge of the custom property definitions themselves) should pass 'false'.
		/// </summary>
		internal static void Do3WayMerge(MergeOrder mergeOrder, MetadataCache mdc, bool addcustomPropertyInformation)
		{
			// Skip doing this for the Custom property definiton file, since it has no real need for the custom prop definitions,
			// which are being merged (when 'false' is provided).
			if (addcustomPropertyInformation)
			{
				mdc.AddCustomPropInfo(mergeOrder); // NB: Must be done before FieldWorksCommonMergeStrategy is created, since it used the MDC.
			}

			var merger = FieldWorksMergeServices.CreateXmlMergerForFieldWorksData(mergeOrder, mdc);
			merger.EventListener = mergeOrder.EventListener;
			var mergeResults = merger.MergeFiles(mergeOrder.pathToOurs, mergeOrder.pathToTheirs, mergeOrder.pathToCommonAncestor);
			// Write out merged data.
			FileWriterService.WriteNestedFile(mergeOrder.pathToOurs, mergeResults.MergedNode);
		}

		#region Implementation of IChorusFileTypeHandler

		public bool CanDiffFile(string pathToFile)
		{
			return CanValidateFile(pathToFile);
		}

		public bool CanMergeFile(string pathToFile)
		{
			return CanValidateFile(pathToFile);
		}

		public bool CanPresentFile(string pathToFile)
		{
			return CanValidateFile(pathToFile);
		}

		public bool CanValidateFile(string pathToFile)
		{
			if (string.IsNullOrEmpty(pathToFile))
				return false;
			if (!File.Exists(pathToFile))
				return false;
			var extension = Path.GetExtension(pathToFile);
			if (string.IsNullOrEmpty(extension))
				return false;
			if (extension[0] != '.')
				return false;

			DoOptionalInitialization(pathToFile);
			var handler = GetHandlerfromExtension(extension.Substring(1));
			return handler.CanValidateFile(pathToFile);
		}

		public void Do3WayMerge(MergeOrder mergeOrder)
		{
			if (mergeOrder == null)
				throw new ArgumentNullException("mergeOrder");

			// Make sure MDC is updated.
			// Since this method is called in another process by ChorusMerge,
			// the MDC that was set up for splitting the file is not available.
			var extension = DoOptionalInitialization(mergeOrder.pathToOurs);

			XmlMergeService.RemoveAmbiguousChildNodes = false; // Live on the edge. Opt out of that expensive code.

			GetHandlerfromExtension(extension).Do3WayMerge(MetadataCache.MdCache, mergeOrder);
		}

		private static string DoOptionalInitialization(string pathname)
		{
			var extension = FileWriterService.GetExtensionFromPathname(pathname);
			DoOptionalModelVersionUpdate(pathname, extension);
			DoOptionalLoadOfCustomProperties(pathname, extension);
			return extension;
		}

		private static void DoOptionalModelVersionUpdate(string pathname, string extension)
		{
			if (extension == FlexBridgeConstants.ModelVersion)
			{
				return;
			}
			var folder = Path.GetDirectoryName(pathname);
			while (!File.Exists(Path.Combine(folder, FlexBridgeConstants.ModelVersionFilename)))
			{
				var parent = Directory.GetParent(folder);
				folder = parent != null ? parent.ToString() : null;
				if (folder == null)
					break;
			}
			// 'folder' should now have the required model version file in it, or null for some tests.
			if (!string.IsNullOrEmpty(folder))
			{
				// Folder will never be null in the wild.
				// It may be null for some tests, but then they should have set the model number, if it is important to the test.
				var ourModelFileData = File.ReadAllText(Path.Combine(folder, FlexBridgeConstants.ModelVersionFilename));
				var desiredModelNumber = int.Parse(ModelVersionFileTypeHandlerStrategy.SplitData(ourModelFileData)[1]);
				MetadataCache.MdCache.UpgradeToVersion(desiredModelNumber);
			}
		}

		private static void DoOptionalLoadOfCustomProperties(string pathname, string extension)
		{
			if (extension != FlexBridgeConstants.CustomProperties)
			{
				return;
			}
			// Find the optional custom prop file in current folder or some parent.
			var folder = Path.GetDirectoryName(pathname);
			var customPropertyPathname = Path.Combine(folder, FlexBridgeConstants.CustomPropertiesFilename);
			while (!File.Exists(customPropertyPathname))
			{
				var parent = Directory.GetParent(folder);
				folder = parent != null ? parent.ToString() : null;
				if (folder == null)
				{
					customPropertyPathname = null;
					break;
				}
				customPropertyPathname = Path.Combine(folder, FlexBridgeConstants.CustomPropertiesFilename);
			}
			// 'customPropertyPathname' should now have the required custom property file in it, or null for some tests.
			if (customPropertyPathname != null)
			{
				MetadataCache.MdCache.AddCustomPropInfo(customPropertyPathname);
			}
		}

		public IEnumerable<IChangeReport> Find2WayDifferences(FileInRevision parent, FileInRevision child, HgRepository repository)
		{
			if (parent == null)
				throw new ArgumentNullException("parent"); // Parent seems not be optional in Chorus usage.
			if (child == null)
				throw new ArgumentNullException("child");
			if (repository == null)
				throw new ArgumentNullException("repository");

			var extension = FileWriterService.GetExtensionFromPathname(child.FullPath);
			return GetHandlerfromExtension(extension).Find2WayDifferences(parent, child, repository);
		}

		public IChangePresenter GetChangePresenter(IChangeReport report, HgRepository repository)
		{
			if (report == null)
				throw new ArgumentNullException("report");
			if (repository == null)
				throw new ArgumentNullException("repository");

			var extension = FileWriterService.GetExtensionFromPathname(report.PathToFile);
			return GetHandlerfromExtension(extension).GetChangePresenter(report, repository);
		}

		public string ValidateFile(string pathToFile, IProgress progress)
		{
			if (progress == null)
				throw new ArgumentNullException("progress");

			if (string.IsNullOrEmpty(pathToFile))
				return "No file to work with.";
			if (!File.Exists(pathToFile))
				return "File does not exist.";
			var extension = Path.GetExtension(pathToFile);
			if (string.IsNullOrEmpty(extension))
				return "File has no extension.";
			if (extension[0] != '.')
				return "File has no extension.";

			var handler = GetHandlerfromExtension(extension.Substring(1));
			var results = handler.ValidateFile(pathToFile);
			if (results != null)
			{
				progress.WriteError("File '{0}' is not valid with message:{1}\t{2}", pathToFile, Environment.NewLine, results);
				progress.WriteWarning("It may also have other problems in addition to the one that was reported.");
			}
			return results;
		}

		public IEnumerable<IChangeReport> DescribeInitialContents(FileInRevision fileInRevision, TempFile file)
		{
			// Skip check, since DefaultChangeReport doesn't require it.
			//if (fileInRevision == null)
			//    throw new ArgumentNullException("fileInRevision");

			// Not used here, so don't fret if it is null.
			//if (file == null)
			//    throw new ArgumentNullException("file");

			return new IChangeReport[] { new DefaultChangeReport(fileInRevision, "Added") };
		}

		public IEnumerable<string> GetExtensionsOfKnownTextFileTypes()
		{
			return WorkingSetOfHandlers.Select(handlerStrategy => handlerStrategy.Extension);
		}

		public uint MaximumFileSize
		{
			get { return int.MaxValue; }
		}

		#endregion
	}
}