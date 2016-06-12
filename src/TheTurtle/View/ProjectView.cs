// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System.Windows.Forms;

namespace TheTurtle.View
{
	internal sealed partial class ProjectView : UserControl, IProjectView
	{
		internal ProjectView()
		{
			InitializeComponent();
		}

		#region Implementation of IProjectView

		public IExistingSystemView ExistingSystemView
		{
			get { return _existingSystemView; }
		}

		#endregion
	}
}
