using global::Education.Models;
using global::Education.Services.Api;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Education.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public async Task<ActionResult> Add()
        {
            try
            {
                var schoolList = await GetAllSchool();
                var newsCategoryList = await GetAllNewsCategory();

                if (schoolList != null && newsCategoryList != null)
                {
                    ViewBag.SchoolList = schoolList;
                    ViewBag.NewsCategoryList = newsCategoryList;

                    return View();
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Add action: {Message}", ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<List<SchoolItemModel>> GetAllSchool()
        {
            int status = 1;

            try
            {
                var schoolEndpoint = $"/School/GetListByPaging?SequenceStatus={status}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{schoolEndpoint}";

                var schoolList = await _apiService.GetAsync<List<SchoolItemModel>>(fullNewsApiUrl);
                return schoolList;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpGet]
        public async Task<List<NewsItemCategoryModel>> GetAllNewsCategory()
        {
            int status = 1;

            try
            {
                var newsCategoryEndpoint = $"/NewsCategory/GetListByPaging?SequenceStatus={status}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsCategoryEndpoint}";

                var newsCategoryList = await _apiService.GetAsync<List<NewsItemCategoryModel>>(fullNewsApiUrl);
                return newsCategoryList;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task PostNews(NewsItemModel model)
        {

            var newsEndpoint = "News/Create";
            var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsEndpoint}";

            // Convert the news object to key-value pairs
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("NewsCategoryId", model.NewsCategoryId.ToString()),
                new KeyValuePair<string, string>("SchoolId", model.SchoolId.ToString()),
                new KeyValuePair<string, string>("Name", model.Name.ToString()),
                new KeyValuePair<string, string>("Description", model.Description),
                new KeyValuePair<string, string>("Status", model.Status),
                new KeyValuePair<string, string>("IsHot", model.IsHot.ToString()),
                new KeyValuePair<string, string>("MetaUrl", model.MetaUrl),
                new KeyValuePair<string, string>("PublishedAt", model.PublishedAt.ToString("o")),
            };

            await _apiService.PostAsync(fullNewsApiUrl, parameters);
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}




