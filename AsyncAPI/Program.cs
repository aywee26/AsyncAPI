using AsyncAPI.Data;
using AsyncAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    opt => opt.UseSqlite("Data Source=RequestDb.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Start Endpoint
app.MapPost("/api/v1/products", async (AppDbContext context, ListingRequest listingRequest) =>
{
    if (listingRequest is null)
        return Results.BadRequest();

    listingRequest.RequestStatus = "ACCEPT";
    listingRequest.EstimatedCompletionTime = "2023-02-08:18:30:00";

    await context.ListingRequests.AddAsync(listingRequest);
    await context.SaveChangesAsync();

    return Results.Accepted(
        $"api/v1/productstatus/{listingRequest.RequestId}",
        listingRequest);
});


app.Run();