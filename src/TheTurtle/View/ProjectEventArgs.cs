// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System;
using TheTurtle.Model;

namespace TheTurtle.View
{
	internal sealed class ProjectEventArgs : EventArgs
	{
		internal LanguageProject Project { get; private set; }

		internal ProjectEventArgs(LanguageProject project)
		{
			Project = project;
		}
	}
}