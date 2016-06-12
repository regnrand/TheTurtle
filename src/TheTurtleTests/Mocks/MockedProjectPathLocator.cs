// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System.Collections.Generic;
using TheTurtle;

namespace TheTurtleTests.Mocks
{
	internal class MockedProjectPathLocator : IProjectPathLocator
	{
		private readonly HashSet<string> _baseFolderPaths;

		internal MockedProjectPathLocator(HashSet<string> baseFolderPaths)
		{
			_baseFolderPaths = baseFolderPaths;
		}

		#region Implementation of IProjectPathLocator

		public HashSet<string> BaseFolderPaths
		{
			get { return new HashSet<string>(_baseFolderPaths); }
		}

		#endregion
	}
}