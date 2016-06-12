﻿// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace TheTurtle.Model
{
	/// <summary>
	/// This implementation is suitable for FieldWorks end users,
	/// who may have projects in: HKLM.software.SIL.FieldWorks.7.0.ProjectsDir
	/// </summary>
	[Export(typeof(IProjectPathLocator))]
	internal sealed class RegularUserProjectPathLocator : IProjectPathLocator
	{
		#region Implementation of IProjectPathLocator

		public HashSet<string> BaseFolderPaths
		{
			get
			{
				return new HashSet<string>
				             	{
				             		TheTurtleUtilities.ProjectsPath
				             	};
			}
		}

		#endregion
	}
}