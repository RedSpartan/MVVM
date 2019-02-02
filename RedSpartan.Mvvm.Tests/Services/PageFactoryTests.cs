using Moq;
using RedSpartan.Mvvm.Core;
using RedSpartan.Mvvm.Services;
using RedSpartan.Mvvm.Tests.ViewModels;
using RedSpartan.Mvvm.Tests.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RedSpartan.Mvvm.Tests.Services
{
    public class PageFactoryTests
    {
        Mock<IIoC> _ioc;
        Mock<IViewModelViewMappings> _mappings;
        public PageFactoryTests()
        {
            _ioc = new Mock<IIoC>();
            _ioc.Setup(ioc => ioc.Build(typeof(TestPage1))).Returns(new TestPage1());
            _ioc.Setup(ioc => ioc.Build(typeof(TestViewModel1))).Returns(new TestViewModel1());
            _ioc.Setup(ioc => ioc.Build<TestPage1>()).Returns(new TestPage1());
            _ioc.Setup(ioc => ioc.Build<TestViewModel1>()).Returns(new TestViewModel1());
            _ioc.Setup(ioc => ioc.Build(typeof(TestPage2))).Returns(new TestPage2());
            _ioc.Setup(ioc => ioc.Build(typeof(TestViewModel2))).Returns(new TestViewModel2());
            _ioc.Setup(ioc => ioc.Build<TestPage2>()).Returns(new TestPage2());
            _ioc.Setup(ioc => ioc.Build<TestViewModel2>()).Returns(new TestViewModel2());

            _mappings = new Mock<IViewModelViewMappings>();
            _mappings.Setup(m => m.ContainsKey(typeof(TestViewModel1), ViewType.Display)).Returns(true);
            _mappings.Setup(m => m.GetViewType(typeof(TestViewModel1), ViewType.Display)).Returns(typeof(TestPage1));
        }

        [Fact]
        public void InitilisePage_SetsViewModelBindingAndPageParameter()
        {
            var factory = new PageFactory(_ioc.Object, _mappings.Object);
            var page = new TestPage1
            {
                BindingContext = new TestViewModel1()
            };

            Assert.Null(((TestViewModel1)page.BindingContext).Parameter);

            factory.InitilisePage(page, "Nexus").Wait();

            Assert.IsType<string>(((TestViewModel1)page.BindingContext).Parameter);
            Assert.Same(((TestViewModel1)page.BindingContext).Parameter, "Nexus");

            factory.InitilisePage(page, new TestPage2()).Wait();

            Assert.IsType<TestPage2>(((TestViewModel1)page.BindingContext).Parameter);
        }

        [Fact]
        public void CreateAndBindPage_ReturnsABoundPage()
        {
            var factory = new PageFactory(_ioc.Object, _mappings.Object);
            var page = factory.CreateAndBindPage(typeof(TestViewModel1), ViewType.Display);

            Assert.True(page.GetType() == typeof(TestPage1));
            Assert.IsType<TestViewModel1>(page.BindingContext);
        }

        [Fact]
        public void CreateAndBindPageT_ReturnsABoundPage()
        {
            var factory = new PageFactory(_ioc.Object, _mappings.Object);
            var page = factory.CreateAndBindPage<TestViewModel1>(ViewType.Display);

            Assert.True(page.GetType() == typeof(TestPage1));
            Assert.IsType<TestViewModel1>(page.BindingContext);
        }

        [Fact]
        public void CreateBindAndInitilisePageAsync_ReturnsABoundPage()
        {
            var factory = new PageFactory(_ioc.Object, _mappings.Object);

            var page = factory.CreateBindAndInitilisePageAsync<TestViewModel1>(ViewType.Display).Result;

            Assert.IsType<TestPage1>(page);
        }

        [Fact]
        public void CreateBindAndInitilisePageAsync_ReturnsAnInitilisedBoundPage()
        {
            var parameter = Guid.NewGuid();
            var factory = new PageFactory(_ioc.Object, _mappings.Object);

            var page = factory.CreateBindAndInitilisePageAsync<TestViewModel1>(parameter, ViewType.Display).Result;

            Assert.IsType<TestPage1>(page);
            Assert.Equal(parameter, ((TestViewModel1)page.BindingContext).Parameter);
        }

        [Fact]
        public void GetPageTypeForViewModel_ReturnsType()
        {
            var factory = new PageFactory(_ioc.Object, _mappings.Object);
            Assert.Same(typeof(TestPage1), factory.GetPageTypeForViewModel(typeof(TestViewModel1), ViewType.Display));
        }

        [Fact]
        public void GetPageTypeForViewModel_ThrowsException()
        {
            var factory = new PageFactory(_ioc.Object, _mappings.Object);
            Assert.Throws<KeyNotFoundException>(() => factory.GetPageTypeForViewModel(typeof(TestViewModelNoPage), ViewType.Display));
        }

        [Fact]
        public void GetPageTypeForViewModel_ReturnsTypeWithoutMapping()
        {
            var factory = new PageFactory(_ioc.Object, _mappings.Object);
            factory.GetPageTypeForViewModel(typeof(TestViewModel2), ViewType.Display);
        }
    }
}
