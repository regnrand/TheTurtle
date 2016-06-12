// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

﻿using System;
using Chorus.Utilities;
using LibFLExBridgeChorusPlugin;
using LibFLExBridgeChorusPlugin.Infrastructure;
using NUnit.Framework;
using SIL.IO;
using SIL.Progress;

namespace LibFLExBridgeChorusPluginTests.Infrastructure
{
	[TestFixture]
	public class FLExProjectSplitterTests
	{
		[Test]
		public void NullPathnameForBreakupShouldThrow()
		{
			Assert.Throws<ApplicationException>(() => FLExProjectSplitter.PushHumptyOffTheWall(
				new NullProgress(), false, null));
		}

		[Test]
		public void EmptyPathnameForBreakupShouldThrow()
		{
			Assert.Throws<ApplicationException>(() => FLExProjectSplitter.PushHumptyOffTheWall(
				new NullProgress(), false, ""));
		}

		[Test]
		public void NonExistingFileForBreakupShouldThrow()
		{
			Assert.Throws<ApplicationException>(() => FLExProjectSplitter.PushHumptyOffTheWall(
				new NullProgress(), false, "Bogus" + FlexBridgeConstants.FwXmlExtension));
		}

		[Test]
		public void NotFwDataFileForBreakupShouldThrow()
		{
			using (var tempFile = new TempFile(""))
			{
				var pathname = tempFile.Path;
				Assert.Throws<ApplicationException>(() => FLExProjectSplitter.PushHumptyOffTheWall(
					new NullProgress(), false, pathname));
			}
		}

		[Test]
		public void UserCancelledBreakupShouldThrow()
		{
			using (var tempFile = TempFile.WithFilename("foo" + FlexBridgeConstants.FwXmlExtension))
			{
				var progress = new NullProgress
				{
					CancelRequested = true
				};
				var pathname = tempFile.Path;
				Assert.Throws<UserCancelledException>(() => FLExProjectSplitter.PushHumptyOffTheWall(
					progress, false, pathname));
			}
		}
	}
}
