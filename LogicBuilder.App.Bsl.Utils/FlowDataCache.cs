using LogicBuilder.App.Bsl.Business.Requests;
using LogicBuilder.App.Bsl.Business.Responses;
using LogicBuilder.App.Bsl.Utils.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LogicBuilder.App.Bsl.Utils
{
    [ExcludeFromCodeCoverage]
    public class FlowDataCache : IFlowDataCache
    {
        public IBaseRequest? Request { get; set; }
        public BaseResponse? Response { get; set; }
        public Dictionary<string, object> Items { get; set; } = [];
    }
}
