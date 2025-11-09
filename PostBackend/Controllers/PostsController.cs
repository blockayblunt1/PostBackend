using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostBackend.Data;
using PostBackend.Dtos;
using PostBackend.Models;

namespace PostBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(AppDbContext db, IWebHostEnvironment env) : ControllerBase
{
    // 1. List posts with search and sort
    // GET: api/posts?search=abc&sort=asc|desc
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetAll([FromQuery] string? search, [FromQuery] string sort = "asc")
    {
        IQueryable<Post> query = db.Posts.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        query = sort?.ToLowerInvariant() == "desc"
            ? query.OrderByDescending(p => p.Name)
            : query.OrderBy(p => p.Name);

        var items = await query.ToListAsync();
        return Ok(items);
    }

    // GET: api/posts/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Post>> GetById(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        return Ok(post);
    }

    // 2. Create a Post
    [HttpPost]
    public async Task<ActionResult<Post>> Create([FromBody] CreatePostDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var post = new Post
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Posts.Add(post);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    // 3. Edit a Post
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Post>> Update(int id, [FromBody] UpdatePostDto dto)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return NotFound();

        post.Name = dto.Name.Trim();
        post.Description = dto.Description.Trim();
        post.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl;
        post.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(post);
    }

    // 4. Delete a Post
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // Optional: Upload an image file (returns URL to use in ImageUrl)
    [HttpPost("upload-image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)] // ~10MB
    public async Task<ActionResult<string>> UploadImage([FromForm] UploadImageDto request)
    {
        var file = request.File;
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadsDir = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);
        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        var url = $"/uploads/{fileName}"; // served by UseStaticFiles
        return Ok(url);
    }
}
