// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibFLExBridgeChorusPlugin.Infrastructure;
using NUnit.Framework;
using SIL.IO;
using SIL.Progress;

namespace LibFLExBridgeChorusPluginTests.Handling.Scripture
{
	[TestFixture]
	public class FieldWorksArchivedDraftsTypeHandlerTests : BaseFieldWorksTypeHandlerTests
	{
		private TempFile _ourFile;
		private TempFile _theirFile;
		private TempFile _commonFile;

		[SetUp]
		public override void TestSetup()
		{
			base.TestSetup();
			Mdc = MetadataCache.TestOnlyNewCache;
			Mdc.UpgradeToVersion(7000065);
			FieldWorksTestServices.SetupTempFilesWithExtension("." + FlexBridgeConstants.ArchivedDraft, out _ourFile, out _commonFile, out _theirFile);
		}

		[TearDown]
		public override void TestTearDown()
		{
			base.TestTearDown();
			FieldWorksTestServices.RemoveTempFiles(ref _ourFile, ref _commonFile, ref _theirFile);
		}

		[Test]
		public void DescribeInitialContentsShouldHaveAddedForLabel()
		{
			var initialContents = FileHandler.DescribeInitialContents(null, null).ToList();
			Assert.AreEqual(1, initialContents.Count);
			var onlyOne = initialContents.First();
			Assert.AreEqual("Added", onlyOne.ActionLabel);
		}

		[Test]
		public void ExtensionOfKnownFileTypesShouldBeArchivedDraft()
		{
			var extensions = FileHandler.GetExtensionsOfKnownTextFileTypes().ToArray();
			Assert.AreEqual(FieldWorksTestServices.ExpectedExtensionCount, extensions.Length, "Wrong number of extensions.");
			Assert.IsTrue(extensions.Contains(FlexBridgeConstants.ArchivedDraft));
		}

		[Test]
		public void ShouldBeAbleToValidateInProperlyFormattedFile()
		{
			const string data =
@"<?xml version='1.0' encoding='utf-8'?>
<ArchivedDrafts>
<ScrDraft guid='0a0be0c1-39c4-44d4-842e-231680c7cd56' />
</ArchivedDrafts>";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsTrue(FileHandler.CanValidateFile(_ourFile.Path));
		}

		[Test]
		public void ShouldBeAbleToDoAllCanOperations()
		{
			const string data =
@"<?xml version='1.0' encoding='utf-8'?>
<ArchivedDrafts>
<ScrDraft guid='0a0be0c1-39c4-44d4-842e-231680c7cd56' />
</ArchivedDrafts>";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsTrue(FileHandler.CanValidateFile(_ourFile.Path));
			Assert.IsTrue(FileHandler.CanDiffFile(_ourFile.Path));
			Assert.IsTrue(FileHandler.CanMergeFile(_ourFile.Path));
			Assert.IsTrue(FileHandler.CanPresentFile(_ourFile.Path));
		}

		[Test]
		public void ShouldNotBeAbleToValidateFile()
		{
			const string data = "<?xml version='1.0' encoding='utf-8'?><classdata />";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsNotNull(FileHandler.ValidateFile(_ourFile.Path, new NullProgress()));
		}

		[Test]
		public void ShouldBeAbleToValidateFile()
		{
			const string data =
@"<?xml version='1.0' encoding='utf-8'?>
<ArchivedDrafts>
<ScrDraft guid='0a0be0c1-39c4-44d4-842e-231680c7cd56' >
<DateCreated val='2016-08-09 18:48:18.679' />
<Type val='0' />
<Protected val='False' />
</ScrDraft>
</ArchivedDrafts>";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsNull(FileHandler.ValidateFile(_ourFile.Path, new NullProgress()));
		}

		[Test]
		public void BothEditedCanonicalNumInConflictingWayButBothIgnoredSinceScrDraftIsImmutable()
		{
			const string commonAncestor =
				@"<?xml version='1.0' encoding='utf-8'?>
<ArchivedDrafts>
<ScrDraft guid='oldie'>
<Books>
<ScrBook guid='16525edd-c902-43ad-99fa-decb7b751c5d'>
<CanonicalNum val='42' />
</ScrBook>
</Books>
</ScrDraft>
</ArchivedDrafts>";
			var ourContent = commonAncestor.Replace("val='42'", "val='43'");
			var theirContent = commonAncestor.Replace("val='42'", "val='44'");

			using (var modelVersionFile = TempFile.WithFilename(FlexBridgeConstants.ModelVersionFilename))
			{
				// Higher than 69 (e.g, 9000000) would fail, since the Scripture domian ws removed in 9000000.
				const string newModelVersionFileContents = "{\"modelversion\": 7000069}";
				File.WriteAllText(modelVersionFile.Path, newModelVersionFileContents);
				var result = FieldWorksTestServices.DoMerge(
					FileHandler,
					_ourFile, ourContent,
					_commonFile, commonAncestor,
					_theirFile, theirContent,
					null, null,
					0, new List<Type>(),
					0, new List<Type>());

				Assert.IsTrue(result.Contains("val=\"42\""));
			}
		}
	}
}