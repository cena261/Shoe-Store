
class Cart {
  constructor() {
    this.items = this.loadCart();
    this.isOpen = false;
  }

  loadCart() {
    const saved = localStorage.getItem('cart');
    return saved ? JSON.parse(saved) : [];
  }

  saveCart() {
    localStorage.setItem('cart', JSON.stringify(this.items));
    this.updateUI();
  }

  addItem(product, size) {
    const existingIndex = this.items.findIndex(
      item => item.id === product.id && item.size === size
    );

    if (existingIndex !== -1) {
      this.items[existingIndex].quantity += 1;
    } else {
      this.items.push({
        ...product,
        size: size,
        quantity: 1
      });
    }

    this.saveCart();
    this.showNotification('Added to cart!');
  }

  removeItem(productId, size) {
    this.items = this.items.filter(
      item => !(item.id === productId && item.size === size)
    );
    this.saveCart();
  }

  updateQuantity(productId, size, quantity) {
    const item = this.items.find(
      item => item.id === productId && item.size === size
    );
    if (item) {
      item.quantity = Math.max(1, quantity);
      this.saveCart();
    }
  }

  clearCart() {
    if (confirm('Are you sure you want to clear your cart?')) {
      this.items = [];
      this.saveCart();
    }
  }

  getTotal() {
    return this.items.reduce((total, item) => {
      const price = item.promotionPrice || item.price;
      return total + (price * item.quantity);
    }, 0);
  }

  getItemCount() {
    return this.items.length;
  }

  toggleCart() {
    this.isOpen = !this.isOpen;
    const cartSidebar = document.getElementById('cart-sidebar');
    const cartOverlay = document.getElementById('cart-overlay');

    if (this.isOpen) {
      cartSidebar.classList.add('open');
      cartOverlay.classList.add('open');
      document.body.style.overflow = 'hidden';
    } else {
      cartSidebar.classList.remove('open');
      cartOverlay.classList.remove('open');
      document.body.style.overflow = '';
    }
  }

  updateUI() {
    this.updateCartBadge();
    this.updateCartSidebar();
  }

  updateCartBadge() {
    const badge = document.getElementById('cart-badge');
    const count = this.getItemCount();

    if (badge) {
      if (count > 0) {
        badge.textContent = count;
        badge.style.display = 'flex';
      } else {
        badge.style.display = 'none';
      }
    }
  }

  updateCartSidebar() {
    const cartProducts = document.getElementById('cart-products');
    const cartTotal = document.getElementById('cart-total');

    if (!cartProducts || !cartTotal) return;

    if (this.items.length === 0) {
      cartProducts.innerHTML = '<p style="text-align: center; color: #6b7280;">Your cart is empty</p>';
    } else {
      cartProducts.innerHTML = this.items.map(item => `
        <div class="cart-product">
          <img src="${item.images[0]}" alt="${item.name}" class="cart-product-image">
          <div class="cart-product-details">
            <h4 class="cart-product-name">${item.name}</h4>
            <p class="cart-product-size">Size: ${item.size}</p>
            <p class="cart-product-price">€${item.promotionPrice || item.price}</p>
            <div class="cart-product-quantity">
              <button onclick="cart.updateQuantity('${item.id}', '${item.size}', ${item.quantity - 1})">-</button>
              <span>${item.quantity}</span>
              <button onclick="cart.updateQuantity('${item.id}', '${item.size}', ${item.quantity + 1})">+</button>
            </div>
          </div>
          <button class="cart-product-remove" onclick="cart.removeItem('${item.id}', '${item.size}')" aria-label="Remove">
            ✕
          </button>
        </div>
      `).join('');
    }

    cartTotal.textContent = `€${this.getTotal()}`;
  }

  showNotification(message) {
    const notification = document.createElement('div');
    notification.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      background: #10b981;
      color: white;
      padding: 1rem 1.5rem;
      border-radius: 0.5rem;
      z-index: 9999;
      animation: slideIn 0.3s ease;
    `;
    notification.textContent = message;
    document.body.appendChild(notification);

    setTimeout(() => {
      notification.style.animation = 'slideOut 0.3s ease';
      setTimeout(() => notification.remove(), 300);
    }, 2000);
  }
}

const cart = new Cart();

class FilterManager {
  constructor() {
    this.filters = {
      gender: { men: false, women: false, unisex: false },
      kids: false,
      colors: [],
      priceRange: { min: 0, max: 1000 },
      sortBy: 'newest'
    };
  }

  applyFilters(products) {
    let filtered = [...products];

    const activeGenders = Object.keys(this.filters.gender).filter(
      key => this.filters.gender[key]
    );
    if (activeGenders.length > 0) {
      filtered = filtered.filter(p => activeGenders.includes(p.gender));
    }

    if (this.filters.kids) {
      filtered = filtered.filter(p => p.category === 'kids');
    }

    if (this.filters.colors.length > 0) {
      filtered = filtered.filter(p =>
        p.colors.some(color => this.filters.colors.includes(color))
      );
    }

    filtered = filtered.filter(p => {
      const price = p.promotionPrice || p.price;
      return price >= this.filters.priceRange.min && price <= this.filters.priceRange.max;
    });

    filtered = this.sortProducts(filtered);

    return filtered;
  }

  sortProducts(products) {
    const sorted = [...products];
    switch (this.filters.sortBy) {
      case 'price-low':
        return sorted.sort((a, b) => (a.promotionPrice || a.price) - (b.promotionPrice || b.price));
      case 'price-high':
        return sorted.sort((a, b) => (b.promotionPrice || b.price) - (a.promotionPrice || a.price));
      case 'name':
        return sorted.sort((a, b) => a.name.localeCompare(b.name));
      default:
        return sorted;
    }
  }

  toggleGender(gender) {
    this.filters.gender[gender] = !this.filters.gender[gender];
  }

  toggleColor(color) {
    const index = this.filters.colors.indexOf(color);
    if (index > -1) {
      this.filters.colors.splice(index, 1);
    } else {
      this.filters.colors.push(color);
    }
  }

  setKidsFilter(value) {
    this.filters.kids = value;
  }

  setSortBy(sortBy) {
    this.filters.sortBy = sortBy;
  }

  reset() {
    this.filters = {
      gender: { men: false, women: false, unisex: false },
      kids: false,
      colors: [],
      priceRange: { min: 0, max: 1000 },
      sortBy: 'newest'
    };
  }
}

function toggleMobileMenu() {
  const menu = document.getElementById('mobile-menu');
  if (menu) {
    menu.classList.toggle('open');
    document.body.style.overflow = menu.classList.contains('open') ? 'hidden' : '';
  }
}

class ProductGallery {
  constructor(images) {
    this.images = images;
    this.currentIndex = 0;
  }

  next() {
    this.currentIndex = (this.currentIndex + 1) % this.images.length;
    this.updateGallery();
  }

  prev() {
    this.currentIndex = (this.currentIndex - 1 + this.images.length) % this.images.length;
    this.updateGallery();
  }

  goTo(index) {
    this.currentIndex = index;
    this.updateGallery();
  }

  updateGallery() {
    const galleryImages = document.querySelectorAll('.product-gallery-image');
    const thumbnails = document.querySelectorAll('.product-gallery-thumbnail');

    galleryImages.forEach((img, i) => {
      img.classList.toggle('active', i === this.currentIndex);
    });

    thumbnails.forEach((thumb, i) => {
      thumb.classList.toggle('active', i === this.currentIndex);
    });
  }
}

function validateEmail(email) {
  const re = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;
  return re.test(email);
}

function validateForm(formId) {
  const form = document.getElementById(formId);
  if (!form) return false;

  const inputs = form.querySelectorAll('input[required]');
  let isValid = true;

  inputs.forEach(input => {
    const errorElement = document.getElementById(`${input.name}-error`);
    let error = '';

    if (!input.value || input.value.trim().length <= 2) {
      error = 'This field is required';
      isValid = false;
    } else if (input.type === 'email' && !validateEmail(input.value)) {
      error = 'Invalid email address';
      isValid = false;
    }

    if (errorElement) {
      errorElement.textContent = error;
    }
  });

  return isValid;
}

function smoothScroll(target) {
  const element = document.querySelector(target);
  if (element) {
    element.scrollIntoView({ behavior: 'smooth' });
  }
}

async function loadProducts() {
  try {
    const response = await fetch('data/products.json');
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error loading products:', error);
    return { products: [], collections: [] };
  }
}

function renderProducts(products, containerId) {
  const container = document.getElementById(containerId);
  if (!container) return;

  container.innerHTML = products.map(product => `
    <a href="product-details.html?id=${product.id}" class="product-card">
      <div class="product-image-container">
        <img src="${product.images[0]}" alt="${product.name}" class="product-image">
      </div>
      <div class="product-info">
        <div class="product-info-left">
          <h4>${product.name}</h4>
          <h5>${product.category.charAt(0).toUpperCase() + product.category.slice(1)}</h5>
        </div>
        <div class="product-info-right">
          <h4>€${product.promotionPrice || product.price}</h4>
          ${product.promotionPrice ? `<h5>€${product.price}</h5>` : ''}
        </div>
      </div>
    </a>
  `).join('');
}

function getUrlParameter(name) {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get(name);
}

document.addEventListener('DOMContentLoaded', function() {
  cart.updateUI();

  const cartOverlay = document.getElementById('cart-overlay');
  if (cartOverlay) {
    cartOverlay.addEventListener('click', () => cart.toggleCart());
  }

  const mobileMenu = document.getElementById('mobile-menu');
  if (mobileMenu) {
    mobileMenu.addEventListener('click', (e) => {
      if (e.target === mobileMenu) {
        toggleMobileMenu();
      }
    });
  }
});

const style = document.createElement('style');
style.textContent = `
  @keyframes slideIn {
    from {
      transform: translateX(100%);
      opacity: 0;
    }
    to {
      transform: translateX(0);
      opacity: 1;
    }
  }

  @keyframes slideOut {
    from {
      transform: translateX(0);
      opacity: 1;
    }
    to {
      transform: translateX(100%);
      opacity: 0;
    }
  }
`;
document.head.appendChild(style);
