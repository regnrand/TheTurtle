// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chorus.FileTypeHandlers.xml;
using LibFLExBridgeChorusPlugin.Infrastructure;
using NUnit.Framework;
using SIL.IO;
using SIL.Progress;

namespace LibFLExBridgeChorusPluginTests.Handling.Linguistics.TextCorpus
{
	[TestFixture]
	public class FieldWorksTextCorpusTypeHandlerTests : BaseFieldWorksTypeHandlerTests
	{
		private TempFile _ourFile;
		private TempFile _theirFile;
		private TempFile _commonFile;

		[SetUp]
		public override void TestSetup()
		{
			base.TestSetup();
			FieldWorksTestServices.SetupTempFilesWithExtension("." + FlexBridgeConstants.TextInCorpus, out _ourFile, out _commonFile, out _theirFile);
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
		public void ExtensionOfKnownFileTypesShouldBeReversal()
		{
			var extensions = FileHandler.GetExtensionsOfKnownTextFileTypes().ToArray();
			Assert.AreEqual(FieldWorksTestServices.ExpectedExtensionCount, extensions.Count(), "Wrong number of extensions.");
			Assert.IsTrue(extensions.Contains(FlexBridgeConstants.TextInCorpus));
		}

		[Test]
		public void ShouldNotBeAbleToValidateIncorrectFormatFile()
		{
			using (var tempModelVersionFile = new TempFile("<classdata />"))
			{
				var newpath = Path.ChangeExtension(tempModelVersionFile.Path, FlexBridgeConstants.TextInCorpus);
				File.Copy(tempModelVersionFile.Path, newpath, true);
				Assert.IsTrue(FileHandler.CanValidateFile(newpath));
				File.Delete(newpath);
			}
		}

		[Test]
		public void ShouldBeAbleToValidateInProperlyFormattedFile()
		{
			const string data =
@"<TextInCorpus>
<Text guid='0bd1fdbc-bedf-43d1-8d6a-c1766b556028' >
</Text>
</TextInCorpus>";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsTrue(FileHandler.CanValidateFile(_ourFile.Path));
		}

		[Test]
		public void ShouldBeAbleToDoAllCanOperations()
		{
			const string data =
@"<TextInCorpus>
<Text guid='0bd1fdbc-bedf-43d1-8d6a-c1766b556028' >
</Text>
</TextInCorpus>";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsTrue(FileHandler.CanValidateFile(_ourFile.Path));
			Assert.IsTrue(FileHandler.CanDiffFile(_ourFile.Path));
			Assert.IsTrue(FileHandler.CanMergeFile(_ourFile.Path));
			Assert.IsTrue(FileHandler.CanPresentFile(_ourFile.Path));
		}

		[Test]
		public void ShouldNotBeAbleToValidateFile()
		{
			const string data = "<classdata />";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsNotNull(FileHandler.ValidateFile(_ourFile.Path, new NullProgress()));
		}

		[Test]
		public void ShouldBeAbleToValidateFile()
		{
			const string data =
@"<TextInCorpus>
<Text guid='0bd1fdbc-bedf-43d1-8d6a-c1766b556028' >
</Text>
</TextInCorpus>";
			File.WriteAllText(_ourFile.Path, data);
			Assert.IsNull(FileHandler.ValidateFile(_ourFile.Path, new NullProgress()));
		}

		[Test]
		public void MergeStTxtParaNoChanges()
		{
			string commonAncestor =
@"<?xml version='1.0' encoding='utf-8'?>
<Text guid='4836797B-5ADE-4C1C-94F7-8C1104236A94'>
	<StText guid='4D86FB53-CB4E-44D9-9FBD-AC7E1CBEA766'>
		<Paragraphs>
			<ownseq class='StTxtPara' guid='9edbb6e1-2bdd-481c-b84d-26c69f22856c'>
				<Contents>
					<Str>
						<Run ws='en'>This is the first paragraph.</Run>
					</Str>
				</Contents>
				<ParseIsCurrent val='true'/>
			</ownseq>
		</Paragraphs>
	</StText>
</Text>".Replace("'", "\"");


			FieldWorksTestServices.DoMerge(
				FileHandler,
				_ourFile, commonAncestor,
				_commonFile, commonAncestor,
				_theirFile, commonAncestor,
				new [] {"Text/StText/Paragraphs/ownseq/ParseIsCurrent[@val='true']"}, null,
				0, new List<Type>(),
				0, new List<Type>());
		}

		[Test]
		public void MergeStTxtParaTheyChangedText_SetsParseIsCurrentFalse()
		{
			string pattern =
@"<?xml version='1.0' encoding='utf-8'?>
<Text guid='4836797B-5ADE-4C1C-94F7-8C1104236A94'>
	<StText guid='4D86FB53-CB4E-44D9-9FBD-AC7E1CBEA766'>
		<Paragraphs>
			<ownseq class='StTxtPara' guid='9edbb6e1-2bdd-481c-b84d-26c69f22856c'>
				<Contents>
					<Str>
						<Run ws='en'>This is the first paragraph.{0}</Run>
					</Str>
				</Contents>
				<ParseIsCurrent val='True'/>
			</ownseq>
		</Paragraphs>
	</StText>
</Text>".Replace("'", "\"");
			string commonAncestor = string.Format(pattern, "");
			string theirs = commonAncestor;
			string ours = string.Format(pattern, "x");


			FieldWorksTestServices.DoMerge(
				FileHandler,
				_ourFile, ours,
				_commonFile, commonAncestor,
				_theirFile, theirs,
				new [] {"Text/StText/Paragraphs/ownseq/ParseIsCurrent[@val='False']"}, null,
				0, new List<Type>(),
				1, new List<Type>() { typeof(XmlChangedRecordReport) });
		}

		[Test]
		public void MergeStTxtParaWeChangedText_SetsParseIsCurrentFalse()
		{
			string pattern =
@"<?xml version='1.0' encoding='utf-8'?>
<Text guid='4836797B-5ADE-4C1C-94F7-8C1104236A94'>
	<StText guid='4D86FB53-CB4E-44D9-9FBD-AC7E1CBEA766'>
		<Paragraphs>
			<ownseq class='StTxtPara' guid='9edbb6e1-2bdd-481c-b84d-26c69f22856c'>
				<Contents>
					<Str>
						<Run ws='en'>This is the first paragraph.{0}</Run>
					</Str>
				</Contents>
				<ParseIsCurrent val='True'/>
			</ownseq>
		</Paragraphs>
	</StText>
</Text>".Replace("'", "\"");
			string commonAncestor = string.Format(pattern, "");
			string ours = commonAncestor;
			string theirs = string.Format(pattern, "x");


			FieldWorksTestServices.DoMerge(
				FileHandler,
				_ourFile, ours,
				_commonFile, commonAncestor,
				_theirFile, theirs,
				new[] { "Text/StText/Paragraphs/ownseq/ParseIsCurrent[@val='False']" }, null,
				0, new List<Type>(),
				1, new List<Type>() { typeof(XmlChangedRecordReport) });
		}
	}
}