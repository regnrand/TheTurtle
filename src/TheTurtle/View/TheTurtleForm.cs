// Copyright (C) 2010-2016 Randy Regnier. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.

using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace TheTurtle.View
{
	[Export(typeof(TheTurtleForm))]
	public partial class TheTurtleForm : Form
	{
		public TheTurtleForm()
		{
			InitializeComponent();
		}

		internal ITurtleView TurtleView { get { return _turtleView; }}
	}
}
