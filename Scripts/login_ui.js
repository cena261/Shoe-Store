document.addEventListener('DOMContentLoaded', function () {
  const container = document.querySelector('.login-page .container');
  const adminBtn = document.querySelector('.administrator-btn');
  const studentBtn = document.querySelector('.student-btn');

  if (adminBtn && studentBtn && container) {
    adminBtn.addEventListener('click', function () {
      container.classList.add('active');
    });

    studentBtn.addEventListener('click', function () {
      container.classList.remove('active');
    });
  }
  // Allow real form submissions in MVC (no preventDefault)
});
