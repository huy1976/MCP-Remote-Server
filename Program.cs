using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1. Cấu hình HTTP Client để gọi GitHub
var client = new HttpClient { BaseAddress = new Uri("https://api.github.com/") };
string? token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

client.DefaultRequestHeaders.Add("User-Agent", "MCP-Remote-Server");
client.DefaultRequestHeaders.Authorization = new("Bearer", token);

// TOOL 1: Lấy danh sách Issues (Sửa lỗi Disposed)
app.MapGet("/get-issues", async () => {
    try
    {
        string raw = await client.GetStringAsync("repos/huy1976/MCP-System-Assignment/issues?state=open");

        // Không dùng "using" ở đây để tránh bị hủy quá sớm
        var doc = JsonDocument.Parse(raw);

        // Trả về Clone của RootElement để an toàn tuyệt đối
        return Results.Json(doc.RootElement.Clone());
    }
    catch (Exception ex)
    {
        return Results.Problem("Loi: " + ex.Message);
    }
});

//  ENDPOINT 2: Tạo Issue mới (POST /create-issue) 
app.MapPost("/create-issue", async (JsonElement body) => {
    try
    {
        var response = await client.PostAsJsonAsync("repos/huy1976/MCP-System-Assignment/issues", body);
        if (response.IsSuccessStatusCode) return Results.Ok("Da tao Issue thanh cong tren GitHub !");
        return Results.StatusCode((int)response.StatusCode);
    }
    catch (Exception ex)
    {
        return Results.Problem("Loi: " + ex.Message);
    }
});
app.Run();