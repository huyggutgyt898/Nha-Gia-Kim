# KẾ HOẠCH THIẾT KẾ & TRIỂN KHAI LANDING PAGE NHÀ GIẢ KIM

> **Phiên bản:** 1.0 · **Phong cách:** Premium Dark / Mystical Luxury · **Kiến trúc:** Single-page landing page · **Ngôn ngữ giao diện:** Tiếng Việt
>
> **Mục tiêu tài liệu:** Tài liệu đặc tả có thể giao thẳng cho lập trình viên hoặc AI coding assistant để triển khai HTML, CSS, Tailwind CSS, JavaScript và icon Font Awesome/SVG. Không phải bản code sản phẩm hoàn chỉnh.

---

## 1. Tổng quan dự án

### 1.1. Sản phẩm

- **Tên sách:** Nhà Giả Kim (*The Alchemist*)
- **Tác giả:** Paulo Coelho
- **Phiên bản bán:** sử dụng đúng bìa **tím đêm + họa tiết vàng** trong ảnh được cung cấp; không tự thay bằng phiên bản bìa khác.
- **Loại website:** Landing page giới thiệu và bán **một đầu sách**.
- **Đối tượng:** Người quan tâm văn học, phát triển bản thân, quà tặng sách và tác phẩm truyền cảm hứng.

### 1.2. Mục tiêu

1. Tạo trải nghiệm thị giác cao cấp, mềm mại và nhất quán với bìa sách.
2. Trình bày đủ thông tin trước khi khách ra quyết định: sản phẩm, tác giả, truyền thông, đọc thử, đánh giá.
3. Tối ưu chuyển đổi thông qua các CTA mua sách xuất hiện hợp lý, **đều mở một form đặt hàng chung**.
4. Responsive tốt trên mobile, tablet, desktop; chú trọng tốc độ tải, accessibility và khả năng bảo trì.
5. Tách nội dung khỏi giao diện để dễ sửa giá bán, hình ảnh, video, đánh giá, thông tin nhà sách.

### 1.3. Cấu trúc nội dung, đúng thứ tự

1. **Header** — logo, menu anchor, CTA.
2. **Hero** — mockup sách, tiêu đề, phụ đề, mô tả, CTA mua/đọc thử.
3. **About the Author** — ảnh và tiểu sử ngắn Paulo Coelho.
4. **Media & Video** — video và bài viết thực tế về sách.
5. **Book Preview** — các trang sách được phép công khai để đọc thử.
6. **Customer Reviews** — đánh giá thực tế và số sao.
7. **Footer** — thông tin nhà sách, liên hệ, chính sách.
8. **Order Modal** — form đặt hàng dùng chung, không phải section nằm trong dòng cuộn.

**Lưu ý:** Không tự ý thêm các section dài làm phá vỡ luồng. Có thể đặt *mini-CTA* hoặc strip thông tin giá ở cuối section hiện có khi cần.

---

## 2. Công nghệ và quy tắc triển khai

| Công nghệ | Vai trò | Nguyên tắc |
|---|---|---|
| **HTML5** | Semantic structure, SEO | `header`, `nav`, `main`, `section`, `article`, `footer`, `dialog`/modal có accessibility |
| **Tailwind CSS** | Utility-first layout, spacing, responsive | Dùng Tailwind CSS v4 với CSS-first theme; build bằng CLI/Vite, **không dùng Play CDN trong production** |
| **CSS3 tùy chỉnh** | Design tokens, gradient, decorative motif, animation | Chỉ viết CSS tùy chỉnh khi utility không phù hợp; tránh lặp style |
| **JavaScript ES Modules** | Mobile nav, preview slider, modal, form validation, API | Vanilla JS, tách module, không cần framework cho landing page một sản phẩm |
| **Font Awesome** | Icon thông dụng | Cài package hoặc dùng build tối ưu; tránh tải cả thư viện khi chỉ dùng vài icon |
| **SVG tự thiết kế** | Stars, sunburst, sand dunes, arrow decoration | SVG inline hoặc file tĩnh; nét mảnh, nhất quán, không lạm dụng |
| **Google Fonts / self-host** | Typography | Playfair Display và Be Vietnam Pro; ưu tiên self-host, subset và `font-display: swap` |
| **Vite** | Development/build | Tùy chọn được khuyến nghị để tổ chức assets và tối ưu bundling |
| **Backend API** | Đơn hàng và thanh toán | Cần backend/serverless endpoint thực sự; HTML/JS thuần không đủ để lưu đơn an toàn |

### 2.1. Thiết lập đề xuất

- Node.js phiên bản LTS tương thích Vite và Tailwind mới nhất đang cài.
- Vite + Tailwind CSS v4 qua `@tailwindcss/vite` (hoặc Tailwind CLI nếu không dùng Vite).
- Dùng npm để quản lý dependencies. Lockfile phải được commit.
- Không trộn Tailwind v3 config (`tailwind.config.js`) một cách máy móc với theme v4; ưu tiên `@theme` trong CSS.
- Dùng `@fortawesome/fontawesome-free` hoặc bộ SVG icon đã chọn; chỉ sử dụng icon có license phù hợp.

**Ví dụ `src/styles/main.css` với Tailwind v4:**

```css
@import "tailwindcss";

@theme {
  --color-midnight: #21123D;
  --color-royal: #34205E;
  --color-antique-gold: #D8AD61;
  --color-champagne: #F3D9A5;
  --color-ivory: #F7F2E9;
  --color-sand: #EAE0D2;
  --color-terracotta: #C98754;
  --color-ink: #292338;
  --font-display: "Playfair Display", Georgia, serif;
  --font-body: "Be Vietnam Pro", system-ui, sans-serif;
}

:root {
  --site-max-width: 1240px;
  --header-height: 76px;
  --radius-card: 20px;
  --radius-button: 12px;
  --shadow-card: 0 16px 40px rgba(33, 18, 61, .10);
}

html { scroll-behavior: smooth; scroll-padding-top: var(--header-height); }
body { font-family: var(--font-body); color: var(--color-ink); background: var(--color-ivory); }
button, input, textarea, select { font: inherit; }
@media (prefers-reduced-motion: reduce) {
  html { scroll-behavior: auto; }
  *, *::before, *::after { animation-duration: .01ms !important; transition-duration: .01ms !important; }
}
```

**Ví dụ utility class**: `bg-midnight`, `bg-antique-gold`, `text-champagne`, `font-display`, `max-w-[1240px]`, `md:grid-cols-2`, `lg:py-24`. Kiểm tra CSS được Tailwind build trước khi sử dụng trong production.

### 2.2. Quy tắc HTML

- Mỗi section có ID ổn định (`#gioi-thieu`, `#tac-gia`, `#truyen-thong`, `#doc-thu`, `#danh-gia`).
- Một `h1` duy nhất ở Hero; mỗi section một `h2`, heading con `h3`.
- Ảnh có `alt` hữu ích, `width`, `height`; mockup Hero dùng tải ưu tiên, ảnh bên dưới dùng lazy load.
- Anchor menu phải cuộn đến đúng section và không bị header che mất.
- Các thao tác mở modal phải dùng `<button type="button">`, không dùng anchor giả.
- SVG mang tính trang trí dùng `aria-hidden="true"`; SVG có nội dung dùng `role="img"` và title.

---

## 3. Design system — phương án A

### 3.1. Bảng màu cố định

| Token | Hex | Dùng cho |
|---|---|---|
| `midnight` | `#21123D` | Hero, nền CTA lớn, khu vực nổi bật |
| `royal` | `#34205E` | Heading và card tối |
| `antique-gold` | `#D8AD61` | Primary CTA, icon, accent |
| `champagne` | `#F3D9A5` | Heading trên nền tím |
| `ivory` | `#F7F2E9` | Nền nội dung chính |
| `sand` | `#EAE0D2` | Nền xen kẽ, card |
| `terracotta` | `#C98754` | Họa tiết sa mạc tinh tế |
| `ink` | `#292338` | Nội dung trên nền sáng |
| `footer` | `#180E2C` | Footer |

Phân bổ gợi ý: **45% Ivory / 30% Midnight / 15% Royal / 7% Gold / 3% accent khác**. Không dùng vàng kim làm màu body text trên nền sáng khi tương phản không đủ.

### 3.2. Chữ

| Thành phần | Desktop | Mobile | Font |
|---|---|---|---|
| Hero H1 | 56–64px | 34–40px | Playfair Display Bold |
| H2 section | 36–44px | 26–30px | Playfair Display SemiBold |
| H3/card title | 20–24px | 18–20px | Be Vietnam Pro SemiBold |
| Body | 16–18px | 15–16px | Be Vietnam Pro Regular |
| Caption/eyebrow | 12–14px | 12px | Be Vietnam Pro Medium |
| CTA | 15–17px | 15–16px | Be Vietnam Pro SemiBold |

Body line-height 1.6–1.8; nội dung dài nên giới hạn độ rộng khoảng 65–75 ký tự một dòng.

### 3.3. Bố cục & khoảng cách

- Container: `max-width: 1240px`, padding ngang mobile 20px, tablet 32px, desktop 40px.
- Section spacing: desktop 88–112px; tablet 72–88px; mobile 56–72px.
- Card radius: 20px; button radius: 12px; image radius: 16px.
- Hệ spacing theo thang 4/8px, không dùng khoảng cách ngẫu nhiên.
- Shadow nhẹ tông tím, không dùng shadow đen quá nặng.
- Transition 180–300ms, ease-out; animation chủ yếu opacity/transform.

### 3.4. Hệ thống CTA

**Primary CTA:** nền `#D8AD61`, chữ `#21123D`, icon mũi tên hướng phải; padding `14–18px 24–32px`; chiều cao tối thiểu 48px, dễ chạm trên mobile.

**Secondary CTA:** viền vàng kim trên nền tối hoặc viền tím trên nền sáng; text cùng màu viền.

**Hover/focus:** hover tăng độ sáng/nhấc nhẹ 1–2px; focus-visible có outline rõ; disabled thể hiện trạng thái. Mọi CTA mua đều có `data-open-order` và được JS gắn cùng một handler.

### 3.5. SVG motif & Font Awesome

**Font Awesome gợi ý:** `fa-bag-shopping`, `fa-arrow-right`, `fa-book-open`, `fa-star`, `fa-play`, `fa-chevron-left`, `fa-chevron-right`, `fa-location-dot`, `fa-phone`, `fa-envelope`, `fa-bars`, `fa-xmark`, `fa-check`, `fa-lock`.

**SVG tự thiết kế:** chòm sao, mặt trời phát tia, đường viền cồn cát, sparkles mảnh. Nét `stroke-width` 1–1.5px, opacity thấp, sắc Champagne Gold. Không dùng quá 1–2 cụm decoration cho một vùng màn hình. Ưu tiên SVG có thể tái sử dụng thay vì hình raster nặng.

---

## 4. Đặc tả từng thành phần

### 4.1. Header / Navigation

**Mục tiêu:** định hướng nội dung và giữ CTA dễ tiếp cận.

- Nền mặc định `midnight`, text trắng ngà, CTA Gold.
- Desktop: logo trái; các mục **Giới thiệu / Tác giả / Truyền thông / Đọc thử / Đánh giá** ở giữa/phải; CTA **MUA NGAY** cuối hàng.
- Sticky top, có backdrop blur nhẹ khi cuộn. Cẩn trọng tối ưu hiệu năng trên mobile.
- Mobile: logo, nút MUA NGAY nhỏ, nút menu; drawer có focus management, `aria-expanded`, đóng khi click link/Escape.
- Không che nội dung do sticky header; anchor offset.

**Tương tác:** click menu → smooth scroll; click CTA → mở modal đặt hàng.

**Tiêu chí nghiệm thu:** không nhảy layout khi sticky, hiển thị đúng các breakpoint 320/375/768/1024/1440px.

### 4.2. Hero Section — `#gioi-thieu`

**Mục tiêu:** tạo ấn tượng trong 3–5 giây đầu; bìa sách và mua ngay là tâm điểm.

**Bố cục desktop:** hai cột 55/45 hoặc 50/50; copy bên trái và mockup sách 3D bên phải. Nền gradient `#21123D → #34205E`, ánh sáng vàng mờ sau ảnh, motif sa mạc/chòm sao với opacity thấp.

**Mobile:** xếp 1 cột, title rồi ảnh sách, CTA trong vùng dễ thấy; cân nhắc CTA ngay sau subtitle nếu mockup cao.

**Copy gợi ý:**

- Eyebrow: `TÁC PHẨM CỦA PAULO COELHO`
- H1: `NHÀ GIẢ KIM`
- Subtitle: `Một hành trình đầy cảm hứng về giấc mơ, lòng dũng cảm và việc lắng nghe tiếng gọi của trái tim.`
- Mô tả: `Theo chân Santiago trên hành trình đi tìm kho báu và khám phá ý nghĩa của việc theo đuổi ước mơ.`
- CTA chính: `MUA SÁCH NGAY` (mở modal).
- CTA phụ: `ĐỌC THỬ` (anchor đến `#doc-thu`).

**Ảnh:** file mockup đúng bìa người dùng cung cấp; ưu tiên WebP/AVIF, shadow chân thực, không làm méo chữ trên bìa; dùng một ảnh riêng cho hero (không lấy ảnh SEO ngẫu nhiên).

**Hiệu ứng:** fade-up chữ, mockup float biên độ 6–10px rất chậm; tắt theo reduced motion. Hạn chế hiệu ứng sao lấp lánh liên tục.

**Tiêu chí nghiệm thu:** H1 và CTA tương phản tốt; ảnh không bị crop mất sách; trên mobile không tạo scroll ngang.

### 4.3. Tác giả — `#tac-gia`

**Background:** Ivory, chữ Ink, heading Royal.

**Bố cục:** 2 cột ảnh chân dung Paulo Coelho và nội dung giới thiệu. Ảnh bo góc, đường viền vàng mảnh; mobile ảnh trước, copy sau.

**Nội dung:** tiểu sử ngắn khoảng 120–180 từ: nhà văn Brazil, phong cách viết chiêm nghiệm, tác phẩm tiêu biểu và ý nghĩa của Nhà Giả Kim. Mọi thông tin lịch sử/xuất bản cần đối chiếu nguồn trước khi đăng.

**CTA:** `SỞ HỮU NHÀ GIẢ KIM` đặt cuối section, dùng màu tím hoặc Gold theo nền và mức tương phản.

**Tiêu chí nghiệm thu:** ảnh chân dung có giấy phép sử dụng; text không quá dài; CTA mở đúng modal.

### 4.4. Truyền thông & Video — `#truyen-thong`

**Background:** xen kẽ Sand/Ivory hoặc một khối Royal nhấn nhẹ.

**Bố cục:**

1. Heading: `Nhà Giả Kim qua góc nhìn truyền thông`.
2. **Featured video** tỷ lệ 16:9, thumbnail thực tế, nút Play lớn, tiêu đề và nguồn.
3. **Media cards**: desktop 3 cột, tablet 2 cột, mobile 1 cột; gồm ảnh, tên báo/kênh, tiêu đề, mô tả ngắn và liên kết gốc.
4. CTA cuối: `KHÁM PHÁ CUỐN SÁCH NGAY`.

**Video:** ưu tiên click-to-load YouTube iframe; thumbnail trước khi người dùng nhấn. `loading="lazy"` cho media dưới fold. Chỉ đăng bài/video có thật, nêu đúng nguồn, không làm như được báo chí bảo chứng nếu không có căn cứ.

**Trường hợp không đủ bài báo:** dùng tiêu đề thay thế `Review và góc nhìn về Nhà Giả Kim`, hiển thị các video/nhận xét độc lập có nguồn.

**Tiêu chí nghiệm thu:** không có link chết, thumbnail đúng nội dung, đóng video không tiếp tục phát ngầm nếu modal video được dùng.

### 4.5. Đọc thử — `#doc-thu`

**Background:** Sand `#EAE0D2`, phần khung đọc trắng ngà, viền Gold tinh tế.

**Mục tiêu:** khuyến khích khách đọc thử và chuyển sang đặt mua.

**Dữ liệu:** chỉ dùng 4–6 trang được nhà xuất bản/chủ sở hữu quyền cho phép phổ biến, không tự scan/phát tán trang sách khi chưa được phép.

**Trải nghiệm:**

- Desktop: slider 2 trang cạnh nhau, gợi cảm giác quyển sách mở.
- Mobile: 1 trang/lượt, vuốt trái/phải.
- Điều khiển trước/sau, indicator `1 / N`, phóng to bằng lightbox nếu cần.
- Ảnh sắc nét, chữ đọc được; preload trang kế tiếp, lazy load trang chưa đến.
- CTA cuối: `MUA SÁCH ĐỂ ĐỌC TRỌN VẸN`.

**Triển khai:** JS module `preview.js` quản lý index, keyboard arrow khi focus trong vùng, nút disabled ở hai đầu; hỗ trợ touch/pointer. Cân nhắc Swiper chỉ nếu thật sự cần, còn lại Vanilla JS là đủ.

**Tiêu chí nghiệm thu:** trang không mờ, mobile vuốt tốt, không rò rỉ nội dung chưa được phép.

### 4.6. Đánh giá khách hàng — `#danh-gia`

**Background:** Ivory; các review card nền trắng hoặc Sand.

**Bố cục:**

- Heading `Độc giả nói gì về Nhà Giả Kim?`
- Rating summary: điểm trung bình (VD bố cục `4.8 / 5`), tổng số đánh giá, thanh phân bố 5/4/3/2/1 sao.
- Dưới là review cards; desktop 3 cột hoặc 2 cột tùy độ dài; mobile slider 1 card.
- Mỗi card: avatar/tên, điểm sao, ngày đánh giá, nội dung, nhãn `Đã mua hàng` **chỉ khi có xác minh**, ảnh thật nếu khách đồng ý.
- CTA cuối: `ĐẶT MUA NGAY HÔM NAY`.

**Nguồn dữ liệu:** chỉ dùng review thật. Không đăng con số rating minh họa, bình luận giả hoặc gắn nhãn verified khi chưa xác minh. Nếu chưa có review, thiết kế trạng thái trống thân thiện, không giả lập social proof.

**Tính toán:** trung bình = tổng `(rating × count)` chia tổng lượt đánh giá; phân bố phần trăm cũng lấy từ dữ liệu thật. Có thể lưu JSON ban đầu; khi có backend thì dùng API.

**SEO:** chỉ dùng schema rating/review nếu đáp ứng quy định dữ liệu có thể chứng minh và yêu cầu của công cụ tìm kiếm.

### 4.7. Footer

**Background:** `#180E2C`, chữ Ivory, tiêu đề Champagne.

**3 cột desktop:**

- Nhà sách: logo, giới thiệu, social.
- Liên kết nhanh: anchor section, chính sách giao hàng/đổi trả/bảo mật/thanh toán.
- Liên hệ: tên nhà sách, địa chỉ thực, số điện thoại `tel:`, email `mailto:`, giờ hoạt động nếu có.

Mobile xếp một cột; link có vùng chạm đủ lớn. Thêm copyright năm hiện tại bằng JS hoặc render server. Không tự bịa địa chỉ/số điện thoại.

---

## 5. Order Modal & Form — trung tâm chuyển đổi

### 5.1. Trigger

Tất cả nút có `data-open-order` gọi một hàm `openOrderModal()`; modal chỉ tồn tại **một instance** trong HTML. Các CTA ở Header, Hero, Tác giả, Media, Đọc thử, Review và sticky mobile phải mở cùng modal.

### 5.2. Giao diện

- Desktop: rộng tối đa 520–560px; overlay đen/tím trong suốt; modal bo góc 20px, shadow mềm; scroll nội bộ khi chiều cao viewport nhỏ.
- Mobile: full-screen dialog hoặc bottom sheet cao tối đa viewport, vùng nút submit dễ chạm; không che input khi bàn phím mở.
- Header modal: thumbnail bìa, `Nhà Giả Kim – Paulo Coelho`, giá bán thực tế và số lượng (mặc định 1).
- Đóng modal bằng nút X, Escape và có thể click overlay (nếu không làm mất dữ liệu quan trọng); focus được đưa vào dialog và trả lại nút kích hoạt khi đóng.

### 5.3. Trường dữ liệu

| Tên | Key | Kiểu | Bắt buộc | Validation |
|---|---|---|---|---|
| Họ và tên | `fullName` | text | Có | Trim, 2–100 ký tự, không chỉ khoảng trắng |
| Số điện thoại | `phone` | tel | Có | Chuẩn hóa định dạng số Việt Nam theo quy tắc cấu hình; xác minh phía server |
| Tỉnh/Thành phố | `province` | select | Có | Danh mục hiện hành |
| Phường/Xã | `ward` | select | Theo cấu hình | Phụ thuộc tỉnh/thành |
| Địa chỉ chi tiết | `addressLine` | textarea | Có | Số nhà, tên đường/tòa nhà, độ dài hợp lý |
| Số lượng | `quantity` | number/stepper | Có | Số nguyên >=1, không vượt tồn kho |
| Phương thức thanh toán | `paymentMethod` | radio | Có | `cod` / `bank_transfer` |
| Ghi chú | `note` | textarea | Không | Giới hạn ký tự |

**Thông tin đơn hàng cần thể hiện trước submit:** đơn giá, số lượng, tạm tính, phí giao hàng, tổng thanh toán, chính sách giao hàng và đổi trả. Nếu phí ship phụ thuộc địa chỉ, tính lại sau khi chọn nơi nhận.

### 5.4. Luồng người dùng

```text
Click CTA mua hàng
    ↓
Mở modal + hiển thị thông tin sách, giá, số lượng
    ↓
Điền khách hàng + địa chỉ + chọn COD / chuyển khoản
    ↓
Client-side validation + tính tổng
    ↓
POST dữ liệu đến backend API (server kiểm tra lại)
    ├─ Lỗi: thông báo rõ ràng, giữ thông tin đã nhập
    └─ Tạo đơn thành công: trả về orderId + trạng thái
        ├─ COD → Chờ xác nhận / xử lý giao hàng
        └─ Chuyển khoản → Hiện mã QR/thông tin chuyển khoản và mã đơn
                            → Xác nhận thanh toán qua đối soát/webhook
```

**Tuyệt đối:** không hiển thị `Đã thanh toán` khi khách chỉ vừa click xác nhận/chỉ xem mã QR. Không lưu thông tin khách hàng vào localStorage nếu không cần. Cần backend có kiểm tra dữ liệu, giới hạn tần suất, CSRF/CORS thích hợp, chống đơn trùng và xử lý lỗi mạng.

### 5.5. Ví dụ HTML semantic rút gọn

```html
<button type="button" data-open-order
  class="inline-flex min-h-12 items-center justify-center gap-2 rounded-xl bg-antique-gold px-7 py-3 font-semibold text-midnight transition hover:brightness-110 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-antique-gold">
  Mua sách ngay <i class="fa-solid fa-arrow-right" aria-hidden="true"></i>
</button>

<dialog id="order-dialog" aria-labelledby="order-title"
  class="w-[min(92vw,560px)] max-h-[90dvh] overflow-y-auto rounded-2xl p-0 backdrop:bg-[#130B25]/75">
  <div class="bg-ivory p-6">
    <div class="flex items-start justify-between gap-4">
      <h2 id="order-title" class="font-display text-2xl text-royal">Đặt mua Nhà Giả Kim</h2>
      <button type="button" data-close-order aria-label="Đóng form đặt hàng">✕</button>
    </div>
    <form id="order-form" class="mt-5 space-y-4" novalidate>
      <!-- Thumbnail, đơn giá, số lượng, thông tin nhận hàng -->
      <label class="block" for="customer-name">Họ và tên *</label>
      <input id="customer-name" name="fullName" autocomplete="name" required
        class="w-full rounded-xl border border-royal/20 bg-white px-4 py-3" />
      <!-- phone, province, ward, addressLine, paymentMethod, total -->
      <button type="submit" class="min-h-12 w-full rounded-xl bg-antique-gold font-semibold text-midnight">
        Xác nhận đặt hàng
      </button>
    </form>
  </div>
</dialog>
```

**Ví dụ JavaScript logic (minh họa, chưa phải module production):**

```js
const orderDialog = document.querySelector('#order-dialog');
let lastTrigger = null;

document.querySelectorAll('[data-open-order]').forEach((button) => {
  button.addEventListener('click', () => {
    lastTrigger = button;
    orderDialog.showModal();
  });
});

document.querySelector('[data-close-order]')?.addEventListener('click', () => {
  orderDialog.close();
});

orderDialog.addEventListener('close', () => lastTrigger?.focus());

// Submit: validate → set loading → POST /api/orders → xử lý trạng thái theo response.
// KHÔNG hardcode trả về thành công nếu API chưa xác nhận tạo đơn.
```

### 5.6. API contract đề xuất

`POST /api/orders` với dữ liệu:

```json
{
  "productId": "nha-gia-kim-edition-01",
  "quantity": 1,
  "customer": {
    "fullName": "Nguyen Van A",
    "phone": "09xxxxxxxx"
  },
  "shippingAddress": {
    "provinceCode": "...",
    "wardCode": "...",
    "addressLine": "..."
  },
  "paymentMethod": "cod",
  "note": ""
}
```

Server **tự lấy giá theo productId**, tự tính tổng, không tin giá client gửi. Ví dụ response khi tạo đơn:

```json
{
  "orderId": "ORD-EXAMPLE-001",
  "status": "pending_confirmation",
  "paymentStatus": "unpaid",
  "total": 0,
  "currency": "VND"
}
```

`total: 0` **chỉ là placeholder schema**, không phải giá sách. Khi triển khai phải trả số tiền thực tế; với chuyển khoản, tích hợp QR/thông tin thanh toán an toàn và đối soát.

### 5.7. CTA sticky mobile

- Thanh cố định dưới màn hình, chỉ hiện sau khi Hero CTA chính đã đi khỏi viewport (IntersectionObserver).
- Hiển thị `MUA NGAY` và có thể giá hiện hành; không tự tạo giá.
- Ẩn khi modal mở; tôn trọng `env(safe-area-inset-bottom)`.
- Không che nút khác, nội dung cuối trang hoặc thanh điều hướng điện thoại.

---

## 6. Cấu trúc thư mục đề xuất

```text
nha-gia-kim-landing/
├── index.html
├── package.json
├── package-lock.json
├── vite.config.js                # nếu dùng Vite
├── public/
│   ├── favicon.svg
│   ├── images/
│   │   ├── book-cover.webp
│   │   ├── book-hero-mockup.webp
│   │   ├── paulo-coelho.webp
│   │   ├── media/
│   │   ├── preview-pages/
│   │   └── reviews/
│   └── icons/
│       ├── desert-dunes.svg
│       ├── celestial-stars.svg
│       └── alchemist-sunburst.svg
├── src/
│   ├── styles/
│   │   ├── main.css              # Tailwind import + theme + global CSS
│   │   ├── components.css        # các component tái sử dụng
│   │   └── animations.css
│   ├── scripts/
│   │   ├── main.js
│   │   ├── navigation.js
│   │   ├── order-modal.js
│   │   ├── order-form.js
│   │   ├── preview.js
│   │   ├── reviews.js
│   │   └── media.js
│   └── data/
│       ├── product.js
│       ├── media.js
│       └── reviews.js
├── docs/
│   ├── content-checklist.md
│   └── api-contract.md
└── README.md
```

**Với site một trang**, có thể giữ markup trong `index.html`, dùng comment phân chia section; không cần tách nhiều file HTML trừ khi muốn template/partial. `product.js`, `media.js`, `reviews.js` chỉ là dữ liệu public hiển thị, không chứa API keys hay dữ liệu riêng tư.

---

## 7. Responsive rules

| Viewport | Header | Hero | Media | Preview | Review |
|---|---|---|---|---|---|
| Mobile < 640px | Drawer + CTA | 1 cột | 1 cột | 1 trang | 1 card |
| Tablet 640–1023px | Menu thu gọn | 1–2 cột | 2 cột | 1–2 trang | 2 card |
| Desktop >= 1024px | Full menu | 2 cột | 3 card | 2 trang | 3 card |

- Test viewport: **320, 375, 390, 768, 1024, 1280, 1440px**.
- Không có scroll ngang; ảnh `max-width: 100%`.
- Ẩn bớt decoration ở mobile thay vì ép thu nhỏ.
- Hỗ trợ thiết bị touch và keyboard; vùng click >=44×44px.
- Form với bàn phím điện thoại phải cuộn được, nút submit không bị khuất.

---

## 8. Hiệu năng, SEO, bảo mật, accessibility

### 8.1. Performance

- Hero image là LCP candidate: `fetchpriority="high"` cho ảnh phù hợp, preload khi cần; không lazy load ảnh Hero.
- Các ảnh còn lại `loading="lazy"`, `decoding="async"`, dùng `srcset`/`sizes` phù hợp.
- Nén WebP/AVIF với quality cân bằng; cân nhắc ảnh bìa có chữ cần giữ nét.
- Tối ưu font: 2 families, số weight vừa phải, `font-display: swap`.
- Video click-to-load, không nhúng nhiều iframe autoplay.
- Tailwind build production để loại utility không dùng; JS chia module, minify.
- Mục tiêu tham khảo: Core Web Vitals tốt (LCP <=2.5s, INP <=200ms, CLS <=0.1 tại percentile 75 dữ liệu thực tế).

### 8.2. SEO

- `<html lang="vi">`, title/mô tả duy nhất, Open Graph, favicon.
- H1 `Nhà Giả Kim – Paulo Coelho`, các H2 đúng cấu trúc.
- Canonical URL khi đã có domain thực.
- Product structured data (`Book`/`Product`, `Offer`) chỉ dùng dữ liệu thật: giá, availability, merchant, edition.
- Review/rating structured data chỉ khi hợp lệ và được xác minh.
- Link bài viết/video có tiêu đề mô tả; alt cho bìa/mockup.

### 8.3. Accessibility

- WCAG 2.2 AA làm mục tiêu: contrast cho text và controls, focus visible, semantic landmarks, modal focus trap/return focus.
- Form có label, thông báo lỗi gắn với input (`aria-describedby`), live region cho status.
- Rating có bản chữ (VD `4 trên 5 sao`) cho screen reader; icon sao không được là thông tin duy nhất.
- Không autoplay video có âm thanh, hỗ trợ `prefers-reduced-motion`.

### 8.4. Privacy & security

- HTTPS bắt buộc khi deploy; kiểm tra input server, rate limit, chống spam; xử lý CORS/CSRF theo kiến trúc.
- Chỉ gửi dữ liệu khách tới backend có quyền truy cập; không log tùy tiện số điện thoại và địa chỉ.
- Công bố chính sách quyền riêng tư, cách xử lý thông tin khách hàng.
- Thanh toán ngân hàng tích hợp qua nguồn tin cậy; thông tin QR cần đúng đơn hàng và có quy trình đối soát.
- Không hiển thị bí mật/token thanh toán trong mã frontend.

---

## 9. Lộ trình triển khai theo giai đoạn

### Phase 0 — Chuẩn bị nội dung và assets

- [ ] Xác nhận bìa/ấn bản, giá bán, tồn kho, chính sách ship và đổi trả.
- [ ] Chuẩn bị mockup đúng bìa sách, ảnh tác giả có quyền sử dụng.
- [ ] Tìm bài báo/video có nguồn xác minh, không tự bịa social proof.
- [ ] Xin phép/kiểm tra quyền đăng 4–6 trang đọc thử.
- [ ] Tổng hợp đánh giá khách hàng có thật.
- [ ] Cập nhật logo, tên nhà sách, địa chỉ, điện thoại, email.

**Deliverable:** thư mục assets chuẩn + nội dung đầy đủ, có nguồn.

### Phase 1 — Setup và design system

- [ ] Khởi tạo Vite + Tailwind v4 + Font Awesome.
- [ ] Tạo tokens màu/font/spacing/shadow/radius.
- [ ] Tạo reusable component styles: button, card, heading, form control, container.
- [ ] Chuẩn bị SVG motif.
- [ ] Test build và mobile baseline.

**Deliverable:** skeleton project + design system nhất quán.

### Phase 2 — Static layout

- [ ] Header + responsive navigation.
- [ ] Hero + mockup + CTA.
- [ ] Tác giả.
- [ ] Media/video cards.
- [ ] Preview UI.
- [ ] Rating summary + review cards.
- [ ] Footer.

**Deliverable:** giao diện hoàn chỉnh về hình thức, dùng dữ liệu có thật hoặc placeholder được đánh dấu rõ khi phát triển.

### Phase 3 — Tương tác frontend

- [ ] Smooth-scroll anchor và menu mobile.
- [ ] Video click-to-load.
- [ ] Preview slider/lightbox.
- [ ] Order dialog mở từ mọi CTA, đóng đúng và focus đúng.
- [ ] Form validation và loading/error/success states.
- [ ] Sticky mobile CTA thông minh.

**Deliverable:** luồng UI end-to-end vận hành, chưa giả vờ đặt hàng thật khi chưa có backend.

### Phase 4 — Backend & thanh toán

- [ ] `POST /api/orders` với validation server.
- [ ] Quản lý sản phẩm/giá/tồn kho nguồn server.
- [ ] Tính phí giao hàng và tổng tiền.
- [ ] COD: tạo đơn chờ xử lý.
- [ ] Chuyển khoản: mã đơn/QR, logic đối soát và trạng thái thanh toán.
- [ ] Gửi thông báo đơn cho nhà sách qua kênh phù hợp.
- [ ] Bảo vệ API khỏi spam/đơn trùng.

**Deliverable:** hệ thống tạo đơn thật, có xác nhận kết quả và xử lý lỗi.

### Phase 5 — QA, tối ưu, triển khai

- [ ] Responsive tất cả breakpoint.
- [ ] Test Chrome/Safari/Firefox, Android/iOS.
- [ ] Keyboard, focus, accessibility.
- [ ] Performance Lighthouse + kiểm tra network chậm.
- [ ] SEO metadata + structured data hợp lệ.
- [ ] QA toàn bộ CTA, form và thanh toán trên môi trường test.
- [ ] Deploy HTTPS, cấu hình domain, backup/log, monitor lỗi.

**Deliverable:** landing page sẵn sàng chạy chiến dịch bán sách.

---

## 10. Danh sách file/khối nên triển khai lần lượt

| Thứ tự | Khối/file chính | Hoàn thành khi |
|---|---|---|
| 1 | `main.css`, `components.css` | Màu, font, button, card, container thống nhất |
| 2 | `index.html` Header/Hero | Có mockup đúng bìa, title, CTA đẹp trên mobile/desktop |
| 3 | Author | Ảnh/tóm tắt căn chỉnh gọn, responsive |
| 4 | Media + `media.js` | Video chạy theo click, media cards đúng nguồn |
| 5 | Preview + `preview.js` | Chuyển trang và zoom mượt |
| 6 | Reviews + `reviews.js` | Rating thống kê đúng dữ liệu |
| 7 | Order modal + `order-form.js` | CTA đều mở cùng form, validate và xử lý states |
| 8 | API integration | Tạo đơn thật, không false success, xử lý COD/chuyển khoản |
| 9 | Footer & QA | Thông tin thật, accessibility, SEO, tốc độ hoàn chỉnh |

---

## 11. Acceptance criteria toàn landing page

1. **Visual:** bám màu phương án A; nhìn sang trọng nhưng đọc dễ; bìa đúng phiên bản đã cung cấp.
2. **Structure:** đủ 5 section do chủ dự án yêu cầu, Header và Footer; không tự ý thêm nội dung không có nguồn.
3. **CTA:** có CTA ở Hero và các section chính; mọi nút mua đều mở **một modal chung**.
4. **Form:** đủ họ tên, số điện thoại, địa chỉ, phương thức thanh toán; có giá/tổng tiền, validation, loading, lỗi và kết quả thực tế.
5. **Payment:** hỗ trợ lựa chọn COD/chuyển khoản; trạng thái đơn và thanh toán không bị đánh đồng.
6. **Responsive:** hiển thị tốt 320–1440px, không scroll ngang hoặc che CTA.
7. **Accessibility:** điều hướng bàn phím, focus/modal, contrast và labels đạt chuẩn mục tiêu.
8. **Media:** chỉ đăng bài/video/đánh giá thật; trang đọc thử có quyền sử dụng.
9. **Performance:** ảnh tối ưu, không auto-load nặng video; không gây layout shift đáng kể.
10. **Maintainability:** tách dữ liệu và UI, có cấu trúc module dễ bổ sung.

---

## 12. Những thông tin cần chốt trước khi code chức năng bán hàng

- **Tên/logo nhà sách** và thông tin liên hệ đầy đủ.
- **Giá bán thực tế**, giảm giá (nếu có), số lượng tồn.
- **Phí ship** theo khu vực, COD có áp dụng toàn quốc không.
- **Tài khoản nhận chuyển khoản / cổng thanh toán**, quy trình đối soát.
- **Phương án backend**: Node.js, PHP/Laravel, Supabase, Firebase hoặc API hiện có (chọn theo hạ tầng; không tự giả định).
- **Nguồn đánh giá thật**, giấy phép ảnh tác giả, video và trang đọc thử.

> **Nguyên tắc cuối:** ưu tiên giao diện kể chuyện giàu cảm xúc, nhưng mọi thông tin thương mại phải chính xác; giữ trải nghiệm đặt hàng ít bước và tuyệt đối không báo thành công trước khi backend xác nhận.
