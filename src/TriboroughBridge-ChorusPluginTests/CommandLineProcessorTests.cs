// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SIL.TestUtilities;
using TriboroughBridge_ChorusPlugin;

namespace TriboroughBridge_ChorusPluginTests
{
	[TestFixture]
	public class CommandLineProcessorTests
	{
		private Dictionary<string, string> _options;
		private TemporaryFolder _tempProjectFolder;
		private TemporaryFolder _tempFwAppsFolder;

		private void BasicWellformedOptionCheck(string option)
		{
			BasicWellFormedCheck(option);

			_options.Remove(option);
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "missing key should have thrown");
		}

		private void BasicWellFormedCheck(string option)
		{
			var originalValue = _options[option];
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "legal value should not throw");

			_options[option] = string.Empty;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options),
												"empty string should have thrown");

			_options[option] = null;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options),
												"null string should have thrown");
			// Restore original value.
			_options[option] = originalValue;
		}

		private void CheckWellformedWithFwDataFile(string optionToTest)
		{
			string fooDir;
			var fooFwdataPathname = CreateFooFwDataFolderAndFile(out fooDir);

			_options[CommandLineProcessor.p] = fooFwdataPathname;
			BasicWellFormedCheck(optionToTest);
		}

		private string CreateFooFwDataFolderAndFile(out string fooDir)
		{
			fooDir = Path.Combine(_tempProjectFolder.Path, "Foo");
			if (Directory.Exists(fooDir))
				Directory.Delete(fooDir);
			Directory.CreateDirectory(fooDir);
			var fooFwdataPathname = Path.Combine(fooDir, "Foo.fwdata");
			File.WriteAllText(fooFwdataPathname, "Some fake FW data");
			return fooFwdataPathname;
		}

		[SetUp]
		public void SetupTest()
		{
			_tempProjectFolder = new TemporaryFolder("Projects");
			_tempFwAppsFolder = new TemporaryFolder("FWAppsFolder");
			var fixitPathname = Path.Combine(_tempFwAppsFolder.Path, "FixFwData.exe");
			File.WriteAllText(fixitPathname, "stuff");
			_options = new Dictionary<string, string>
				{
					{CommandLineProcessor.u, "Randy"},
					{CommandLineProcessor.p, _tempProjectFolder.Path},
					{CommandLineProcessor.v, "obtain"},
					{CommandLineProcessor.f, fixitPathname},
					//{CommandLineProcessor.g, "projectGuid"}, // not present for most '-v' options, but required for 'move_lift'
					{CommandLineProcessor.projDir, _tempProjectFolder.Path},
					{CommandLineProcessor.fwAppsDir, _tempFwAppsFolder.Path},
					{CommandLineProcessor.locale, "en"},
					{CommandLineProcessor.fwmodel, "7000066"},
					{CommandLineProcessor.pipeID, "FW pipe id"}
				};
		}

		[TearDown]
		public void TeardownTest()
		{
			_tempProjectFolder.Dispose();
			_tempProjectFolder = null;
			_tempFwAppsFolder.Dispose();
			_tempFwAppsFolder = null;
			_options = null;
		}

		[Test]
		public void MalformedUOptionThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.u);
		}

		[Test]
		public void MalformedPOptionThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.p);
		}

		[Test]
		public void MalformedVOptionThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.v);

			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Unsupported 'v' option should have thrown");
		}

		[Test]
		public void MalformedProjDirOptionThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.projDir);
		}

		[Test]
		public void MalformedLocaleOptionThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.locale);
		}

		[Test]
		public void MalformedFwAppsDirThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.fwAppsDir);
		}

		[Test]
		public void MalformedFwmodelOptionThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.fwmodel);
		}

		[Test]
		public void MalformedPipeIDOptionThrows()
		{
			_options.Remove(CommandLineProcessor.f);
			BasicWellformedOptionCheck(CommandLineProcessor.pipeID);
		}

		[Test]
		public void about_flex_bridgeOption()
		{
			_options[CommandLineProcessor.v] = CommandLineProcessor.about_flex_bridge;
			_options.Remove(CommandLineProcessor.f);

			// Has no project folder
			_options[CommandLineProcessor.p] = Path.Combine(_tempProjectFolder.Path, "NonExistantFolder");
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Should not check project folder existance when showing About");
			_options[CommandLineProcessor.p] = _tempProjectFolder.Path;
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Should not check project folder validity when showing About");

			string fooDir;
			var fooFwdataPathname = CreateFooFwDataFolderAndFile(out fooDir);
			_options[CommandLineProcessor.p] = fooFwdataPathname;
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to not have accepted an fwdata file for '-p'");
		}

		[Test]
		public void check_for_updatesOption()
		{
			_options[CommandLineProcessor.v] = CommandLineProcessor.check_for_updates;
			_options.Remove(CommandLineProcessor.f);

			// Has no project folder
			_options[CommandLineProcessor.p] = Path.Combine(_tempProjectFolder.Path, "NonExistantFolder");
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Should not check project folder existance when checking for updates");
			_options[CommandLineProcessor.p] = _tempProjectFolder.Path;
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Should not check project folder validity when checking for updates");

			string fooDir;
			var fooFwdataPathname = CreateFooFwDataFolderAndFile(out fooDir);
			_options[CommandLineProcessor.p] = fooFwdataPathname;
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to not have accepted an fwdata file for '-p'");
		}

		[Test]
		public void obtainOption()
		{
			_options[CommandLineProcessor.v] = CommandLineProcessor.obtain;
			_options.Remove(CommandLineProcessor.f);

			// Has no project folder
			_options[CommandLineProcessor.p] = "NonExistantFolder";
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have an FW project file");

			// Has FW project folder, but fed improperly to 'obtain'.
			var fooDir = Path.Combine(_tempProjectFolder.Path, "Foo");
			Directory.CreateDirectory(fooDir);
			_options[CommandLineProcessor.p] = fooDir;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have accepted a folder for '-p'");

			var fooFwdataPathname = CreateFooFwDataFolderAndFile(out fooDir);
			_options[CommandLineProcessor.p] = fooFwdataPathname;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have accepted an fwdata file for '-p'");

			_options[CommandLineProcessor.p] = _tempProjectFolder.Path;
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to not accept the correct '-p' option");
		}

		[Test]
		public void send_receiveOption()
		{
			_options[CommandLineProcessor.v] = CommandLineProcessor.send_receive;

			// Has no project folder
			_options[CommandLineProcessor.p] = Path.Combine(_tempProjectFolder.Path, "NonExistantFolder");
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have an FW project file");
			_options[CommandLineProcessor.p] = _tempProjectFolder.Path;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have accepted the main FW project folder for '-p'");

			string fooDir;
			var fooFwdataPathname = CreateFooFwDataFolderAndFile(out fooDir);
			_options[CommandLineProcessor.p] = fooFwdataPathname;

			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to not have the fwdata file");

			File.Delete(fooFwdataPathname);
			_options[CommandLineProcessor.p] = fooFwdataPathname;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have an fwdata file");

			_options[CommandLineProcessor.p] = fooDir;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have an fwdata file, when only given the containing folder");
		}

		[Test]
		public void view_notesOption()
		{
			_options.Remove(CommandLineProcessor.f);
			_options[CommandLineProcessor.v] = CommandLineProcessor.view_notes;
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Should not have -g option");
			
			string fooDir;
			var fooFwdataPathname = CreateFooFwDataFolderAndFile(out fooDir);
			_options[CommandLineProcessor.p] = fooFwdataPathname;

			// Has no .hg folder
			Assert.Throws<CommandLineException>(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to have an '.hg' folder");

			Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(fooFwdataPathname), TriboroughBridgeUtilities.hg));
			Assert.DoesNotThrow(() => CommandLineProcessor.ValidateCommandLineArgs(_options), "Seems to not have the Lift '.hg' folder");
		}
	}
}