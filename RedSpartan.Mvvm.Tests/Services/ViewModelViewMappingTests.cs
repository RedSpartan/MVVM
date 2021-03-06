using RedSpartan.Mvvm.Services;
using RedSpartan.Mvvm.Tests.ViewModels;
using RedSpartan.Mvvm.Tests.Views;
using System;
using System.Collections.Generic;
using Xunit;

namespace RedSpartan.Mvvm.Tests
{
    public class ViewModelViewMappingTests
    {
        [Fact]
        public void ViewMapping_AreEqualTest()
        {
            var model1 = new ViewMapping<TestViewModel1, TestPage1>();
            var model2 = new ViewMapping<TestViewModel1, TestPage1>();
            Assert.Equal(model1, model2);
        }

        [Fact]
        public void ViewModelViewMappings_DuplicatesExceptionTest()
        {
            var mapper = new ViewModelViewMappings(new TestInitiliser());
            Assert.Equal(0, mapper.Count);

            var model1 = new ViewMapping<TestViewModel1, TestPage1>();
            var model2 = new ViewMapping<TestViewModel1, TestPage2>();

            mapper.AddMapping(model1);
            Assert.True(mapper.ContainsKey<TestViewModel1>());
            Assert.Same(mapper.GetViewType<TestViewModel1>(), typeof(TestPage1));

            Assert.Throws<InvalidOperationException>(() => { mapper.AddMapping(model2); });
        }

        [Fact]
        public void ViewModelViewMappings_ConstructorTest()
        {
            var model = new List<ViewMapping>
            {
                new ViewMapping<TestViewModel1, TestPage1>(ViewType.Display),
                new ViewMapping<TestViewModel2, TestPage2>(ViewType.Display),
                new ViewMapping<TestViewModel1, TestPage1>(ViewType.Edit),
                new ViewMapping<TestViewModel2, TestPage1>(ViewType.Edit)
            };
            var mapper = new ViewModelViewMappings(new TestInitiliser());
            mapper.AddMapping(model);

            Assert.Equal(4, mapper.Count);
            Assert.Same(mapper.GetViewType<TestViewModel1>(), typeof(TestPage1));
        }
    }
}
