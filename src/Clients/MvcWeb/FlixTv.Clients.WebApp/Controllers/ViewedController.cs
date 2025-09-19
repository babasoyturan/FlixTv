using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.ViewModels;
using FlixTv.Common.Models.ResponseModels.ViewData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    [Authorize(Roles = "User, Admin, Moderator")]
    public class ViewedController : Controller
    {
        private readonly IViewDatasService _viewDatas;

        public ViewedController(IViewDatasService viewDatas) => _viewDatas = viewDatas;

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 2)
        {
            var vm = new ViewedViewModel
            {
                Page = Math.Max(1, page),
                PageSize = Math.Max(1, pageSize)
            };

            var countRes = await _viewDatas.GetMyViewDatasCountAsync();
            if (countRes.IsSuccess) vm.TotalCount = countRes.Data;

            if (vm.TotalCount > 0)
            {
                var listRes = await _viewDatas.GetMyViewDatasAsync(vm.Page, vm.PageSize, "createdDate");
                if (listRes.IsSuccess && listRes.Data != null)
                    vm.ViewDatas = listRes.Data;
            }

            return View(vm);
        }

        // AJAX Partial for pagination
        [HttpGet]
        public async Task<IActionResult> MyViewDatas(int page = 1, int pageSize = 10)
        {
            var countRes = await _viewDatas.GetMyViewDatasCountAsync();
            if (!countRes.IsSuccess) return StatusCode(500, "Could not load view history count.");

            var listRes = await _viewDatas.GetMyViewDatasAsync(page, pageSize, "createdDate");
            if (!listRes.IsSuccess) return StatusCode(500, "Could not load view history.");

            var vm = new MyViewDatasTablePartialViewModel
            {
                ViewDatas = listRes.Data ?? new List<GetViewDataQueryResponse>(),
                TotalCount = countRes.Data,
                Page = page,
                PageSize = pageSize
            };

            return PartialView("_MyViewDatasTable", vm);
        }
    }
}
