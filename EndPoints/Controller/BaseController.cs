using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;

namespace EndPoints.Controller
{
  
    public class BaseController : ControllerBase
    {
        protected List<LinkVM> Links(List<Tuple<string, string, string, object?>> linkDtos)
        {
            var linkDto = new List<LinkVM>();

            foreach (var item in linkDtos)
            {
                linkDto.Add(new LinkVM(Url.RouteUrl(item.Item1, item.Item4), item.Item2, item.Item3));
            }

            return linkDto;
        }

        protected List<Tuple<string, string, string, object?>> createTupleObject()
        {
            return new List<Tuple<string, string, string, object?>>();
        }
    }
}
