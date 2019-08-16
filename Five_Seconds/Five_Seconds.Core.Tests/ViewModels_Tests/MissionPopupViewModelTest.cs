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
    public class MissionPopupViewModelTest
    {
        private MissionPopupViewModel missionPopupViewModel;
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

            missionPopupViewModel = new MissionPopupViewModel(navigation.Object, localData.Object, popupNavigation.Object);
        }

        [Test]
        public void CloseCommand()
        {
            // Arrange
            // Act
            missionPopupViewModel.CloseCommand.Execute(null);
            // Assert
            popupNavigation.Verify(n => n.PopAsync(true), Times.Once());
        }

        [Test]
        public void SaveCommand()
        {
            // Arrange
            // Act
            missionPopupViewModel.SaveCommand.Execute(null);
            // Assert
            localData.Verify(l => l.SaveMission(It.IsAny<Mission>()));
            popupNavigation.Verify(n => n.PopAsync(true), Times.Once());
        }
    }
}
