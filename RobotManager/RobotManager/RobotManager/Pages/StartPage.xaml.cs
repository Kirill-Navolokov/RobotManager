﻿using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using RobotManager.Core.DI;
using RobotManager.Core.ViewModels;
using RobotManager.Interfaces;

namespace RobotManager.Pages
{
	[MvxMasterDetailPagePresentation(MasterDetailPosition.Detail, WrapInNavigationPage = true, NoHistory = true)]
	public partial class StartPage : MvxContentPage<StartViewModel>
	{
		private readonly IMenuService _menuService;

		public StartPage ()
		{
			_menuService = DiResolver.Instance.Resolve<IMenuService>();

			InitializeComponent ();
		}
		
		protected override void OnDisappearing()
		{
			_menuService.CloseMenu();

			base.OnDisappearing();
		}
	}
}