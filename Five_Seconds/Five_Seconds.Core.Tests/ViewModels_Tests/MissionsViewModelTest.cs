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
    public class AlarmsViewModelTest
    {
        private AlarmsViewModel alarmsViewModel;
        private Mock<INavigation> navigation;
        private Mock<IAlarmsRepository> localData;
        private Mock<IPopupNavigation> popupNavigation;
        private Mock<IMessageBoxService> messageBoxService;

        [SetUp]
        public void Setup()
        {
            MockForms.Init();

            navigation = new Mock<INavigation>();
            localData = new Mock<IAlarmsRepository>();
            messageBoxService = new Mock<IMessageBoxService>();
            popupNavigation = new Mock<IPopupNavigation>();

            alarmsViewModel = new AlarmsViewModel(navigation.Object, messageBoxService.Object);
        }

        [Test]
        public void ShowAddAlarmCommand()
        {
            // Arrange
            // Act
            alarmsViewModel.ShowAddAlarmCommand.Execute(null);
            // Assert
            navigation.Verify(n => n.PushAsync(It.IsAny<AlarmPage>(), true), Times.Once());
        }

        [Test]
        public void ShowMenuCommand()
        {
            // Arrange
            string[] actionSheetBtns = { "Modify", "Record", "Delete" };
            // Act
            alarmsViewModel.ShowModifyAlarmCommand.Execute(null);
            // Assert
            messageBoxService.Verify(m => m.ShowActionSheet("Options", "Cancel", null, actionSheetBtns), Times.Once());
        }

        [Test]
        public async Task ClickMenuAction_Modify()
        {
            // Arrange
            var alarm = new Alarm();
            MethodInfo methodInfo = typeof(AlarmsViewModel).GetMethod("ClickMenuAction", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { "Modify", alarm };

            // Act //
            var methodAsync = (Task)methodInfo.Invoke(alarmsViewModel, parameters);
            await methodAsync;

            // Assert
            navigation.Verify(n => n.PushAsync(It.IsAny<AlarmPage>(), true), Times.Once());
        }

        [Test]
        public async Task ClickMenuAction_Record()
        {
            // Arrange
            var alarm = new Alarm();
            MethodInfo methodInfo = typeof(AlarmsViewModel).GetMethod("ClickMenuAction", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { "Record", alarm };

            // Act //
            var methodAsync = (Task)methodInfo.Invoke(alarmsViewModel, parameters);
            await methodAsync;

            // Assert
            navigation.Verify(n => n.PushAsync(It.IsAny<RecordPage>()), Times.Once());
        }

        [Test]
        public async Task ShowMenuCommand_Delete()
        {
            // Arrange
            var alarm = new Alarm();
            MethodInfo methodInfo = typeof(AlarmsViewModel).GetMethod("ClickMenuAction", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { "Delete", alarm };

            // Act //
            var methodAsync = (Task)methodInfo.Invoke(alarmsViewModel, parameters);
            await methodAsync;

            // Assert
            localData.Verify(l => l.DeleteAlarm(It.IsAny<int>()));
        }
    }
}
