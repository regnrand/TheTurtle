// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System.Collections.Generic;

namespace TheTurtle
{
	/// <summary>
	/// Interface that locates one, or more, base paths to FieldWorks projects.
	/// </summary>
	internal interface IProjectPathLocator
	{
		HashSet<string> BaseFolderPaths { get; }
	}
}