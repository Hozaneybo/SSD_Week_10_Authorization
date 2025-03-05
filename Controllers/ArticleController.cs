using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ssd_authorization_solution.DTOs;
using ssd_authorization_solution.Entities;

namespace MyApp.Namespace;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleController(AppDbContext ctx)
    {
        this.db = ctx;
    }

    [HttpGet]
    [AllowAnonymous]
    public IEnumerable<ArticleDto> Get()
    {
        return db.Articles.Include(x => x.Author).Select(ArticleDto.FromEntity);
    }

    [HttpGet(":id")]
    [AllowAnonymous]
    public ArticleDto? GetById(int id)
    {
        return db
            .Articles.Include(x => x.Author)
            .Where(x => x.Id == id)
            .Select(ArticleDto.FromEntity)
            .SingleOrDefault();
    }

    [HttpPost]
    [Authorize(Policy = "WriterOnly")]
    public ArticleDto Post([FromBody] ArticleFormDto dto)
    {
        var userName = HttpContext.User.Identity?.Name;
        var author = db.Users.Single(x => x.UserName == userName);
        var entity = new Article
        {
            Title = dto.Title,
            Content = dto.Content,
            Author = author,
            CreatedAt = DateTime.Now
        };
        var created = db.Articles.Add(entity).Entity;
        db.SaveChanges();
        return ArticleDto.FromEntity(created);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "WriterOrEditor")]
    public IActionResult Put(int id, [FromBody] ArticleFormDto dto)
    {
        var userName = HttpContext.User.Identity?.Name;
        var entity = db
            .Articles
            .Include(x => x.Author)
            .SingleOrDefault(x => x.Id == id); // Use SingleOrDefault

        if (entity == null)
        {
            return NotFound(); // Return 404 if the article is not found
        }

        // Ensure the user is the author or an Editor
        if (entity.Author.UserName != userName && !User.IsInRole(Roles.Editor))
        {
            return Forbid(); // Return 403 Forbidden
        }

        entity.Title = dto.Title;
        entity.Content = dto.Content;
        db.Articles.Update(entity);
        db.SaveChanges();
        return Ok(ArticleDto.FromEntity(entity));
    }
    
    [HttpDelete("{id}")]
    [Authorize(Policy = "EditorOnly")]
    public IActionResult Delete(int id)
    {
        var entity = db.Articles.Find(id);
        if (entity == null)
        {
            return NotFound();
        }
        db.Articles.Remove(entity);
        db.SaveChanges();
        return NoContent();
    }
}
