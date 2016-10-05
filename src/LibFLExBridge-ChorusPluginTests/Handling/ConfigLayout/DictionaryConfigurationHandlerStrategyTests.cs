﻿// --------------------------------------------------------------------------------------------
// Copyright (C) 2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System.IO;
using System.Linq;
using LibFLExBridgeChorusPlugin.Infrastructure;
using NUnit.Framework;
using SIL.IO;
using SIL.Progress;
using SIL.TestUtilities;

namespace LibFLExBridgeChorusPluginTests.Handling.ConfigLayout
{
	public class DictionaryConfigurationHandlerStrategyTests : BaseFieldWorksTypeHandlerTests
	{
		private TempFile _configFile;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public override void TestSetup()
		{
			base.TestSetup();
			Mdc.UpgradeToVersion(MetadataCache.MaximumModelVersion);
			_configFile = TempFile.WithExtension("." + FlexBridgeConstants.fwdictconfig);
			// Copy the schema file to where the Strategy looks
			var appsDir = LibFLExBridgeUtilities.GetAppsDir();
			var xsdPath = Path.Combine(appsDir, "TestData", "Language Explorer", "Configuration", FlexBridgeConstants.DictConfigSchemaFilename);
			_tempFolder = new TemporaryFolder("Temp");
			var xsdPathInProj = Path.Combine(_tempFolder.Path, FlexBridgeConstants.DictConfigSchemaFilename);
			File.Copy(xsdPath, xsdPathInProj, true);
		}

		[TearDown]
		public override void TestTearDown()
		{
			base.TestTearDown();
			_tempFolder.Dispose();
			_tempFolder = null;
			_configFile.Dispose();
			_configFile = null;
		}

		[Test]
		public void ExtensionOfKnownFileTypesShouldContain_fwdictconfig()
		{
			var extensions = FileHandler.GetExtensionsOfKnownTextFileTypes().ToArray();
			Assert.AreEqual(FieldWorksTestServices.ExpectedExtensionCount, extensions.Count(), "Wrong number of extensions.");
			Assert.IsTrue(extensions.Contains(FlexBridgeConstants.fwdictconfig));
		}

		[Test]
		public void CanValidateFiles() // Config File and Schema both exist
		{
			Assert.That(FileHandler.CanValidateFile(_configFile.Path));
		}

		[Test]
		public void ValidatesValidFile()
		{
			const string configXml = @"<?xml version='1.0' encoding='utf-8'?>
<DictionaryConfiguration name='Root-based (complex forms as subentries)' allPublications='true' version='1' lastModified='2014-10-07'>
  <ConfigurationItem name='Main Entry' style='Dictionary-Normal' isEnabled='true' field='LexEntry' cssClassNameOverride='entry'>
  <ParagraphOptions paragraphStyle='Dictionary-Normal' continuationParagraphStyle='Dictionary-Continuation' />
	<ConfigurationItem name='Headword' between=' ' after='  ' style='Dictionary-Headword' isEnabled='true' field='MLHeadWord' cssClassNameOverride='mainheadword'>
	  <WritingSystemOptions writingSystemType='vernacular' displayWSAbreviation='false'>
		<Option id='vernacular' isEnabled='true'/>
	  </WritingSystemOptions>
	</ConfigurationItem>
	<ConfigurationItem name='Variant Forms' before='(' between='; ' after=') ' isEnabled='true' field='VariantFormEntryBackRefs'>
	  <ListTypeOptions list='variant'>
		<Option isEnabled='true' id='b0000000-c40e-433e-80b5-31da08771344'/>
		<Option isEnabled='false' id='0c4663b3-4d9a-47af-b9a1-c8565d8112ed'/>
	  </ListTypeOptions>
	</ConfigurationItem>
  </ConfigurationItem>
</DictionaryConfiguration>";
			File.WriteAllText(_configFile.Path, configXml);
			Assert.IsNullOrEmpty(FileHandler.ValidateFile(_configFile.Path, new NullProgress()));
		}

		[Test]
		public void DoesNotValidateMalformedXmlFile()
		{
			const string configXml = @"<?xml version='1.0' encoding='utf-8'?>
<DictionaryConfiguration name='Root-based (complex forms as subentries)' allPublications='true' version='1' lastModified='2014-10-07'>
  </ConfigurationItem>
</DictionaryConfiguration>";
			File.WriteAllText(_configFile.Path, configXml);
			Assert.That(FileHandler.ValidateFile(_configFile.Path, new NullProgress()), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void DoesNotValidateInvalidConfigFile()
		{
			const string configXml = @"<?xml version='1.0' encoding='utf-8'?>
<DictionaryConfiguration name='Root-based (complex forms as subentries)' allPublications='true' version='1' badAttribute='prohibited'/>";
			File.WriteAllText(_configFile.Path, configXml);
			Assert.That(FileHandler.ValidateFile(_configFile.Path, new NullProgress()), Is.Not.Null.Or.Empty);
		}
	}
}