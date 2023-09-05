using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using SportsStore.Controllers;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Tests
{
    public class PageLinkTagHelperTests
    {
        [Fact]
        public void Can_Generate_Page_Link()
        {
            //Организация
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.SetupSequence(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("Test/Page1")
                .Returns("Test/Page2")
                .Returns("Test/Page3");

            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory.Setup(f =>
                f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper.Object);

            PageLinkTagHelper helper = new PageLinkTagHelper(urlHelperFactory.Object)
            {
                PageModel = new PagingInfo
                {
                    CurrentPage = 2,
                    TotalItems = 28,
                    ItemsPerPage = 10,
                },
                PageAction = "Test"
            };

            TagHelperContext ctx = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(), "");

            var content = new Mock<TagHelperContent>();
            TagHelperOutput output = new TagHelperOutput("div",
                new TagHelperAttributeList(),
                (cache, encoder) => Task.FromResult(content.Object));

            //Действие
            helper.Process(ctx, output);

            //Утверждение
            Assert.Equal(@"<a href=""Test/Page1"">1</a>"
                + @"<a href=""Test/Page2"">2</a>"
                + @"<a href=""Test/Page3"">3</a>",
                output.Content.GetContent());
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            //Организаци
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"},
                new Product { ProductID = 3, Name = "P3"},
                new Product { ProductID = 4, Name = "P4"},
                new Product { ProductID = 5, Name = "P5"},
            }).AsQueryable<Product>());
            //Организация
            ProductController controller = new ProductController(mock.Object) { PageSize = 3 };

            //Действие
            ProductsListViewModel result = controller.List(null, 2).ViewData.Model as ProductsListViewModel;
            //Утверждение
            PagingInfo pageinfo = result.PagingInfo;
            Assert.Equal(2, pageinfo.CurrentPage);
            Assert.Equal(3, pageinfo.ItemsPerPage);
            Assert.Equal(5, pageinfo.TotalItems);
            Assert.Equal(2, pageinfo.TotalPages);
        }

    }
}
