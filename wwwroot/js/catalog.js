(() => {
  const cartIdKey = "nha-gia-kim-cart-id";
  const cartCount = document.querySelector("[data-cart-count]");
  const cartToggle = document.querySelector("[data-cart-toggle]");
  const cartPopover = document.querySelector("[data-cart-popover]");
  const cartLines = document.querySelector("[data-cart-lines]");
  const cartSubtotal = document.querySelector("[data-cart-subtotal]");
  const notice = document.querySelector("[data-cart-notice]");
  let addQueue = Promise.resolve();

  if (!cartCount || !cartToggle || !cartPopover || !cartLines || !cartSubtotal || !notice) {
    return;
  }

  const formatPrice = (price) =>
    new Intl.NumberFormat("vi-VN", { maximumFractionDigits: 0 }).format(price) + " đ";

  const showNotice = (message) => {
    notice.textContent = message;
  };

  const getCartId = (create) => {
    let cartId = window.localStorage.getItem(cartIdKey);
    if (!cartId && create) {
      cartId = window.crypto.randomUUID();
      window.localStorage.setItem(cartIdKey, cartId);
    }
    return cartId;
  };

  const updateCount = (cart) => {
    const count = cart.items.reduce((total, item) => total + item.quantity, 0);
    cartCount.textContent = String(count);
    cartToggle.setAttribute(
      "aria-label",
      `Mở giỏ hàng, ${count} sản phẩm`
    );
  };

  const renderCart = (cart) => {
    updateCount(cart);
    cartLines.replaceChildren();

    if (cart.items.length === 0) {
      const emptyMessage = document.createElement("p");
      emptyMessage.className = "cart-empty";
      emptyMessage.textContent = "Giỏ hàng đang trống.";
      cartLines.append(emptyMessage);
      cartSubtotal.hidden = true;
      return;
    }

    for (const item of cart.items) {
      const row = document.createElement("div");
      row.className = "cart-line";

      const details = document.createElement("div");
      const title = document.createElement("p");
      title.className = "cart-line-title";
      title.textContent = item.title;
      const meta = document.createElement("p");
      meta.className = "cart-line-meta";
      meta.textContent = `${item.quantity} cuốn · ${formatPrice(item.unitPrice)}`;
      details.append(title, meta);

      const lineTotal = document.createElement("span");
      lineTotal.className = "cart-line-total";
      lineTotal.textContent = formatPrice(item.lineTotal);
      row.append(details, lineTotal);
      cartLines.append(row);
    }

    cartSubtotal.replaceChildren();
    const label = document.createElement("span");
    label.textContent = "Tạm tính";
    const total = document.createElement("span");
    total.textContent = formatPrice(cart.subtotal);
    cartSubtotal.append(label, total);
    cartSubtotal.hidden = false;
  };

  document.addEventListener("cart:updated", (event) => {
    renderCart(event.detail);
  });

  const getApiError = async (response) => {
    try {
      const problem = await response.json();
      return problem.detail || problem.title || `Yêu cầu thất bại (${response.status}).`;
    } catch {
      return `Yêu cầu thất bại (${response.status}).`;
    }
  };

  const loadCart = async () => {
    const cartId = getCartId(false);
    if (!cartId) {
      renderCart({ items: [], subtotal: 0 });
      return;
    }

    const response = await fetch(`/api/carts/${encodeURIComponent(cartId)}`);
    if (!response.ok) {
      throw new Error(await getApiError(response));
    }
    const cart = await response.json();
    renderCart(cart);
    document.dispatchEvent(new CustomEvent("cart:updated", { detail: cart }));
  };

  cartToggle.addEventListener("click", async () => {
    const isOpening = cartPopover.hidden;
    cartPopover.hidden = !isOpening;
    cartToggle.setAttribute("aria-expanded", String(isOpening));
    if (!isOpening) return;

    try {
      await loadCart();
    } catch (error) {
      cartLines.replaceChildren();
      const errorMessage = document.createElement("p");
      errorMessage.className = "cart-error";
      errorMessage.textContent = `Không tải được giỏ hàng: ${error.message}`;
      cartLines.append(errorMessage);
      cartSubtotal.hidden = true;
    }
  });

  document.querySelector("[data-cart-close]")?.addEventListener("click", () => {
    cartPopover.hidden = true;
    cartToggle.setAttribute("aria-expanded", "false");
    cartToggle.focus();
  });

  document.querySelectorAll("[data-add-to-cart]").forEach((button) => {
    button.addEventListener("click", () => {
      const label = button.querySelector("span");
      const originalLabel = label.textContent;
      const wasDisabled = button.disabled;
      button.disabled = true;
      label.textContent = "Đang chờ…";

      addQueue = addQueue.then(async () => {
        label.textContent = "Đang thêm…";
        try {
          const cartId = getCartId(true);
          const response = await fetch(`/api/carts/${encodeURIComponent(cartId)}/items`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
              bookId: Number(button.dataset.addToCart),
              quantity: 1
            })
          });

          if (!response.ok) {
            throw new Error(await getApiError(response));
          }

          const cart = await response.json();
          renderCart(cart);
          document.dispatchEvent(new CustomEvent("cart:updated", { detail: cart }));
          showNotice("Đã thêm sách vào giỏ hàng.");
        } catch (error) {
          showNotice(`Chưa thể thêm sách vào giỏ: ${error.message}`);
        } finally {
          button.disabled = wasDisabled;
          label.textContent = originalLabel;
        }
      });
    });
  });

  if (getCartId(false)) {
    loadCart().catch((error) => {
      showNotice(`Không tải được giỏ hàng đã lưu: ${error.message}`);
    });
  }
})();
