using Education.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;

namespace Education.Areas.Admin.Controllers
{
    public class AdminNewsController : Controller
    {
        private readonly ILogger<AdminNewsController> _logger;
        private readonly HttpClient _apiClient;
        private readonly string _baseUri = "https://api-intern-test.h2aits.com/";

        public AdminNewsController(ILogger<AdminNewsController> logger)
        {
            _logger = logger;
            _apiClient = new HttpClient();
        }

        public async Task<ActionResult> Index(int? page, int? record, int? sequenceStatus, string? searchText)
        {
            int pageNumber = page ?? 1;
            int pageSize = record ?? 5;
            int status = sequenceStatus ?? 1;
            string search = searchText ?? "";

            if (page < 1)
            {
                pageNumber = 1;
            }

            try
            {
                // Gọi API và xử lý kết quả
                string apiUrl = $"{_baseUri}News/GetListByPaging?sequenceStatus={status}&?record={pageSize}&?page={pageNumber}&searchText={search}";
                HttpResponseMessage response = await _apiClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    // Model
                    var newsListModel = JsonConvert.DeserializeObject<NewsListModel>(apiResponse);

                    // Xử lý kết quả API ở đây
                    var newsLists = newsListModel.Data;

                    return View(newsLists.ToList().ToPagedList(pageNumber, pageSize));
                }
            }
            catch (HttpRequestException e)
            {
                // Xử lý lỗi gọi API
                ViewBag.Error = "Error calling API: " + e.Message;
            }

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
    }
}






/*

public class NewsController : Controller
{
    private readonly ILogger<NewsController> _logger;
    private readonly HttpClient _apiClient;
    private readonly string _baseUri = "https://api-intern-test.h2aits.com/";

    public NewsController(ILogger<NewsController> logger)
    {
        _logger = logger;
        _apiClient = new HttpClient();
    }

    public async Task<ActionResult> Index(int? page, int? record, int? sequenceStatus, string? searchText)
    {
        int pageNumber = page ?? 1;
        int pageSize = record ?? 4;
        int status = sequenceStatus ?? 1;
        string search = searchText ?? "";

        if (page < 1)
        {
            pageNumber = 1;
        }

        try
        {
            // Gọi API và xử lý kết quả
            string apiUrl = $"{_baseUri}News/GetListByPaging?sequenceStatus={status}&?record={pageSize}&?page={pageNumber}&searchText={search}";
            HttpResponseMessage response = await _apiClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                // Model
                var newsListModel = JsonConvert.DeserializeObject<NewsListModel>(apiResponse);

                // Xử lý kết quả API ở đây
                var newsLists = newsListModel.Data;

                return View(newsLists.ToList().ToPagedList(pageNumber, pageSize));
            }
        }
        catch (HttpRequestException e)
        {
            // Xử lý lỗi gọi API
            ViewBag.Error = "Error calling API: " + e.Message;
        }

        return View();
    }
}
*/