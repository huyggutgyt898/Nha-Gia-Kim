using Microsoft.AspNetCore.Mvc;
using Nha_Gia_Kim.Models;
using Nha_Gia_Kim.Models.ViewModels;
using Nha_Gia_Kim.Services;
using System.Diagnostics;

namespace Nha_Gia_Kim.Controllers
{
    public class HomeController(
        ICatalogService catalogService,
        IBookReviewService bookReviewService,
        IPressArticleService pressArticleService) : Controller
    {
        private const int FeaturedBookId = 1;

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var viewModel = await BuildLandingViewModelAsync(new BookReviewSubmission(), cancellationToken);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(
            [Bind(Prefix = "ReviewForm")] BookReviewSubmission submission,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await BuildLandingViewModelAsync(submission, cancellationToken);
                return View(nameof(Index), viewModel);
            }

            if (!await bookReviewService.AddAsync(FeaturedBookId, submission, cancellationToken))
            {
                return NotFound();
            }

            TempData["ReviewSuccess"] = "Cảm ơn bạn đã gửi đánh giá.";
            return Redirect($"{Url.Action(nameof(Index))}#danh-gia");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<HomeLandingViewModel> BuildLandingViewModelAsync(
            BookReviewSubmission reviewForm,
            CancellationToken cancellationToken)
        {
            var books = await catalogService.GetBooksAsync(null, null, 1, 100, cancellationToken);
            var reviewSummary = await bookReviewService.GetSummaryAsync(FeaturedBookId, cancellationToken);
            var pressArticles = await pressArticleService.GetForBookAsync(FeaturedBookId, cancellationToken);
            var relatedBooks = books.Items
                .Where(book => book.Id != FeaturedBookId && book.CategoryId == 3)
                .Take(4)
                .ToArray();

            if (relatedBooks.Length == 0)
            {
                relatedBooks = books.Items
                    .Where(book => book.Id != FeaturedBookId)
                    .Take(4)
                    .ToArray();
            }

            return new HomeLandingViewModel(
                books,
                reviewSummary.Reviews,
                reviewSummary.Count,
                reviewSummary.AverageRating,
                relatedBooks,
                pressArticles,
                reviewForm);
        }
    }
}
