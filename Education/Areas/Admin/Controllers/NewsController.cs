using Education.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Globalization;
using X.PagedList;

namespace Education.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        private readonly HttpClient _httpClient;
        Uri _baseUri = new Uri("https://api-intern-test.h2aits.com/");
        private readonly ApiConfig _apiConfig;

        public NewsController(ILogger<NewsController> logger, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            /*            _httpClient.BaseAddress = _baseUri;*/
        }

        public async Task<ActionResult> Index(int? page, int? record, int? sequenceStatus, string? searchText, int? schoolId)
        {
            int pageNumber = page ?? 1;
            int pageSize = record ?? 6;
            int status = sequenceStatus ?? 1;
            string search = searchText ?? "";
            int school = schoolId ?? 9;

            if (page < 1)
            {
                pageNumber = 1;
            }

            try
            {
                // Gọi API và xử lý kết quả
                string apiUrl = $"{_baseUri}News/GetListByPaging?sequenceStatus={status}&?record={pageSize}&?page={pageNumber}&searchText={search}&schoolId={school}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    // Model
                    var newsListModel = JsonConvert.DeserializeObject<NewsModel<List<NewsItemModel>>>(apiResponse);

                    // Xử lý kết quả API ở đây
                    var newsLists = newsListModel?.Data;

                    return View(newsLists?.ToList().ToPagedList(pageNumber, pageSize));
                }
            }
            catch (HttpRequestException e)
            {
                // Xử lý lỗi gọi API
                ViewBag.Error = "Error calling API: " + e.Message;
            }

            return View();
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
                string apiUrl = $"{_baseUri}School/GetListByPaging?SequenceStatus={status}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsAsync<SchoolModel<List<SchoolItemModel>>>();
                var schoolList = res?.Data;
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
                string apiUrl = $"{_baseUri}NewsCategory/GetListByPaging?SequenceStatus={status}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsAsync<NewsCategoryModel<List<NewsCategoryItemModel>>>();
                var newsCategoryList = res?.Data;
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
        public async Task<IActionResult> AddNews(NewsItemModel model)
        {
            try
            {
                if (ModelState.IsValid)
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
                    string apiUrl = $"{_baseUri}News/Create";

                    var httpRequestMessage = new HttpRequestMessage();
                    httpRequestMessage.Method = HttpMethod.Post;
                    httpRequestMessage.RequestUri = new Uri(apiUrl);

                    var content = new FormUrlEncodedContent(data);
                    httpRequestMessage.Content = content;

                    // Thực hiện Post
                    var response = await _httpClient.SendAsync(httpRequestMessage);
                    await response.Content.ReadAsAsync<NewsModel<List<NewsItemModel>>>();

                    return RedirectToAction("index", "news", new { area = "admin" });
                }
                else
                {
                    return RedirectToAction("add", "news", new { area = "admin" });
                }

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
                string apiUrl = $"{_baseUri}NewsCategory/GetById?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsAsync<NewsCategoryModel<NewsCategoryItemModel>>();
                var newsCategory = res?.Data;
                return newsCategory;

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
                string apiUrl = $"{_baseUri}School/GetById?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsAsync<SchoolModel<SchoolItemModel>>();
                var school = res?.Data;
                return school;
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
                string apiUrl = $"{_baseUri}News/GetById?id={id}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsAsync<NewsModel<NewsItemModel>>();
                var news = res?.Data;
                return news;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error calling API: {Message}", e.Message);
                throw new Exception("Error calling API: " + e.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(NewsItemModel model)
        {
            try
            {
                var content = new MultipartFormDataContent();

                foreach (var pr in typeof(NewsItemModel).GetProperties())
                {
                    var value = pr.GetValue(model)?.ToString() ?? "";
                    if (pr.Name == "PublishedAt")
                    {
                        value = DateTime.Parse(value).ToString("yyyy/MM/dd");
                    }
                    content.Add(new StringContent(value), pr.Name);

                }

                string apiUrl = "News/Update";

                // Thực hiện Post
                var response = await _httpClient.PutAsync(_httpClient.BaseAddress + apiUrl, content);
                var values = response.Content.ReadAsStringAsync();

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
                string apiUrl = $"https://api-intern-test.h2aits.com/News/Delete?id={id}";

                // Thực hiện Delete
                await _httpClient.DeleteAsync(apiUrl);
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
