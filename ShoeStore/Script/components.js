function renderNavBar(currentPage = '') {
  return `
    <nav>
      <h2>
        <a href="index.html">Logo</a>
      </h2>
      <div class="nav-links">
        <a href="shoes.html" class="nav-item" onclick="filterByGender('men'); return false;">Men</a>
        <a href="shoes.html" class="nav-item" onclick="filterByGender('women'); return false;">Women</a>
        <a href="shoes.html" class="nav-item" onclick="filterByGender('kids'); return false;">Kids</a>
        <a href="shoes.html" class="nav-item" onclick="filterByGender('unisex'); return false;">Unisex</a>
      </div>
      <div class="nav-icons">
        <a href="register.html">
          <button class="btn-icon" aria-label="Account">
            <svg stroke="currentColor" fill="currentColor" stroke-width="0" viewBox="0 0 1024 1024" height="1em" width="1em" xmlns="http://www.w3.org/2000/svg"><path d="M858.5 763.6a374 374 0 0 0-80.6-119.5 375.63 375.63 0 0 0-119.5-80.6c-.4-.2-.8-.3-1.2-.5C719.5 518 760 444.7 760 362c0-137-111-248-248-248S264 225 264 362c0 82.7 40.5 156 102.8 201.1-.4.2-.8.3-1.2.5-44.8 18.9-85 46-119.5 80.6a375.63 375.63 0 0 0-80.6 119.5A371.7 371.7 0 0 0 136 901.8a8 8 0 0 0 8 8.2h60c4.4 0 7.9-3.5 8-7.8 2-77.2 33-149.5 87.8-204.3 56.7-56.7 132-87.9 212.2-87.9s155.5 31.2 212.2 87.9C779 752.7 810 825 812 902.2c.1 4.4 3.6 7.8 8 7.8h60a8 8 0 0 0 8-8.2c-1-47.8-10.9-94.3-29.5-138.2zM512 534c-45.9 0-89.1-17.9-121.6-50.4S340 407.9 340 362c0-45.9 17.9-89.1 50.4-121.6S466.1 190 512 190s89.1 17.9 121.6 50.4S684 316.1 684 362c0 45.9-17.9 89.1-50.4 121.6S557.9 534 512 534z"></path></svg>
          </button>
        </a>
        <button class="btn-icon" onclick="cart.toggleCart()" aria-label="Cart" style="position: relative; margin-left: 0.75rem;">
          <span id="cart-badge" class="cart-badge" style="display: none;">0</span>
          <svg stroke="currentColor" fill="currentColor" stroke-width="0" viewBox="0 0 1024 1024" height="1em" width="1em" xmlns="http://www.w3.org/2000/svg"><path d="M922.9 701.9H327.4l29.9-60.9 496.8-.9c16.8 0 31.2-12 34.2-28.6l68.8-385.1c1.8-10.1-.9-20.5-7.5-28.4a34.99 34.99 0 0 0-26.6-12.5l-632-2.1-5.4-25.4c-3.4-16.2-18-28-34.6-28H96.5a35.3 35.3 0 1 0 0 70.6h125.9L246 312.8l58.1 281.3-74.8 122.1a34.96 34.96 0 0 0-3 36.8c6 11.9 18.1 19.4 31.5 19.4h62.8a102.43 102.43 0 0 0-20.6 61.7c0 56.6 46 102.6 102.6 102.6s102.6-46 102.6-102.6c0-22.3-7.4-44-20.6-61.7h161.1a102.43 102.43 0 0 0-20.6 61.7c0 56.6 46 102.6 102.6 102.6s102.6-46 102.6-102.6c0-22.3-7.4-44-20.6-61.7H923c19.4 0 35.3-15.8 35.3-35.3a35.42 35.42 0 0 0-35.4-35.2zM305.7 253l575.8 1.9-56.4 315.8-452.3.8L305.7 253zm96.9 612.7c-17.4 0-31.6-14.2-31.6-31.6 0-17.4 14.2-31.6 31.6-31.6s31.6 14.2 31.6 31.6a31.6 31.6 0 0 1-31.6 31.6zm325.1 0c-17.4 0-31.6-14.2-31.6-31.6 0-17.4 14.2-31.6 31.6-31.6s31.6 14.2 31.6 31.6a31.6 31.6 0 0 1-31.6 31.6z"></path></svg>
        </button>
        <button class="btn-icon mobile-menu-button" onclick="toggleMobileMenu()" aria-label="Menu" style="margin-left: 0.75rem; display: inline-flex;">
          <svg stroke="currentColor" fill="currentColor" stroke-width="0" viewBox="0 0 1024 1024" height="1em" width="1em" xmlns="http://www.w3.org/2000/svg"><path d="M904 160H120c-4.4 0-8 3.6-8 8v64c0 4.4 3.6 8 8 8h784c4.4 0 8-3.6 8-8v-64c0-4.4-3.6-8-8-8zm0 624H120c-4.4 0-8 3.6-8 8v64c0 4.4 3.6 8 8 8h784c4.4 0 8-3.6 8-8v-64c0-4.4-3.6-8-8-8zm0-312H120c-4.4 0-8 3.6-8 8v64c0 4.4 3.6 8 8 8h784c4.4 0 8-3.6 8-8v-64c0-4.4-3.6-8-8-8z"></path></svg>
        </button>
      </div>
    </nav>
  `;
}

function renderMobileMenu() {
  const user = JSON.parse(localStorage.getItem('user') || 'null');
  const isLoggedIn = user && user.loggedIn;

  return `
    <div class="nav-menu" id="mobile-menu">
      <div class="nav-menu-content">
        <div class="nav-menu-close">
          <button class="btn-icon" onclick="toggleMobileMenu()" aria-label="Close menu">
            <svg stroke="currentColor" fill="black" stroke-width="0" viewBox="0 0 1024 1024" height="1.5em" width="1.5em" xmlns="http://www.w3.org/2000/svg"><path d="M563.8 512l262.5-312.9c4.4-5.2.7-13.1-6.1-13.1h-79.8c-4.7 0-9.2 2.1-12.3 5.7L511.6 449.8 295.1 191.7c-3-3.6-7.5-5.7-12.3-5.7H203c-6.8 0-10.5 7.9-6.1 13.1L459.4 512 196.9 824.9A7.95 7.95 0 0 0 203 838h79.8c4.7 0 9.2-2.1 12.3-5.7l216.5-258.1 216.5 258.1c3 3.6 7.5 5.7 12.3 5.7h79.8c6.8 0 10.5-7.9 6.1-13.1L563.8 512z"></path></svg>
          </button>
        </div>
        <div class="nav-menu-links">
          <a href="shoes.html" onclick="filterByGender('men')">Men</a>
          <a href="shoes.html" onclick="filterByGender('women')">Women</a>
          <a href="shoes.html" onclick="filterByGender('kids')">Kids</a>
          <a href="shoes.html" onclick="filterByGender('unisex')">Unisex</a>
          ${isLoggedIn
            ? '<a href="account.html">Account</a><a href="#" onclick="logoutFromMenu(); return false;">Logout</a>'
            : '<a href="register.html">Account</a>'
          }
        </div>
      </div>
    </div>
  `;
}

function renderFooter() {
  return `
    <footer>
      <div style="margin-bottom: 0.5rem; height: 1px; width: 100%; background-color: #f3f4f6;"></div>
      <h3 style="margin-bottom: -0.5rem;">Sport,</h3>
      <h3>the smart choice.</h3>
      <h6>
        Shop by <a href="https://github.com/kriziu" target="_blank" rel="noreferrer">kriziu</a>,
        all product images from <a href="https://nike.com" target="_blank" rel="noreferrer">nike.com</a>
      </h6>
    </footer>
  `;
}

function renderCart() {
  return `
    <!-- Cart Overlay -->
    <div class="cart-overlay" id="cart-overlay" onclick="cart.toggleCart()"></div>

    <!-- Cart Sidebar -->
    <div class="cart-sidebar" id="cart-sidebar">
      <div class="cart-header">
        <div class="cart-header-left">
          <h1>Cart (<span id="cart-count">0</span>)</h1>
          <button class="cart-clear" onclick="cart.clearCart()">(Clear cart)</button>
        </div>
        <button class="btn-icon" onclick="cart.toggleCart()">
          <svg stroke="currentColor" fill="currentColor" stroke-width="0" viewBox="0 0 1024 1024" height="1em" width="1em" xmlns="http://www.w3.org/2000/svg"><path d="M563.8 512l262.5-312.9c4.4-5.2.7-13.1-6.1-13.1h-79.8c-4.7 0-9.2 2.1-12.3 5.7L511.6 449.8 295.1 191.7c-3-3.6-7.5-5.7-12.3-5.7H203c-6.8 0-10.5 7.9-6.1 13.1L459.4 512 196.9 824.9A7.95 7.95 0 0 0 203 838h79.8c4.7 0 9.2-2.1 12.3-5.7l216.5-258.1 216.5 258.1c3 3.6 7.5 5.7 12.3 5.7h79.8c6.8 0 10.5-7.9 6.1-13.1L563.8 512z"></path></svg>
        </button>
      </div>

      <div class="cart-products" id="cart-products">
        <p style="text-align: center; color: #6b7280;">Your cart is empty</p>
      </div>

      <div class="cart-footer">
        <div class="cart-total">
          <h3>Total: <span id="cart-total">€0</span></h3>
          <h4>
            <svg stroke="currentColor" fill="currentColor" stroke-width="0" viewBox="0 0 24 24" height="1em" width="1em" xmlns="http://www.w3.org/2000/svg" style="margin-bottom: -2px; margin-right: 4px;"><path d="M22 8a.76.76 0 0 0 0-.21v-.08a.77.77 0 0 0-.07-.16.35.35 0 0 0-.05-.08l-.1-.13-.08-.06-.12-.09-9-5a1 1 0 0 0-1 0l-9 5-.09.07-.11.08a.41.41 0 0 0-.07.11.39.39 0 0 0-.08.1.59.59 0 0 0-.06.14.3.3 0 0 0 0 .1A.76.76 0 0 0 2 8v8a1 1 0 0 0 .52.87l9 5a.75.75 0 0 0 .13.06h.1a1.06 1.06 0 0 0 .5 0h.1l.14-.06 9-5A1 1 0 0 0 22 16V8zm-10 3.87L5.06 8l2.76-1.52 6.83 3.9zm0-7.72L18.94 8 16.7 9.25 9.87 5.34zM4 9.7l7 3.92v5.68l-7-3.89zm9 9.6v-5.68l3-1.68V15l2-1v-3.18l2-1.11v5.7z"></path></svg>
            Free shipping
          </h4>
        </div>
        <a href="checkout.html" class="btn cart-checkout" onclick="cart.toggleCart()">Checkout</a>
      </div>
    </div>
  `;
}

function filterByGender(gender) {
  localStorage.setItem('activeFilter', JSON.stringify({ type: 'gender', value: gender }));
  window.location.href = 'shoes.html';
}

function logoutFromMenu() {
  showLogoutConfirm();
}

function showLogoutConfirm() {
  const overlay = document.createElement('div');
  overlay.className = 'logout-modal-overlay';
  overlay.setAttribute('role', 'dialog');
  overlay.setAttribute('aria-modal', 'true');
  overlay.setAttribute('aria-labelledby', 'logout-modal-title');
  overlay.style.cssText = 'position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 10000;';

  const modal = document.createElement('div');
  modal.style.cssText = 'background: white; padding: 2rem; border-radius: 8px; max-width: 400px; width: 90%; box-shadow: 0 4px 6px rgba(0,0,0,0.1);';
  modal.innerHTML = `
    <h3 id="logout-modal-title" style="margin: 0 0 1rem 0; color: #1f2937; font-size: 1.25rem;">Confirm Logout</h3>
    <p style="margin: 0 0 1.5rem 0; color: #6b7280;">Are you sure you want to logout?</p>
    <div style="display: flex; gap: 0.75rem; justify-content: flex-end;">
      <button id="logout-cancel-btn" style="padding: 0.5rem 1rem; border: 1px solid #d1d5db; background: white; border-radius: 4px; cursor: pointer; color: #374151;">Cancel</button>
      <button id="logout-confirm-btn" style="padding: 0.5rem 1rem; border: none; background: #ef4444; color: white; border-radius: 4px; cursor: pointer;">Logout</button>
    </div>
  `;

  overlay.appendChild(modal);
  document.body.appendChild(overlay);

  const cancelBtn = document.getElementById('logout-cancel-btn');
  const confirmBtn = document.getElementById('logout-confirm-btn');
  const focusableElements = [cancelBtn, confirmBtn];
  let currentFocusIndex = 0;

  const closeModal = () => {
    document.body.removeChild(overlay);
    document.removeEventListener('keydown', handleKeyDown);
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Escape') {
      e.preventDefault();
      closeModal();
    } else if (e.key === 'Tab') {
      e.preventDefault();
      currentFocusIndex = e.shiftKey
        ? (currentFocusIndex - 1 + focusableElements.length) % focusableElements.length
        : (currentFocusIndex + 1) % focusableElements.length;
      focusableElements[currentFocusIndex].focus();
    } else if (e.key === 'Enter' && document.activeElement === confirmBtn) {
      e.preventDefault();
      confirmBtn.click();
    }
  };

  cancelBtn.addEventListener('click', closeModal);

  overlay.addEventListener('click', (e) => {
    if (e.target === overlay) {
      closeModal();
    }
  });

  confirmBtn.addEventListener('click', () => {
    localStorage.removeItem('user');
    closeModal();
    toggleMobileMenu();
    window.location.href = 'login.html';
  });

  document.addEventListener('keydown', handleKeyDown);

  cancelBtn.focus();
}

function initializeComponents() {
  const navbarContainer = document.getElementById('navbar-container');
  if (navbarContainer) {
    navbarContainer.innerHTML = renderNavBar();
  }

  const mobileMenuContainer = document.getElementById('mobile-menu-container');
  if (mobileMenuContainer) {
    mobileMenuContainer.innerHTML = renderMobileMenu();
  }

  const footerContainer = document.getElementById('footer-container');
  if (footerContainer) {
    footerContainer.innerHTML = renderFooter();
  }

  const cartContainer = document.getElementById('cart-container');
  if (cartContainer) {
    cartContainer.innerHTML = renderCart();
  }

  if (typeof cart !== 'undefined') {
    cart.updateUI();
  }
}

if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initializeComponents);
} else {
  initializeComponents();
}
