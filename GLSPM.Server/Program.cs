using GLSPM.Application;
using GLSPM.Application.EFCore.Repositories;
using GLSPM.Server;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();
try
{
    Log.Information("Application Starting.");
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
    app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();


    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");


    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application failed to start.");
    File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "MescoOnlinePaymentError.txt"), ex.Message);
}
finally
{
    Log.CloseAndFlush();
}


