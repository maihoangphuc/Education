using Education.Models;
using Education.Services.Api;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Education.Controllers
{
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        private readonly ApiService _apiService;

        public NewsController(ILogger<NewsController> logger, ApiService apiService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? record, int? sequenceStatus, string? searchText, int? schoolId)
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = record ?? 6;
                int status = sequenceStatus ?? 1;
                int school = schoolId ?? 9;
                string search = searchText ?? "";

                if (page < 1)
                {
                    pageNumber = 1;
                }
                string apiUrl = $"{_apiService.DefautApiBaseUri}/News/GetListByPaging?sequenceStatus={status}&?record={pageSize}&?page={pageNumber}&searchText={search}&schoolId={school}";

                var response = await _apiService.GetAsync<NewsModel<List<NewsItemModel>>>(apiUrl);

                var data = response?.Data ?? new List<NewsItemModel>();
                var panigation = data.ToPagedList(pageNumber, pageSize);

                return View(panigation);
            }
            catch (HttpRequestException e)
            {
                // Xử lý lỗi gọi API
                ViewBag.Error = "Error calling API: " + e.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id < 1)
            {
                return RedirectToAction("index", "news");
            }

            try
            {
                string apiUrl = $"{_apiService.DefautApiBaseUri}/News/GetById?id={id}";
                var response = await _apiService.GetAsync<NewsModel<NewsItemModel>>(apiUrl);
                var data = response.Data;
                return View(data);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }
    }
}
