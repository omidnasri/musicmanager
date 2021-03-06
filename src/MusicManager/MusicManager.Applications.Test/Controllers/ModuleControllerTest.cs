﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.IO;
using Test.MusicManager.Applications.Services;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Applications.Views;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Applications.Controllers
{
    [TestClass]
    public class ModuleControllerTest : ApplicationsTest
    {
        private ModuleController controller;
        private IShellService shellService;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            controller = Container.GetExportedValue<ModuleController>();
            shellService = Container.GetExportedValue<IShellService>();
            controller.Initialize();
            controller.Run();
        }

        protected override void OnCleanup()
        {
            controller.Shutdown();
            base.OnCleanup();
        }
        

        [TestMethod]
        public void LaodAndSaveSettings()
        {
            var environmentService = Container.GetExportedValue<MockEnvironmentService>();
            environmentService.AppSettingsPath = Environment.CurrentDirectory;

            controller.Initialize();
            controller.Run();

            var playerController = Container.GetExportedValue<PlayerController>();
            shellService.Settings.Height = 42;
            playerController.PlaylistManager.CurrentItem = new PlaylistItem(MockMusicFile.CreateEmpty("Test"));

            controller.Shutdown();

            shellService.Settings.Height = 0;
            playerController.PlaylistSettings.LastPlayedFileName = null;

            controller.Initialize();
            controller.Run();

            Assert.AreEqual(42, shellService.Settings.Height);
            Assert.AreEqual("Test", playerController.PlaylistSettings.LastPlayedFileName);

            File.Delete(Path.Combine(environmentService.AppSettingsPath, "Settings.xml"));
            File.Delete(Path.Combine(environmentService.AppSettingsPath, "Playlist.xml"));
        }
        
        [TestMethod]
        public void ShowDetailViewsTest()
        {
            var view = Container.GetExportedValue<MockShellView>();
            Assert.IsTrue(view.IsVisible);
            
            var viewModel = Container.GetExportedValue<ShellViewModel>();
            Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
            Assert.IsTrue(viewModel.IsPlaylistViewVisible);
            Assert.IsFalse(viewModel.IsTranscodingListViewVisible);

            
            shellService.ShowMusicPropertiesView();
            Assert.IsTrue(viewModel.IsMusicPropertiesViewVisible);
            Assert.IsFalse(viewModel.IsPlaylistViewVisible);
            Assert.IsFalse(viewModel.IsTranscodingListViewVisible);

            shellService.ShowTranscodingListView();
            Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
            Assert.IsFalse(viewModel.IsPlaylistViewVisible);
            Assert.IsTrue(viewModel.IsTranscodingListViewVisible);

            shellService.ShowPlaylistView();
            Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
            Assert.IsTrue(viewModel.IsPlaylistViewVisible);
            Assert.IsFalse(viewModel.IsTranscodingListViewVisible);
        }
    }
}
