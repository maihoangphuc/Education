using Education.Models;
using Education.Services.Api;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Education.Controllers
{
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        private readonly IApiService _apiService;

        public NewsController(ILogger<NewsController> logger, IApiService apiService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task<IActionResult> Index(int? page, int? record, int? sequenceStatus, string? searchText)
        {
            int pageNumber = page ?? 1;
            int pageSize = record ?? 6;
            int status = sequenceStatus ?? 1;
            string search = searchText ?? "";

            ViewBag.PageSize = pageSize;

            if (page < 1)
            {
                pageNumber = 1;
            }

            try
            {
                var newsEndpoint = $"/News/GetListByPaging?sequenceStatus={status}&record={pageSize}&page={pageNumber}&searchText={search}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsEndpoint}";

                var newsListModel = await _apiService.GetAsync<NewsListModel>(fullNewsApiUrl);

                return View(newsListModel.Data.ToPagedList(pageNumber, pageSize));
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                ViewBag.Error = "Error calling API: " + e.Message;
            }

            return View();
        }
    }
}
