using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Nha_Gia_Kim.Models.Api;
using Nha_Gia_Kim.Services;

namespace Nha_Gia_Kim.Controllers.Api;

[ApiController]
[Route("api")]
[Produces("application/json")]
public sealed class CatalogController(ICatalogService catalogService) : ControllerBase
{
    /// <summary>Returns a paged list of active books.</summary>
    [HttpGet("books")]
    [EndpointSummary("Browse books")]
    [EndpointDescription("Returns active books with optional title/author search, category filter, and pagination.")]
    [ProducesResponseType<BookListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookListResponse>> GetBooks(
        [FromQuery, StringLength(100)] string? search,
        [FromQuery, Range(1, 1000000)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        [FromQuery, Range(1, int.MaxValue)] int? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await catalogService.GetBooksAsync(search, categoryId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns details for one active book.</summary>
    [HttpGet("books/{id:int}", Name = nameof(GetBookById))]
    [EndpointSummary("Get a book")]
    [EndpointDescription("Returns details for an active book by its catalog identifier.")]
    [ProducesResponseType<BookResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponse>> GetBookById(int id, CancellationToken cancellationToken)
    {
        var book = await catalogService.GetBookByIdAsync(id, cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    /// <summary>Returns all book categories.</summary>
    [HttpGet("categories")]
    [EndpointSummary("List categories")]
    [EndpointDescription("Returns the categories currently used to organize the book catalog.")]
    [ProducesResponseType<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetCategories(CancellationToken cancellationToken)
    {
        return Ok(await catalogService.GetCategoriesAsync(cancellationToken));
    }
}
