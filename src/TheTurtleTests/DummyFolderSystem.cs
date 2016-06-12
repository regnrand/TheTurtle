// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System;
using System.Collections.Generic;
using System.IO;
using TheTurtle;

namespace TheTurtleTests
{
	/// <summary>
	/// Class used to create/delete test folders and files.
	/// </summary>
	internal sealed class DummyFolderSystem : IDisposable
	{
		private readonly List<string> _dummyFolderPaths = new List<string>();

		internal DummyFolderSystem()
		{
			BaseFolderPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "Projects")).FullName;

			BaseFolderPaths = new HashSet<string> {BaseFolderPath};

			// Remote collaboration enabled project
			var projectPath = Directory.CreateDirectory(Path.Combine(BaseFolderPath, "ZPI")).FullName;
			Directory.CreateDirectory(Path.Combine(projectPath, TheTurtleUtilities.hg));
			_dummyFolderPaths.Add(projectPath);
			File.WriteAllText(Path.Combine(projectPath, "ZPI" + TheTurtleUtilities.FwXmlExtension), "");

			// Remote collaboration not enabled project
			projectPath = Directory.CreateDirectory(Path.Combine(BaseFolderPath, "NotEnabled")).FullName;
			_dummyFolderPaths.Add(projectPath);
			File.WriteAllText(Path.Combine(projectPath, "NotEnabled" + TheTurtleUtilities.FwXmlExtension), "");

			// Client-Server DB4o project
			projectPath = Directory.CreateDirectory(Path.Combine(BaseFolderPath, "DB4o")).FullName;
			_dummyFolderPaths.Add(projectPath);
			File.WriteAllText(Path.Combine(projectPath, "DB4o" + TheTurtleUtilities.FwDb4oExtension), "");

			// Random non-FW folder
			projectPath = Directory.CreateDirectory(Path.Combine(BaseFolderPath, "RandomFolder")).FullName;
			_dummyFolderPaths.Add(projectPath);
		}

		internal string BaseFolderPath { get; private set; }

		internal HashSet<string> BaseFolderPaths { get; private set; }

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Directory.Delete(BaseFolderPath, true);
		}

		#endregion
	}
}