// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

﻿using System;
﻿using System.IO;
using NUnit.Framework;
using SIL.IO;
﻿using SIL.Progress;
using LibFLExBridgeChorusPlugin;
using LibFLExBridgeChorusPlugin.Infrastructure;

namespace LibFLExBridgeChorusPluginTests.Infrastructure
{
	[TestFixture]
	public class FLExProjectUnifierTests
	{
		[Test]
		public void NullPathnameForRestoreShouldThrow()
		{
			Assert.Throws<ApplicationException>(() => FLExProjectUnifier.PutHumptyTogetherAgain(
				new NullProgress(), false, null));
		}

		[Test]
		public void EmptyPathnameForRestoreShouldThrow()
		{
			Assert.Throws<ApplicationException>(() => FLExProjectUnifier.PutHumptyTogetherAgain(
				new NullProgress(), false, ""));
		}

		[Test]
		public void NonExistingFileForRestoreShouldThrow()
		{
			Assert.Throws<ApplicationException>(() => FLExProjectUnifier.PutHumptyTogetherAgain(
				new NullProgress(), false, "Bogus" + FlexBridgeConstants.FwXmlExtension));
		}

		[Test]
		public void NonExistantPathForRestoreShouldThrow()
		{
			using (var tempFile = new TempFile())
			{
				var pathname = tempFile.Path;
				Assert.Throws<ApplicationException>(() => FLExProjectUnifier.PutHumptyTogetherAgain(
					new NullProgress(), false, Path.Combine(pathname, "Itaintthere")));
			}
		}
	}
}