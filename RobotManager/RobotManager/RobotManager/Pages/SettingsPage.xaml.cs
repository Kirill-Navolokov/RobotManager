﻿using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using RobotManager.Core.DI;
using RobotManager.Core.ViewModels;
using RobotManager.Interfaces;

namespace RobotManager.Pages
{
	[MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, WrapInNavigationPage = true, NoHistory = true)]
	public partial class SettingsPage : MvxContentPage<SettingsViewModel>
	{
		private readonly IMenuService _menuService;

		public SettingsPage()
		{
			_menuService = DiResolver.Instance.Resolve<IMenuService>();

			InitializeComponent();
		}

		protected override void OnDisappearing()
		{
			_menuService.CloseMenu();

			base.OnDisappearing();
		}
	}
}