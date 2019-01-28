using RedSpartan.Mvvm.Services;
using System.Collections.Generic;
using Xunit;

namespace RedSpartan.Mvvm.Tests
{
    public class ViewModelViewMappingTests
    {
        [Fact]
        public void ViewMapping_AreEqualTest()
        {
            var model1 = new ViewMapping(typeof(int), typeof(string));
            var model2 = new ViewMapping(typeof(int), typeof(byte));
            Assert.Equal(model1, model2);
        }

        [Fact]
        public void ViewModelViewMappings_DuplicatesOverwriteTest()
        {
            var mapper = new ViewModelViewMappings();
            Assert.Equal(0, mapper.Count);

            var model1 = new ViewMapping(typeof(int), typeof(string));
            var model2 = new ViewMapping(typeof(int), typeof(byte));

            mapper.AddMapping(model1);
            Assert.True(mapper.ContainsKey(typeof(int)));
            Assert.Same(mapper.GetViewType(typeof(int)), typeof(string));

            mapper.AddMapping(model2);
            Assert.Equal(1, mapper.Count);
            Assert.Same(mapper.GetViewType(typeof(int)), typeof(byte));
        }

        [Fact]
        public void ViewModelViewMappings_ConstructorTest()
        {
            var model = new List<ViewMapping>
            {
                new ViewMapping(typeof(int), typeof(string), ViewType.Default),
                new ViewMapping(typeof(string), typeof(string), ViewType.Default),
                new ViewMapping(typeof(int), typeof(string), ViewType.Edit),
                new ViewMapping(typeof(string), typeof(string), ViewType.Edit),
                new ViewMapping(typeof(int), typeof(byte))
            };
            var mapper = new ViewModelViewMappings(model);

            Assert.Equal(4, mapper.Count);
            Assert.Same(mapper.GetViewType(typeof(int)), typeof(byte));
        }
    }
}
