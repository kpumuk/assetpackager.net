using System;
using System.Web.UI;

namespace DemoProject.Controls
{
	public partial class Contact : UserControl
	{
		protected override void OnLoad(EventArgs e)
		{
			rfvEmail.ValidationGroup = revEmail.ValidationGroup = btnSubmit.ValidationGroup = ID;
			if (IsSuccessLoaded) LoadSuccessControl();
			base.OnLoad(e);
		}

		private void LoadSuccessControl()
		{
			phContent.Controls.Clear();
			phContent.Controls.Add(LoadControl("~/Controls/Success.ascx"));
		}

		private bool IsSuccessLoaded
		{
			get { return (bool?) ViewState["IsSuccessLoaded"] ?? false; }
			set { ViewState["IsSuccessLoaded"] = value; }
		}

		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			if (!Page.IsValid) return;
			IsSuccessLoaded = true;
			LoadSuccessControl();
		}
	}
}