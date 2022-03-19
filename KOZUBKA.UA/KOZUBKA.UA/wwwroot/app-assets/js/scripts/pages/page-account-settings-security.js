$(function () {
  ('use strict');

  // variables
  var form = $('.validate-form'),
    select2 = $('.select2'),
    accountNumberMask = $('.account-number-mask');

  // jQuery Validation for all forms
  // --------------------------------------------------------------------
  if (form.length) {
    form.each(function () {
      var $this = $(this);

      $this.validate({
        rules: {
          password: {
            required: true
          },
          'new-password': {
            required: true,
            minlength: 1
          },
          'confirm-new-password': {
            required: true,
            minlength: 1,
            equalTo: '#account-new-password'
          },
          apiKeyName: {
            required: true
          }
        },
        messages: {
          'new-password': {
          
          },
          'confirm-new-password': {
            equalTo: 'Ви ввели не однакові нові паролі'
          }
        }
      });
      $this.on('submit', function (e) {
        e.preventDefault();
      });
    });
  }

  //phone
  if (accountNumberMask.length) {
    accountNumberMask.each(function () {
      new Cleave($(this), {
        phone: true,
        phoneRegionCode: 'US'
      });
    });
  }

  // For all Select2
  if (select2.length) {
    select2.each(function () {
      var $this = $(this);
      $this.wrap('<div class="position-relative"></div>');
      $this.select2({
        dropdownParent: $this.parent()
      });
    });
  }
});
