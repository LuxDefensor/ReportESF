/*
 * Created by SharpDevelop.
 * User: smke-ing3
 * Date: 09.03.2016
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReportESF
{
	/// <summary>
	/// Description of frmProgress.
	/// </summary>
	public partial class frmProgress : Form
	{
		public frmProgress()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
		}
		
		public void SetProgress(int progressValue)
		{
			if (progressValue < 0) {
				this.progressBar1.Value = 0;
			}	
			else if(progressValue > 100)
			{
				this.progressBar1.Value = 100;
			}
			else
			{
				this.progressBar1.Value = progressValue;
			}
			this.Refresh();
		}		
	}
}
