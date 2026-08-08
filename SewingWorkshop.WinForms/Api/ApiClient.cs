using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SewingWorkshop.WinForms.Api
{
    public sealed class ApiClient : IDisposable
    {
        private readonly HttpClient _httpClient = new();
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public ApiClient()
        {
            BaseUrl = "http://localhost:5217/api/";
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public string BaseUrl { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? AccessTokenExpiresAt { get; private set; }
        public string? Username { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);

        public void SetBaseUrl(string value)
        {
            var normalized = NormalizeBaseUrl(value);
            BaseUrl = normalized;
            _httpClient.BaseAddress = new Uri(normalized);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var response = await PostAsync<AuthResponse>("auth/login", request);
            ApplyAuth(response);
            return response;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var response = await PostAsync<AuthResponse>("auth/register", request);
            ApplyAuth(response);
            return response;
        }

        public async Task<AuthResponse> RefreshAsync()
        {
            if (string.IsNullOrWhiteSpace(RefreshToken))
            {
                throw new ApiException(HttpStatusCode.Unauthorized, "Refresh token отсутствует.");
            }

            var response = await PostAsync<AuthResponse>("auth/refresh", new RefreshTokenRequest { RefreshToken = RefreshToken });
            ApplyAuth(response);
            return response;
        }

        public void Logout()
        {
            AccessToken = null;
            RefreshToken = null;
            AccessTokenExpiresAt = null;
            Username = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public Task<DatabaseHealthResponse> CheckDatabaseAsync()
        {
            return GetAsync<DatabaseHealthResponse>("health/database", skipAuthRefresh: true);
        }

        public Task<PagedResponse<EmployeeResponse>> GetEmployeesAsync(int page, int pageSize, string? search, string? position, int? rank)
        {
            return GetAsync<PagedResponse<EmployeeResponse>>(
                $"employees?page={page}&pageSize={pageSize}{Query("search", search)}{Query("position", position)}{Query("rank", rank)}");
        }

        public Task<EmployeeResponse> CreateEmployeeAsync(EmployeeCreateRequest request)
        {
            return PostAsync<EmployeeResponse>("employees", request);
        }

        public Task<EmployeeResponse> UpdateEmployeeAsync(int id, EmployeeUpdateRequest request)
        {
            return PutAsync<EmployeeResponse>($"employees/{id}", request);
        }

        public Task DeleteEmployeeAsync(int id)
        {
            return DeleteAsync($"employees/{id}");
        }

        public Task<PagedResponse<ContragentResponse>> GetContragentsAsync(int page, int pageSize, string? search, string? type)
        {
            return GetAsync<PagedResponse<ContragentResponse>>(
                $"contragents?page={page}&pageSize={pageSize}{Query("search", search)}{Query("type", type)}");
        }

        public Task<ContragentResponse> CreateContragentAsync(ContragentCreateRequest request)
        {
            return PostAsync<ContragentResponse>("contragents", request);
        }

        public Task<ContragentResponse> UpdateContragentAsync(int id, ContragentUpdateRequest request)
        {
            return PutAsync<ContragentResponse>($"contragents/{id}", request);
        }

        public Task DeleteContragentAsync(int id)
        {
            return DeleteAsync($"contragents/{id}");
        }

        public Task<PagedResponse<MaterialResponse>> GetMaterialsAsync(int contragentId, int page, int pageSize)
        {
            return GetAsync<PagedResponse<MaterialResponse>>(
                $"materials?contragentId={contragentId}&page={page}&pageSize={pageSize}");
        }

        public Task<MaterialResponse> CreateMaterialAsync(MaterialCreateRequest request)
        {
            return PostAsync<MaterialResponse>("materials", request);
        }

        public Task<MaterialResponse> UpdateMaterialAsync(int id, MaterialUpdateRequest request)
        {
            return PutAsync<MaterialResponse>($"materials/{id}", request);
        }

        public Task DeleteMaterialAsync(int id)
        {
            return DeleteAsync($"materials/{id}");
        }

        public Task<PagedResponse<WarehouseResponse>> GetWarehouseAsync(int page, int pageSize, string? search, string? contragentType, string? materialType)
        {
            return GetAsync<PagedResponse<WarehouseResponse>>(
                $"warehouse?page={page}&pageSize={pageSize}{Query("search", search)}{Query("contragentType", contragentType)}{Query("materialType", materialType)}");
        }

        public Task<WarehouseResponse> UpdateWarehouseQuantityAsync(int id, decimal qty)
        {
            return PutAsync<WarehouseResponse>($"warehouse/{id}/quantity", new WarehouseUpdateQtyRequest { Qty = qty });
        }

        public Task<WarehouseResponse> DeductWarehouseQuantityAsync(int id, decimal qty)
        {
            return PostAsync<WarehouseResponse>($"warehouse/{id}/deduct", new WarehouseUpdateQtyRequest { Qty = qty });
        }

        public Task DeleteWarehouseAsync(int id)
        {
            return DeleteAsync($"warehouse/{id}");
        }

        public Task<PagedResponse<FillResponse>> GetFillsAsync(int page, int pageSize, string? type, DateTime? dateFrom, DateTime? dateTo)
        {
            return GetAsync<PagedResponse<FillResponse>>(
                $"fills?page={page}&pageSize={pageSize}{Query("type", type)}{Query("dateFrom", dateFrom?.ToString("O"))}{Query("dateTo", dateTo?.ToString("O"))}");
        }

        public Task<FillResponse> CreateFillAsync(FillCreateRequest request)
        {
            return PostAsync<FillResponse>("fills", request);
        }

        public Task<FillStatsResponse> GetFillStatsAsync()
        {
            return GetAsync<FillStatsResponse>("fills/stats");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private void ApplyAuth(AuthResponse response)
        {
            AccessToken = response.AccessToken;
            RefreshToken = response.RefreshToken;
            AccessTokenExpiresAt = response.ExpiresAt;
            Username = response.Username;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }

        private async Task<T> GetAsync<T>(string path, bool skipAuthRefresh = false)
        {
            if (!skipAuthRefresh)
            {
                await EnsureFreshTokenAsync();
            }

            try
            {
                using var response = await _httpClient.GetAsync(path);
                return await ReadResponseAsync<T>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException(HttpStatusCode.BadGateway, 
       $"Ошибка соединения с API: {ex.Message}. Проверьте, запущен ли сервер на {BaseUrl}");
            }
            catch (TaskCanceledException ex)
            {
                throw new ApiException(HttpStatusCode.RequestTimeout,
  $"Timeout при подключении к API: {ex.Message}. Сервер не отвечает.");
            }
        }

        private async Task<T> PostAsync<T>(string path, object request)
        {
            await EnsureFreshTokenAsync(path);
            using var response = await _httpClient.PostAsJsonAsync(path, request, _jsonOptions);
            return await ReadResponseAsync<T>(response);
        }

        private async Task<T> PutAsync<T>(string path, object request)
        {
            await EnsureFreshTokenAsync();
            using var response = await _httpClient.PutAsJsonAsync(path, request, _jsonOptions);
            return await ReadResponseAsync<T>(response);
        }

        private async Task DeleteAsync(string path)
        {
            await EnsureFreshTokenAsync();
            using var response = await _httpClient.DeleteAsync(path);
            await EnsureSuccessAsync(response);
        }

        private async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
        {
            await EnsureSuccessAsync(response);
            var value = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            if (value is null)
            {
                throw new ApiException(response.StatusCode, "API вернул пустой ответ.");
            }

            return value;
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var body = await response.Content.ReadAsStringAsync();
            var message = TryReadProblemDetails(body);

            if (string.IsNullOrWhiteSpace(message))
            {
                message = string.IsNullOrWhiteSpace(body)
                    ? $"Ошибка API: {(int)response.StatusCode} {response.ReasonPhrase}"
                    : body;
            }

            throw new ApiException(response.StatusCode, message);
        }

        private async Task EnsureFreshTokenAsync(string? path = null)
        {
            if (path is not null && path.StartsWith("auth/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(AccessToken))
            {
                return;
            }

            if (AccessTokenExpiresAt.HasValue && AccessTokenExpiresAt.Value <= DateTime.UtcNow.AddMinutes(2))
            {
                await RefreshAsync();
            }
        }

        private static string? TryReadProblemDetails(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                var detail = root.TryGetProperty("detail", out var detailElement)
                    ? detailElement.GetString()
                    : null;
                var title = root.TryGetProperty("title", out var titleElement)
                    ? titleElement.GetString()
                    : null;

                if (root.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Array)
                {
                    var errors = errorsElement.EnumerateArray()
                        .Select(e => e.GetString())
                        .Where(e => !string.IsNullOrWhiteSpace(e));
                    return string.Join(Environment.NewLine, errors);
                }

                return detail ?? title;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static string Query(string name, object? value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            var text = value.ToString();
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : $"&{name}={Uri.EscapeDataString(text)}";
        }

        private static string NormalizeBaseUrl(string value)
        {
            var normalized = string.IsNullOrWhiteSpace(value)
                ? "http://localhost:5217/api/"
                : value.Trim();

            if (!normalized.EndsWith("/", StringComparison.Ordinal))
            {
                normalized += "/";
            }

            if (!normalized.EndsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                normalized += "api/";
            }

            return normalized;
        }
    }
}
