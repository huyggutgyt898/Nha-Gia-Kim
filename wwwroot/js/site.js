// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("click", (event) => {
  if (!(event.target instanceof Element)) return;

  const link = event.target.closest(".shop-mobile-links a");
  if (link) {
    link.closest(".shop-mobile-nav")?.removeAttribute("open");
  }
});
