// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Chorus.FileTypeHandlers;
using LibFLExBridgeChorusPlugin.Handling;
using LibFLExBridgeChorusPlugin.Infrastructure;
using NUnit.Framework;

namespace LibFLExBridgeChorusPluginTests.Handling
{
	[TestFixture]
	public class LoadHandlersTests
	{
		private IChorusFileTypeHandler _commonHandler;

		[SetUp]
		public void TestSetup()
		{
			_commonHandler = new FieldWorksCommonFileHandler();
		}

		[TearDown]
		public void TestTearDown()
		{
			_commonHandler = null;
		}

		[Test]
		public void EnsureHandlerIsLoaded()
		{
			Assert.IsNotNull(_commonHandler);
		}

		[Test]
		public void EnsureAllSupportedExtensionsAreReturnedBelow9000000()
		{
			var supportedExtensions = new HashSet<string>
			{
				// Common
				FlexBridgeConstants.ModelVersion,		// 'ModelVersion' Better validation done.
				FlexBridgeConstants.CustomProperties,	// 'CustomProperties' Better validation done.
				FlexBridgeConstants.Style,				// 'style'
				FlexBridgeConstants.List,				// 'list'

				// General
				FlexBridgeConstants.langproj,			// 'langproj'
				FlexBridgeConstants.Annotation,			// 'annotation'
				FlexBridgeConstants.Filter,				// 'filter'
				FlexBridgeConstants.orderings,			// 'orderings'
				FlexBridgeConstants.pictures,			// 'pictures'

				// Anthropology
				FlexBridgeConstants.Ntbk,				// 'ntbk'

				// Linguistics
				FlexBridgeConstants.Reversal,			// 'reversal'
				FlexBridgeConstants.Lexdb,				// 'lexdb' The lexicon only added one new extension "lexdb", as the lists are already taken care of.
				FlexBridgeConstants.TextInCorpus,		// 'textincorpus' Text corpus only added one new extension "textincorpus", as the list is already taken care of.
				FlexBridgeConstants.Inventory,			// 'inventory' inventory
				FlexBridgeConstants.DiscourseExt,		// 'discourse' discourse
				FlexBridgeConstants.Featsys,			// 'featsys' Feature structure systems (Phon and Morph & Syn)
				FlexBridgeConstants.Phondata,			// 'phondata'
				FlexBridgeConstants.Morphdata,			// 'morphdata'
				FlexBridgeConstants.Agents,				// 'agents'

				// Scripture
				FlexBridgeConstants.ArchivedDraft,		// ArchivedDraft
				FlexBridgeConstants.ImportSetting,		// ImportSetting
				FlexBridgeConstants.bookannotations,	// bookannotations
				FlexBridgeConstants.book,				// book
				FlexBridgeConstants.Srs,				// srs
				FlexBridgeConstants.Trans,				// trans

				// FW layouts
				FlexBridgeConstants.fwlayout			// 'fwlayout'
			};

			MetadataCache.TestOnlyNewCache.UpgradeToVersion(7000065);
			var knownExtensions = new HashSet<string>(_commonHandler.GetExtensionsOfKnownTextFileTypes());
			Assert.IsTrue(knownExtensions.SetEquals(supportedExtensions));
		}

		[Test]
		public void EnsureAllSupportedExtensionsAreReturnedAt9000000()
		{
			var supportedExtensions = new HashSet<string>
			{
				// Common
				FlexBridgeConstants.ModelVersion,		// 'ModelVersion' Better validation done.
				FlexBridgeConstants.CustomProperties,	// 'CustomProperties' Better validation done.
				FlexBridgeConstants.Style,				// 'style'
				FlexBridgeConstants.List,				// 'list'

				// General
				FlexBridgeConstants.langproj,			// 'langproj'
				FlexBridgeConstants.Annotation,			// 'annotation'
				FlexBridgeConstants.Filter,				// 'filter'
				FlexBridgeConstants.orderings,			// 'orderings'
				FlexBridgeConstants.pictures,			// 'pictures'

				// Anthropology
				FlexBridgeConstants.Ntbk,				// 'ntbk'

				// Linguistics
				FlexBridgeConstants.Reversal,			// 'reversal'
				FlexBridgeConstants.Lexdb,				// 'lexdb' The lexicon only added one new extension "lexdb", as the lists are already taken care of.
				FlexBridgeConstants.TextInCorpus,		// 'textincorpus' Text corpus only added one new extension "textincorpus", as the list is already taken care of.
				FlexBridgeConstants.Inventory,			// 'inventory' inventory
				FlexBridgeConstants.DiscourseExt,		// 'discourse' discourse
				FlexBridgeConstants.Featsys,			// 'featsys' Feature structure systems (Phon and Morph & Syn)
				FlexBridgeConstants.Phondata,			// 'phondata'
				FlexBridgeConstants.Morphdata,			// 'morphdata'
				FlexBridgeConstants.Agents,				// 'agents'

				// FW layouts
				FlexBridgeConstants.fwlayout			// 'fwlayout'
			};

			MetadataCache.TestOnlyNewCache.UpgradeToVersion(9000000);
			var knownExtensions = new HashSet<string>(_commonHandler.GetExtensionsOfKnownTextFileTypes());
			Assert.IsTrue(knownExtensions.SetEquals(supportedExtensions));
		}

		[Test]
		public void EnsureAllSupportedExtensionsAreReturnedAtOrAbove9000001()
		{
			var supportedExtensions = new HashSet<string>
			{
				// Common
				FlexBridgeConstants.ModelVersion,		// 'ModelVersion' Better validation done.
				FlexBridgeConstants.CustomProperties,	// 'CustomProperties' Better validation done.
				FlexBridgeConstants.Style,				// 'style'
				FlexBridgeConstants.List,				// 'list'

				// General
				FlexBridgeConstants.langproj,			// 'langproj'
				FlexBridgeConstants.Annotation,			// 'annotation'
				FlexBridgeConstants.Filter,				// 'filter'
				FlexBridgeConstants.orderings,			// 'orderings'
				FlexBridgeConstants.pictures,			// 'pictures'

				// Anthropology
				FlexBridgeConstants.Ntbk,				// 'ntbk'

				// Linguistics
				FlexBridgeConstants.Reversal,			// 'reversal'
				FlexBridgeConstants.Lexdb,				// 'lexdb' The lexicon only added one new extension "lexdb", as the lists are already taken care of.
				FlexBridgeConstants.TextInCorpus,		// 'textincorpus' Text corpus only added one new extension "textincorpus", as the list is already taken care of.
				FlexBridgeConstants.Inventory,			// 'inventory' inventory
				FlexBridgeConstants.DiscourseExt,		// 'discourse' discourse
				FlexBridgeConstants.Featsys,			// 'featsys' Feature structure systems (Phon and Morph & Syn)
				FlexBridgeConstants.Phondata,			// 'phondata'
				FlexBridgeConstants.Morphdata,			// 'morphdata'
				FlexBridgeConstants.Agents,				// 'agents'

				// FW layouts
				FlexBridgeConstants.fwlayout,			// 'fwlayout'
				FlexBridgeConstants.fwdictconfig		// 'fwdictconfig'
			};

			MetadataCache.TestOnlyNewCache.UpgradeToVersion(MetadataCache.MaximumModelVersion);
			var knownExtensions = new HashSet<string>(_commonHandler.GetExtensionsOfKnownTextFileTypes());
			Assert.IsTrue(knownExtensions.SetEquals(supportedExtensions));
		}
	}
}
