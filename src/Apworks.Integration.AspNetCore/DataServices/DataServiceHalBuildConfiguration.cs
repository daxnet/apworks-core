using Apworks.Integration.AspNetCore.Hal;
using Hal.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace Apworks.Integration.AspNetCore.DataServices
{
    public class DataServiceHalBuildConfiguration : HalBuildConfiguration
    {
        public DataServiceHalBuildConfiguration()
        {
            // Registers HAL builder factory for the GET method
            this.RegisterHalBuilderFactory("*.Get(int, int)", context =>
                {
                    dynamic state = context.State;
                    var pageSize = (int)state.PageSize;
                    var pageNumber = (int)state.PageNumber;
                    var totalRecords = (long)state.TotalRecords;
                    var totalPages = (long)state.TotalPages;
                    return new ResourceBuilder()
                        .WithState(new { pageNumber, pageSize, totalRecords, totalPages })
                        .AddSelfLink().WithLinkItem(context.HttpContext.Request.GetDisplayUrl())
                        .AddEmbedded(context.ControllerAction.ControllerName.ToLower())
                        .Resource(new ResourceBuilder().WithState(context.State));
                }
            );
        }
    }
}
