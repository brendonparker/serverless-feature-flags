using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) =>
{
    config.MinimumLevel.Information()
    .Enrich.WithSpan()
    .WriteTo.Console(new ElasticsearchJsonFormatter());
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddRazorPages();
builder.Services.AddSSFImplementations(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
