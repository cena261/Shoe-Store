
function formatCurrency(amount) {
  if (!amount && amount !== 0) return '0₫';
  const rounded = Math.round(amount);
  return rounded.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.') + '₫';
}

class Cart {
  constructor() {
    this.items = [];
    this.isOpen = false;
    this.isLoggedIn = false;
    this.syncInProgress = false;
    this.ready = false;
    this.readyPromise = this.init();
  }

  async init() {
    const user = this.getUser();

    if (user && user.loggedIn === true) {
      const sessionValid = await this.validateSession();

      if (sessionValid) {
        this.isLoggedIn = true;
        const guestCart = localStorage.getItem('cart');
        const hasGuestItems = guestCart && JSON.parse(guestCart).length > 0;

        if (hasGuestItems) {
          this.loadFromLocalStorage();
          await this.syncToServer();
          await this.loadFromServer();
        } else {
          await this.loadFromServer();
        }
      } else {
        this.isLoggedIn = false;
        localStorage.removeItem('user');
        this.loadFromLocalStorage();
      }
    } else {
      this.isLoggedIn = false;
      this.loadFromLocalStorage();
    }

    this.updateUI();
    this.ready = true;
  }

  async waitReady() {
    await this.readyPromise;
  }

  async validateSession() {
    try {
      const response = await fetch('/Account/GetUserInfo');
      const result = await response.json();
      return result.Status === true;
    } catch (error) {
      if (typeof logger !== 'undefined') {
        logger.error('Session validation failed', { error: error.message });
      }
      return false;
    }
  }

  getUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  loadFromLocalStorage() {
    const saved = localStorage.getItem('cart');
    this.items = saved ? JSON.parse(saved) : [];
  }

  async loadFromServer() {
    try {
      const user = this.getUser();
      if (!user) return;

      const response = await fetch('/api/Cart');

      const result = await response.json();
      if (result.Status && result.Cart) {
        this.items = result.Cart.Items.map(item => ({
          id: item.ProductID,
          variantId: item.VariantID,
          name: item.ProductName,
          slug: item.Slug,
          styleColor: item.StyleColor,
          colorName: item.ColorName,
          size: item.Size,
          price: item.Price,
          promotionPrice: item.PromotionPrice,
          quantity: item.Quantity,
          stockQty: item.StockQty,
          images: [item.ImageURL]
        }));
      }
    } catch (error) {
      if (typeof logger !== 'undefined') {
        logger.error('Failed to load cart from server', { error: error.message });
      }
      this.loadFromLocalStorage();
    }
  }

  async saveCart() {
    localStorage.setItem('cart', JSON.stringify(this.items));

    if (this.isLoggedIn && !this.syncInProgress) {
      await this.syncToServer();
    }

    this.updateUI();
  }

  async syncToServer() {
    try {
      const user = this.getUser();
      if (!user) {
        return;
      }

      this.syncInProgress = true;

      const syncItems = this.items.map(item => ({
        VariantID: item.variantId,
        Quantity: item.quantity
      }));

      const response = await fetch('/api/Cart/sync', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(syncItems)
      });

      if (!response.ok) {
        const errorText = await response.text();
        if (typeof logger !== 'undefined') {
          logger.error('Sync failed', { status: response.status, error: errorText });
        }
      }

      this.syncInProgress = false;
    } catch (error) {
      if (typeof logger !== 'undefined') {
        logger.error('Failed to sync cart to server', { error: error.message });
      }
      this.syncInProgress = false;
    }
  }

  async migrateGuestCart() {
    const guestCart = localStorage.getItem('cart');
    if (!guestCart) return;

    const guestItems = JSON.parse(guestCart);
    if (guestItems.length === 0) {
      localStorage.removeItem('cart');
      return;
    }

    const user = this.getUser();
    if (!user) return;

    await new Promise(resolve => setTimeout(resolve, 500));

    try {
      const syncItems = guestItems.map(item => ({
        VariantID: item.variantId,
        Quantity: item.quantity
      }));

      const response = await fetch('/api/Cart/sync', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(syncItems)
      });

      if (response.ok) {
        const result = await response.json();
        if (result.Status) {
          localStorage.removeItem('cart');
          await this.loadFromServer();
          toast.success('Your cart has been synced!');
        }
      }
    } catch (error) {
      if (typeof logger !== 'undefined') {
        logger.error('Failed to migrate guest cart', { error: error.message });
      }
    }
  }

  onLogout() {
    localStorage.removeItem('cart');
    this.items = [];
    this.isLoggedIn = false;
    this.updateUI();
  }

  addItem(product, size) {
    const existingIndex = this.items.findIndex(
      item => item.variantId === product.variantId
    );

    if (existingIndex !== -1) {
      if (this.items[existingIndex].quantity < product.stockQty) {
        this.items[existingIndex].quantity += 1;
      } else {
        toast.warning('Cannot add more. Stock limit reached.');
        return;
      }
    } else {
      this.items.push(product);
    }

    this.saveCart();
    toast.success('Product added to bag!');
  }

  removeItem(variantId) {
    this.items = this.items.filter(item => item.variantId !== variantId);
    this.saveCart();
  }

  updateQuantity(variantId, quantity) {
    const item = this.items.find(item => item.variantId === variantId);
    if (item) {
      if (quantity <= 0) {
        this.removeItem(variantId);
      } else if (quantity > item.stockQty) {
        toast.warning('Stock limit reached');
        item.quantity = item.stockQty;
        this.saveCart();
      } else {
        item.quantity = quantity;
        this.saveCart();
      }
    }
  }

  clearCart() {
    this.items = [];
    this.saveCart();
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
    const checkoutBtn = document.getElementById('checkout-btn');

    if (!cartProducts || !cartTotal) return;

    if (this.items.length === 0) {
      cartProducts.innerHTML = '<p style="text-align: center; color: #6b7280;">Your cart is empty</p>';
      if (checkoutBtn) {
        checkoutBtn.style.pointerEvents = 'none';
        checkoutBtn.style.opacity = '0.5';
      }
    } else {
      cartProducts.innerHTML = this.items.map(item => `
        <div class="cart-product">
          <img src="${item.images[0]}" alt="${item.name}" class="cart-product-image">
          <div class="cart-product-details">
            <h4 class="cart-product-name">${item.name}</h4>
            <p class="cart-product-size">${item.colorName} - Size: EU ${item.size}</p>
            <p class="cart-product-price">${formatCurrency(item.promotionPrice || item.price)}</p>
            <div class="cart-product-quantity">
              <button onclick="cart.updateQuantity(${item.variantId}, ${item.quantity - 1})">-</button>
              <span>${item.quantity}</span>
              <button onclick="cart.updateQuantity(${item.variantId}, ${item.quantity + 1})">+</button>
            </div>
          </div>
          <button class="cart-product-remove" onclick="cart.removeItem(${item.variantId})" aria-label="Remove">
            ✕
          </button>
        </div>
      `).join('');
      if (checkoutBtn) {
        checkoutBtn.style.pointerEvents = 'auto';
        checkoutBtn.style.opacity = '1';
      }
    }

    cartTotal.textContent = formatCurrency(this.getTotal());
  }
}

const cart = new Cart();

function toggleMobileMenu() {
  const menu = document.getElementById('mobile-menu');
  if (menu) {
    menu.classList.toggle('open');
    document.body.style.overflow = menu.classList.contains('open') ? 'hidden' : '';
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
