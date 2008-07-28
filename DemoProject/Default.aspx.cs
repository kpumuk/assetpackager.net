using System;
using System.Web.UI;

namespace DemoProject
{
	public partial class _Default : Page
	{
		protected override void OnLoad(EventArgs e)
		{
			if (IsSuccessLoaded) LoadSuccessControl();
			base.OnLoad(e);
		}

		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			if (!IsValid) return;
			IsSuccessLoaded = true;
			LoadSuccessControl();
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
	}
}
