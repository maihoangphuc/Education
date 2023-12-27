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

        [HttpGet]
        public async Task<ActionResult> Index(int? page, int? record, int? sequenceStatus, string? searchText, int? schoolId)
        {
            var listNews = await GetAllNews(page, record, sequenceStatus, searchText, schoolId);
            if (listNews != null)
                return View(listNews);
            return View(null);
        }

        [HttpGet]
        public async Task<IPagedList<NewsItemModel>> GetAllNews(int? page, int? record, int? sequenceStatus, string? searchText, int? schoolId)
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

            try
            {
                var newsEndpoint = $"/News/GetListByPaging?sequenceStatus={status}&record={pageSize}&page={pageNumber}&searchText={search}&schoolId={school}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsEndpoint}";

                var newsListModel = await _apiService.GetAsync<NewsModel<List<NewsItemModel>>>(fullNewsApiUrl);

                var pagedList = newsListModel.Data.ToPagedList(pageNumber, pageSize);
                return pagedList;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id < 1)
            {
                id = 1;
            }

            try
            {
                var newsEndpoint = $"/News/GetById?id={id}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsEndpoint}";

                var newsListModel = await _apiService.GetAsync<NewsModel<NewsItemModel>>(fullNewsApiUrl);

                return View(newsListModel.Data);

            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }
    }
}
