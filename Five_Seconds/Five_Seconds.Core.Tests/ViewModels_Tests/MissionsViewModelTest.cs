using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.ViewModels;
using Five_Seconds.Views;
using Moq;
using NUnit.Framework;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Mocks;

namespace Five_Seconds.Core.Tests.ViewModels
{
    public class MissionsViewModelTest
    {
        private MissionsViewModel missionsViewModel;
        private Mock<INavigation> navigation;
        private Mock<ILocalData> localData;
        private Mock<IPopupNavigation> popupNavigation;
        private Mock<IMessageBoxService> messageBoxService;

        [SetUp]
        public void Setup()
        {
            MockForms.Init();

            navigation = new Mock<INavigation>();
            localData = new Mock<ILocalData>();
            messageBoxService = new Mock<IMessageBoxService>();
            popupNavigation = new Mock<IPopupNavigation>();

            missionsViewModel = new MissionsViewModel(navigation.Object, localData.Object, messageBoxService.Object, popupNavigation.Object);
        }

        [Test]
        public void AddMissionCommand()
        {
            // Arrange
            // Act
            missionsViewModel.ShowAddMissionCommand.Execute(null);
            // Assert
            popupNavigation.Verify(n => n.PushAsync(It.IsAny<MissionPopupPage>(), true), Times.Once());
        }

        [Test]
        public void ShowMenuCommand()
        {
            // Arrange
            string[] actionSheetBtns = { "Modify", "Record", "Delete" };
            // Act
            missionsViewModel.ShowMenuCommand.Execute(null);
            // Assert
            messageBoxService.Verify(m => m.ShowActionSheet("Options", "Cancel", null, actionSheetBtns), Times.Once());
        }

        [Test]
        public async Task ClickMenuAction_Modify()
        {
            // Arrange
            var mission = new Mission();
            MethodInfo methodInfo = typeof(MissionsViewModel).GetMethod("ClickMenuAction", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { "Modify", mission };

            // Act //
            var methodAsync = (Task)methodInfo.Invoke(missionsViewModel, parameters);
            await methodAsync;

            // Assert
            popupNavigation.Verify(n => n.PushAsync(It.IsAny<MissionPopupPage>(), true), Times.Once());
        }

        [Test]
        public async Task ClickMenuAction_Record()
        {
            // Arrange
            var mission = new Mission();
            MethodInfo methodInfo = typeof(MissionsViewModel).GetMethod("ClickMenuAction", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { "Record", mission };

            // Act //
            var methodAsync = (Task)methodInfo.Invoke(missionsViewModel, parameters);
            await methodAsync;

            // Assert
            navigation.Verify(n => n.PushAsync(It.IsAny<RecordPage>()), Times.Once());
        }

        [Test]
        public async Task ShowMenuCommand_Delete()
        {
            // Arrange
            var mission = new Mission();
            MethodInfo methodInfo = typeof(MissionsViewModel).GetMethod("ClickMenuAction", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { "Delete", mission };

            // Act //
            var methodAsync = (Task)methodInfo.Invoke(missionsViewModel, parameters);
            await methodAsync;

            // Assert
            localData.Verify(l => l.DeleteMission(It.IsAny<int>()));
        }
    }
}
