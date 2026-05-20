using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var blogs = new List<Blog>
{
   new Blog { Title = "My First Blog", Body = "This is my first blog post!" },
   new Blog { Title = "My Second Blog", Body = "This is my second blog post!" }
};

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "I am root!").ExcludeFromDescription();
app.MapGet("/blogs", () =>
{
    return blogs;   
});

app.MapGet("/blogs/{id}", Results<Ok<Blog>, NotFound> (int id) =>
{
    if (id < 0 || id >= blogs.Count)
    {
        return TypedResults.NotFound();
    }
    else
    {
        return TypedResults.Ok(blogs[id]);
    }
}).WithOpenApi(operation =>
{
    operation.Parameters[0].Description = "The ID of the blog to retrieve";
    operation.Summary = "Get a blog by ID";
    operation.Description = "Returns a single blog post based on the provided ID.";
    return operation;
});/*.Produces<Blog>(StatusCodes.Status200OK)
  .Produces(StatusCodes.Status404NotFound);*/

app.MapPost("/blogs", (Blog blog) =>
{
    blogs.Add(blog);
    return Results.Created($"/blogs/{blogs.Count - 1}", blog);
});

app.Run();

record Blog
{
    required public string Title { get; init; }
    required public string Body { get; init; }
}
