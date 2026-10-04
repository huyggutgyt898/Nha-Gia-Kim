(() => {
  const cartIdKey = "nha-gia-kim-cart-id";
  const form = document.querySelector("[data-order-form]");
  const itemsElement = document.querySelector("[data-checkout-items]");
  const typeCountElement = document.querySelector("[data-order-types]");
  const itemCountElement = document.querySelector("[data-order-count]");
  const subtotalElement = document.querySelector("[data-order-subtotal]");
  const submitButton = document.querySelector("[data-order-submit]");
  const statusElement = document.querySelector("[data-checkout-status]");

  if (
    !form ||
    !itemsElement ||
    !typeCountElement ||
    !itemCountElement ||
    !subtotalElement ||
    !submitButton ||
    !statusElement
  ) {
    return;
  }

  let cart = { items: [], subtotal: 0 };
  let pendingOperations = 0;
  let isSubmitting = false;
  let orderPlaced = false;

  const formatPrice = (price) =>
    new Intl.NumberFormat("vi-VN", { maximumFractionDigits: 0 }).format(price) + " đ";

  const setStatus = (message, tone = "") => {
    statusElement.textContent = message;
    statusElement.dataset.tone = tone;
  };

  const getApiError = async (response) => {
    try {
      const problem = await response.json();
      return problem.detail || problem.title || `Yêu cầu thất bại (${response.status}).`;
    } catch {
      return `Yêu cầu thất bại (${response.status}).`;
    }
  };

  const syncSubmitButton = () => {
    submitButton.disabled =
      cart.items.length === 0 || pendingOperations > 0 || isSubmitting || orderPlaced;
  };

  const notifyCartChanged = () => {
    document.dispatchEvent(new CustomEvent("cart:updated", { detail: cart }));
  };

  const renderCart = () => {
    typeCountElement.textContent = String(cart.items.length);
    const totalQuantity = cart.items.reduce((total, item) => total + item.quantity, 0);
    itemCountElement.textContent = `${totalQuantity} cuốn`;
    subtotalElement.textContent = formatPrice(cart.subtotal);
    itemsElement.replaceChildren();

    if (cart.items.length === 0) {
      const emptyMessage = document.createElement("p");
      emptyMessage.className = "cart-empty";
      emptyMessage.textContent = "Giỏ hàng đang trống. Hãy chọn sách trước khi đặt hàng.";
      const catalogLink = document.createElement("a");
      catalogLink.href = "/Catalog";
      catalogLink.textContent = "Xem danh mục sách";
      catalogLink.className = "button-outline";
      itemsElement.append(emptyMessage, catalogLink);
      syncSubmitButton();
      return;
    }

    for (const item of cart.items) {
      const row = document.createElement("article");
      row.className = "checkout-item";

      if (item.coverImageUrl) {
        const cover = document.createElement("img");
        cover.className = "checkout-item-cover";
        cover.src = item.coverImageUrl;
        cover.alt = `Bìa sách ${item.title}`;
        cover.loading = "lazy";
        row.append(cover);
      } else {
        const coverPlaceholder = document.createElement("div");
        coverPlaceholder.className = "checkout-item-cover";
        coverPlaceholder.setAttribute("aria-hidden", "true");
        row.append(coverPlaceholder);
      }

      const details = document.createElement("div");
      const title = document.createElement("h3");
      title.className = "checkout-item-title";
      title.textContent = item.title;
      const price = document.createElement("p");
      price.className = "checkout-item-price";
      price.textContent = `${item.author} · ${formatPrice(item.unitPrice)} / cuốn`;
      details.append(title, price);
      row.append(details);

      const tools = document.createElement("div");
      tools.className = "checkout-item-tools";
      const quantity = document.createElement("input");
      quantity.type = "number";
      quantity.min = "1";
      quantity.max = String(Math.min(item.availableStock, 100));
      quantity.value = String(item.quantity);
      quantity.inputMode = "numeric";
      quantity.setAttribute("aria-label", `Số lượng ${item.title}`);
      quantity.disabled = pendingOperations > 0 || isSubmitting || orderPlaced;
      quantity.addEventListener("change", () => updateQuantity(item, quantity));

      const removeButton = document.createElement("button");
      removeButton.className = "checkout-remove";
      removeButton.type = "button";
      removeButton.textContent = "Xóa";
      removeButton.setAttribute("aria-label", `Xóa ${item.title} khỏi giỏ hàng`);
      removeButton.disabled = pendingOperations > 0 || isSubmitting || orderPlaced;
      removeButton.addEventListener("click", () => removeItem(item));
      tools.append(quantity, removeButton);
      row.append(tools);

      const lineTotal = document.createElement("p");
      lineTotal.className = "checkout-item-total";
      lineTotal.textContent = `Thành tiền: ${formatPrice(item.lineTotal)}`;
      row.append(lineTotal);
      itemsElement.append(row);
    }

    syncSubmitButton();
  };

  const loadCart = async () => {
    const cartId = window.localStorage.getItem(cartIdKey);
    if (!cartId) {
      cart = { items: [], subtotal: 0 };
      renderCart();
      return;
    }

    const response = await fetch(`/api/carts/${encodeURIComponent(cartId)}`);
    if (!response.ok) {
      throw new Error(await getApiError(response));
    }
    cart = await response.json();
    renderCart();
  };

  const reloadCartAfterFailure = async (message) => {
    try {
      await loadCart();
      setStatus(message, "error");
    } catch (error) {
      setStatus(`${message} Không tải lại được giỏ hàng: ${error.message}`, "error");
    }
  };

  const updateQuantity = async (item, input) => {
    const quantity = input.valueAsNumber;
    if (
      !Number.isInteger(quantity) ||
      quantity < 1 ||
      quantity > Math.min(item.availableStock, 100)
    ) {
      input.value = String(item.quantity);
      setStatus(`Số lượng “${item.title}” phải từ 1 đến ${Math.min(item.availableStock, 100)} cuốn.`, "error");
      return;
    }

    if (quantity === item.quantity) return;

    pendingOperations += 1;
    renderCart();
    setStatus("Đang cập nhật giỏ hàng…");
    try {
      const cartId = window.localStorage.getItem(cartIdKey);
      const response = await fetch(
        `/api/carts/${encodeURIComponent(cartId)}/items/${item.bookId}`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ quantity })
        }
      );
      if (!response.ok) {
        throw new Error(await getApiError(response));
      }
      cart = await response.json();
      renderCart();
      notifyCartChanged();
      setStatus("Đã cập nhật số lượng.");
    } catch (error) {
      await reloadCartAfterFailure(`Chưa thể cập nhật giỏ hàng: ${error.message}`);
    } finally {
      pendingOperations -= 1;
      renderCart();
    }
  };

  const removeItem = async (item) => {
    pendingOperations += 1;
    renderCart();
    setStatus("Đang xóa sách khỏi giỏ hàng…");
    try {
      const cartId = window.localStorage.getItem(cartIdKey);
      const response = await fetch(
        `/api/carts/${encodeURIComponent(cartId)}/items/${item.bookId}`,
        { method: "DELETE" }
      );
      if (!response.ok) {
        throw new Error(await getApiError(response));
      }
      await loadCart();
      notifyCartChanged();
      setStatus("Đã xóa sách khỏi giỏ hàng.");
    } catch (error) {
      await reloadCartAfterFailure(`Chưa thể xóa sách: ${error.message}`);
    } finally {
      pendingOperations -= 1;
      renderCart();
    }
  };

  document.addEventListener("cart:updated", (event) => {
    cart = event.detail;
    renderCart();
  });

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!form.reportValidity() || cart.items.length === 0 || pendingOperations > 0 || orderPlaced) {
      return;
    }

    const cartId = window.localStorage.getItem(cartIdKey);
    if (!cartId) {
      setStatus("Không tìm thấy giỏ hàng. Hãy thêm sách rồi thử lại.", "error");
      return;
    }

    const formData = new FormData(form);
    const request = {
      cartId,
      customerName: formData.get("customerName"),
      customerEmail: formData.get("customerEmail"),
      customerPhone: formData.get("customerPhone"),
      shippingAddress: formData.get("shippingAddress"),
      provinceCity: formData.get("provinceCity"),
      paymentMethod: formData.get("paymentMethod")
    };

    isSubmitting = true;
    submitButton.textContent = "Đang gửi đơn…";
    syncSubmitButton();
    setStatus("Đang kiểm tra tồn kho và ghi nhận đơn hàng…");
    try {
      const response = await fetch("/api/orders", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request)
      });
      if (!response.ok) {
        throw new Error(await getApiError(response));
      }

      const order = await response.json();
      orderPlaced = true;
      window.localStorage.removeItem(cartIdKey);
      cart = { items: [], subtotal: 0 };
      renderCart();
      notifyCartChanged();
      form.querySelectorAll("input, select, textarea").forEach((field) => {
        field.disabled = true;
      });
      submitButton.textContent = "Đã đặt hàng";
      const reference = order.orderNumber;
      const confirmationAddress = request.customerEmail;
      if (order.confirmationEmailSent) {
        setStatus(
          `Đặt hàng thành công! Mã đơn ${reference}. Email xác nhận đã được gửi đến ${confirmationAddress}.`,
          "success"
        );
      } else {
        setStatus(
          `Đơn hàng ${reference} đã được ghi nhận nhưng chưa gửi được email xác nhận đến ${confirmationAddress}. Vui lòng lưu lại mã đơn này và liên hệ cửa hàng.`,
          "error"
        );
      }
    } catch (error) {
      setStatus(`Chưa thể đặt hàng: ${error.message}`, "error");
      if (error.message.includes("quá nhiều") || error.message.includes("Too Many")) {
        setStatus("Bạn vừa gửi nhiều yêu cầu. Vui lòng chờ một lúc rồi thử lại.", "error");
      }
      if (error.message.includes("không đủ") || error.message.includes("thay đổi")) {
        await loadCart();
      }
    } finally {
      isSubmitting = false;
      syncSubmitButton();
    }
  });

  loadCart().catch((error) => {
    itemsElement.replaceChildren();
    const errorMessage = document.createElement("p");
    errorMessage.className = "cart-error";
    errorMessage.textContent = `Không tải được giỏ hàng: ${error.message}`;
    itemsElement.append(errorMessage);
    setStatus("Hãy thử tải lại trang hoặc mở giỏ hàng để kiểm tra.", "error");
    syncSubmitButton();
  });
})();
