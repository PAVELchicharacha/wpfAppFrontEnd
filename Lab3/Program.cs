using Lab3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true
    };
});

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
IServiceCollection serviceCollection = builder.Services.AddDbContext<ModelDB>(options => options.UseSqlServer(connection));
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapPost("/login", async (User loginData, ModelDB db) =>
{
    User? person = await db.Users!.FirstOrDefaultAsync(p => p.EMail == loginData.EMail &&
p.Password == loginData.Password);
    if (person is null) return Results.Unauthorized();
    var claims = new List<Claim> { new Claim(ClaimTypes.Email, person.EMail!) };
    var jwt = new JwtSecurityToken(issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.Now.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );
    var encoderJWT = new JwtSecurityTokenHandler().WriteToken(jwt);
    var response = new
    {
        access_token = encoderJWT,
        username = person.EMail
    };
    return Results.Json(response);
});

app.MapGet("/api/Pricelist", [Authorize] async (ModelDB db) => await db.PriceLists!.ToListAsync());
app.MapGet("api/Pricelist/{id:int}", [Authorize] async (ModelDB db, int id) => await db.PriceLists!.Where(g => g.Id == id).FirstOrDefaultAsync());
app.MapGet("/api/Product", [Authorize] async (ModelDB db) => await db.products!.ToListAsync());
app.MapGet("/api/Pricelist/{name}", [Authorize] async (ModelDB db, string name) => await db.PriceLists!.Where(u => u.Name == name).FirstOrDefaultAsync());
app.MapPost("/api/Product", [Authorize] async (PriceList priceList, ModelDB db) =>
{
    await db.PriceLists!.AddAsync(priceList);
    await db.SaveChangesAsync();
    return priceList;
});
app.MapPost("/api/PriceList", [Authorize] async (PriceList PriceList, ModelDB db) =>
{
    await db.PriceLists!.AddAsync(PriceList);
    await db.SaveChangesAsync();
    return PriceList;
});
app.MapDelete("/api/PriceList/{id:int}", [Authorize] async (int id, ModelDB db) =>
{
    PriceList? PriceList = await db.PriceLists!.FirstOrDefaultAsync(u => u.Id == id);
    if (PriceList == null) return Results.NotFound(new { message = "прайслист не найден" });
    db.PriceLists!.Remove(PriceList);
    await db.SaveChangesAsync();
    return Results.Json(PriceList);
});
app.MapDelete("/api/PriceList/{id:int}", [Authorize] async (int id, ModelDB db) =>
{
    PriceList? PriceList = await db.PriceLists!.FirstOrDefaultAsync(u => u.Id == id);
    if (PriceList == null) return Results.NotFound(new { message = "прайслист не найден" });
    db.PriceLists!.Remove(PriceList);
    await db.SaveChangesAsync();
    return Results.Json(PriceList);
});
app.MapPut("/api/PriceList", [Authorize] async (PriceList PriceList, ModelDB db) =>
{
    PriceList? g = await db.PriceLists!.FirstOrDefaultAsync(u => u.Id == PriceList.Id);
    if (g == null) return Results.NotFound(new { message = "прайслист не найден" });
    g.Name = PriceList.Name;
    g.Coast = PriceList.Coast;
    g.Id = PriceList.Id;
    await db.SaveChangesAsync();
    return Results.Json(g);
});
app.MapPut("/api/Product", [Authorize] async (Product product, ModelDB db) =>
{
    Product? st = await db.products!.FirstOrDefaultAsync(u => u.Id == product.Id);
    if (st == null) return Results.NotFound(new { message = "продукт не найдена" });
    st.SaleDate = product.SaleDate;
    st.Quantity = product.Quantity;
    st.ProductCoast = product.ProductCoast;

    await db.SaveChangesAsync();
    return Results.Json(st);
});
app.Run();
