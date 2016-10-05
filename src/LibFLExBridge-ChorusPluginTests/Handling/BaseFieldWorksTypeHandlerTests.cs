// --------------------------------------------------------------------------------------------
// Copyright (C) 2010-2016 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------

using Chorus.FileTypeHandlers;
using LibFLExBridgeChorusPlugin.Handling;
using LibFLExBridgeChorusPlugin.Infrastructure;
using NUnit.Framework;

namespace LibFLExBridgeChorusPluginTests.Handling
{
	public abstract class BaseFieldWorksTypeHandlerTests
	{
		protected IChorusFileTypeHandler FileHandler;
		internal MetadataCache Mdc;

		[SetUp]
		public virtual void TestSetup()
		{
			FileHandler = new FieldWorksCommonFileHandler();
			Mdc = MetadataCache.TestOnlyNewCache;
			Mdc.UpgradeToVersion(7000065); // 66 and higher require all basic data types to be in test code.
		}

		[TearDown]
		public virtual void TestTearDown()
		{
			FileHandler = null;
			Mdc = null;
		}
	}
}