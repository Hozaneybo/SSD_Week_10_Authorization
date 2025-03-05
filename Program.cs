using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ssd_authorization_solution;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AppDb");
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<DbSeeder>();

// Add controllers
builder.Services.AddControllers();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    // Policy for Editors
    options.AddPolicy("EditorOnly", policy => policy.RequireRole(Roles.Editor));

    // Policy for Writers
    options.AddPolicy("WriterOnly", policy => policy.RequireRole(Roles.Writer));

    // Policy for Subscribers
    options.AddPolicy("SubscriberOnly", policy => policy.RequireRole(Roles.Subscriber));

    // Policy for Writers to edit their own articles
    options.AddPolicy("WriterOrEditor", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var isWriter = user.IsInRole(Roles.Writer);
            var isEditor = user.IsInRole(Roles.Editor);
            return isWriter || isEditor;
        });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Identity services
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync().Wait();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapIdentityApi<IdentityUser>();

app.Run();