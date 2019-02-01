using Moq;
using RedSpartan.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RedSpartan.Mvvm.Tests.Services
{
    public class PageFactoryTests
    {
        [Fact]
        public void CreateAndBindPage()
        {
            var mock = new Mock<IIoC>();
            mock.Setup(ioc => ioc.Build<TestPage1>()).Returns(new TestPage1());
        }
    }
}
