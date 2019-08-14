using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.ViewModels;
using Five_Seconds.Views;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Mocks;

namespace Five_Seconds.Core.Tests.ViewModels
{
    public class MissionsViewModelTest
    {
        private MissionsViewModel missionsViewModel;
        private Mock<INavigation> navigationService;
        private IDependencyService dependencyService;

        [SetUp]
        public void Setup()
        {
            MockForms.Init();

            //DependencyService.Register<IDatabase>(new ItemDatabaseGeneric());
            navigationService = new Mock<INavigation>();
            var ds = new DependencyServiceForUT();

            ds.Register<IDatabase>();

            missionsViewModel = new MissionsViewModel(navigationService.Object);
        }

        [Test]
        public void AddMissionCommand()
        {
            // Arrange
            // Act
            missionsViewModel.AddMissionCommand.Execute(null);
            // Assert
            navigationService.Verify(n => n.PushAsync(It.IsAny<MissionPopupPage>()));
        }
    }
}
