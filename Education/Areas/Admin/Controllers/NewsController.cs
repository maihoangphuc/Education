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
        private readonly ApiService _apiService;

        public NewsController(ILogger<NewsController> logger, ApiService apiService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

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
                return View();
            }
        }

        [HttpGet]
        public async Task<List<SchoolItemModel>> GetAllSchool()
        {
            int status = 1;

            try
            {
                string apiUrl = $"{_apiService.DefautApiBaseUri}/School/GetListByPaging?SequenceStatus={status}";
                var response = await _apiService.GetAsync<SchoolModel<List<SchoolItemModel>>>(apiUrl);
                var data = response.Data;

                return data ?? new List<SchoolItemModel>();
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
                string apiUrl = $"{_apiService.DefautApiBaseUri}/NewsCategory/GetListByPaging?SequenceStatus={status}";
                var response = await _apiService.GetAsync<NewsCategoryModel<List<NewsCategoryItemModel>>>(apiUrl);
                var data = response.Data;

                return data ?? new List<NewsCategoryItemModel>();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNews(NewsItemModel model)
        {
            try
            {
                string apiUrl = $"{_apiService.DefautApiBaseUri}/News/Create";
                var content = new MultipartFormDataContent();

                foreach (var pr in typeof(NewsItemModel).GetProperties())
                {
                    var value = pr.GetValue(model)?.ToString() ?? "";
                    if (pr.Name == "PublishedAt")
                    {
                        value = DateTime.Parse(value).ToString("o");
                    }
                    content.Add(new StringContent(value), pr.Name);
                }

                // Thực hiện Post
                await _apiService.PostAsync(apiUrl, content);
                return RedirectToAction("index", "news", new { area = "admin" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("index", "news", new { area = "admin" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var newsDetail = await GetDetailNews(id);

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

                // Droplist
                ViewBag.SchoolList = new SelectList(schoolList, "Id", "Name", schoolDetail.Id);
                ViewBag.NewsCategoryList = new SelectList(newsCategoryList, "Id", "Name", newsCategoryDetail.Id);

                var modelState = ModelState;

                // Xóa giá trị mặc định cho thuộc tính mô hình
                modelState.Clear();

                // Đặt giá trị mặc định cho thuộc tính mô hình
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
                string apiUrl = $"{_apiService.DefautApiBaseUri}/NewsCategory/GetById?id={id}";
                var response = await _apiService.GetAsync<NewsCategoryModel<NewsCategoryItemModel>>(apiUrl);

                var data = response.Data;
                return data ?? new NewsCategoryItemModel();
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
                string apiUrl = $"{_apiService.DefautApiBaseUri}/School/GetById?id={id}";
                var response = await _apiService.GetAsync<SchoolModel<SchoolItemModel>>(apiUrl);

                var data = response.Data;
                return data ?? new SchoolItemModel();
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
                string apiUrl = $"{_apiService.DefautApiBaseUri}/News/GetById?id={id}";
                var response = await _apiService.GetAsync<NewsModel<NewsItemModel>>(apiUrl);

                var data = response.Data;
                return data ?? new NewsItemModel();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditNews(NewsItemModel model)
        {
            try
            {
                string apiUrl = $"{_apiService.DefautApiBaseUri}/News/Update";
                var content = new MultipartFormDataContent();

                foreach (var pr in typeof(NewsItemModel).GetProperties())
                {
                    var value = pr.GetValue(model)?.ToString() ?? "";
                    if (pr.Name == "PublishedAt")
                    {
                        value = DateTime.Parse(value).ToString("o");
                    }
                    content.Add(new StringContent(value), pr.Name);
                }

                // Thực hiện Put
                await _apiService.PutAsync(apiUrl, content);
                return RedirectToAction("index", "news", new { area = "admin" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("index", "news", new { area = "admin" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                string apiUrl = $"{_apiService.DefautApiBaseUri}/News/Delete?id={id}";

                // Thực hiện Delete
                await _apiService.DeleteAsync(apiUrl);
                return RedirectToAction("index", "news", new { area = "admin" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("index", "news", new { area = "admin" });
            }
        }
    }
}
