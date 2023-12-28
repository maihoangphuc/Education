using Education.Models;
using Education.Services.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
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
        public async Task<IActionResult> Add()
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

                var res = await _apiService.GetAsync<SchoolModel<List<SchoolItemModel>>>(fullNewsApiUrl);
                var schoolList = res?.Data?.ToList();
                return schoolList;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpGet]
        public async Task<List<NewsCategoryItemModel>> GetAllNewsCategory()
        {
            int status = 1;

            try
            {
                var newsCategoryEndpoint = $"/NewsCategory/GetListByPaging?SequenceStatus={status}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsCategoryEndpoint}";

                var res = await _apiService.GetAsync<NewsCategoryModel<List<NewsCategoryItemModel>>>(fullNewsApiUrl);
                var newsCategoryList = res?.Data?.ToList();
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
        public async Task AddNews(NewsItemModel model)
        {
            try
            {
                var data = new List<KeyValuePair<string, string>>
                     {
                        new KeyValuePair<string, string>("NewsCategoryId", model.NewsCategoryId?.ToString()),
                        new KeyValuePair<string, string>("SchoolId", model.SchoolId?.ToString()),
                        new KeyValuePair<string, string>("Name", model.Name.ToString()),
                        new KeyValuePair<string, string>("Description", model.Description),
                        new KeyValuePair<string, string>("Status", model.Status.ToString()),
                        new KeyValuePair<string, string>("IsHot", model.IsHot.ToString()),
                        new KeyValuePair<string, string>("MetaUrl", model.MetaUrl.ToString()),
                        new KeyValuePair<string, string>("PublishedAt", model.PublishedAt.ToString("o")),
                     };

                var newsCategoryEndpoint = "/News/Create";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsCategoryEndpoint}";

                await _apiService.PostAsync(fullNewsApiUrl, data);

                RedirectToAction("index", "news", new { area = "admin" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var idNews = id ?? 0;

                var newsDetail = await GetDetailNews(idNews);

                if (newsDetail == null)
                {
                    return RedirectToAction("index", "news", new { area = "admin" });
                }

                var schoolList = await GetAllSchool();
                var newsCategoryList = await GetAllNewsCategory();
                var schoolDetail = await GetDetailSchool(newsDetail.SchoolId ?? 0);
                var newsCategoryDetail = await GetDetailNewsCategory(newsDetail.NewsCategoryId ?? 0);

                if (schoolList == null || newsCategoryList == null || newsCategoryDetail == null || schoolDetail == null)
                {
                    return RedirectToAction("index", "news", new { area = "admin" });
                }

                //news detail
                ViewBag.NewsDetail = newsDetail;

                //droplist
                ViewBag.SchoolList = new SelectList(schoolList, "Id", "Name", schoolDetail.Id);
                ViewBag.NewsCategoryList = new SelectList(newsCategoryList, "Id", "Name", newsCategoryDetail.Id);

                // Đặt giá trị mặc định cho thuộc tính mô hình
                var modelState = ModelState;
                modelState.Clear();
                modelState.SetModelValue("Id", new ValueProviderResult(newsDetail?.Id.ToString() ?? "", CultureInfo.CurrentCulture));
                modelState.SetModelValue("Name", new ValueProviderResult(newsDetail?.Name.ToString() ?? "", CultureInfo.CurrentCulture));
                modelState.SetModelValue("Description", new ValueProviderResult(newsDetail?.Description.ToString() ?? "", CultureInfo.CurrentCulture));
                modelState.SetModelValue("MetaUrl", new ValueProviderResult(newsDetail?.MetaUrl.ToString() ?? "", CultureInfo.CurrentCulture));
                modelState.SetModelValue("NewsCategoryId", new ValueProviderResult(newsCategoryDetail?.Id.ToString() ?? "", CultureInfo.CurrentCulture));
                modelState.SetModelValue("SchoolId", new ValueProviderResult(schoolDetail?.Id.ToString() ?? "", CultureInfo.CurrentCulture));

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Edit action: {Message}", ex.Message);
                return RedirectToAction("Index", "Home", new { area = "admin" });
            }
        }

        [HttpGet]
        public async Task<NewsCategoryItemModel> GetDetailNewsCategory(int id)
        {
            try
            {
                var newsEndpoint = $"/NewsCategory/GetById?id={id}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsEndpoint}";

                var res = await _apiService.GetAsync<NewsCategoryModel<NewsCategoryItemModel>>(fullNewsApiUrl);
                return res.Data ?? new NewsCategoryItemModel();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpGet]
        public async Task<SchoolItemModel> GetDetailSchool(int id)
        {
            try
            {
                var newsEndpoint = $"/School/GetById?id={id}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsEndpoint}";

                var res = await _apiService.GetAsync<SchoolModel<SchoolItemModel>>(fullNewsApiUrl);
                return res.Data ?? new SchoolItemModel();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpGet]
        public async Task<NewsItemModel> GetDetailNews(int id)
        {
            try
            {
                var newsEndpoint = $"/News/GetById?id={id}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsEndpoint}";

                var res = await _apiService.GetAsync<NewsModel<NewsItemModel>>(fullNewsApiUrl);
                return res.Data ?? new NewsItemModel();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(NewsItemModel model)
        {
            try
            {
                var _httpClient = new HttpClient();
                var content = new MultipartFormDataContent();

                foreach (var pr in typeof(NewsItemModel).GetProperties())
                {
                    var value = pr.GetValue(model)?.ToString() ?? "";

                    /*  if (pr.Name == "PublishedAt")
                      {
                          value = DateTime.Parse(value).ToString("MM/dd/yyyy");
                      }*/
                    content.Add(new StringContent(value), pr.Name);
                }

                var newsCategoryEndpoint = "/news/update";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsCategoryEndpoint}";

                var response = await _httpClient.PutAsync(fullNewsApiUrl, content);
                await response.Content.ReadAsStringAsync();
                return RedirectToAction("index", "news", new { area = "admin" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [HttpDelete]
        public async Task DeleteNews(int id)
        {
            try
            {
                var newsCategoryEndpoint = $"News/Delete?id={id}";
                var fullNewsApiUrl = $"{_apiService.DefautApiBaseUri}{newsCategoryEndpoint}";

                await _apiService.DeleteAsync(fullNewsApiUrl);
                RedirectToAction("index", "news", new { area = "admin" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
    }
}
