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
    public class RecordViewModelTest
    {
        private RecordViewModel recordViewModel;
        private Mock<INavigation> navigation;
        private Mock<IMissionsRepository> localData;
        private Mock<IPopupNavigation> popupNavigation;
        private Mock<IMessageBoxService> messageBoxService;

        [SetUp]
        public void Setup()
        {
            MockForms.Init();

            navigation = new Mock<INavigation>();
            localData = new Mock<IMissionsRepository>();
            messageBoxService = new Mock<IMessageBoxService>();
            popupNavigation = new Mock<IPopupNavigation>();

            recordViewModel = new RecordViewModel(navigation.Object);
        }
    }
}
