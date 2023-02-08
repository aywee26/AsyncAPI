using AsyncAPI.Data;
using AsyncAPI.DTOs;
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

// Status Endpoint
app.MapGet("api/v1/productstatus/{requestId}", (AppDbContext context, string requestId) =>
{
    var listingRequest = context.ListingRequests
        .FirstOrDefault(lr => lr.RequestId == requestId);

    if (listingRequest is null)
        return Results.NotFound();

    var listingStatus = new ListingStatus
    {
        RequestStatus = listingRequest.RequestStatus,
        ResourceURL = string.Empty
    };

    if (listingRequest.RequestStatus!.ToUpper() == "COMPLETE")
    {
        listingStatus.ResourceURL = $"api/v1/products/{Guid.NewGuid().ToString()}";
        //return Results.Ok(listingStatus);
        return Results.Redirect($"~/{listingStatus.ResourceURL}");
    }

    listingStatus.EstimatedCompletionTime = "2023-02-08:19:30:00";
    return Results.Ok(listingStatus);
});

// Final Endpoint
app.MapGet("api/v1/products/{requestId}", (string requestId) =>
{
    return Results.Ok("This is where the final result is passed.");
});

app.Run();