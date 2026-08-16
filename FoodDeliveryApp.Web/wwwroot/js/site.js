// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code

function FetchCartCount() {
    $.getJSON('/Customer/ShoppingCart/GetCartCount', function (data) {
        $('.cart-badge').text(data.count);
    });
}

$(document).ready(function () {

    $(document).on("click", "a", function () {
        $('#globalLoader').show();
    })

    $(document).on("submit", "form", function () {
        $('#globalLoader').show();
    });

    $(document).on('click', '.btnAddToCart', function (e) {
        e.preventDefault();

        var button = $(this);
        var menuItemId = button.data('menuitemid');


        $.ajax({
            url: '/Customer/ShoppingCart/Add',
            type: 'POST',
            data: {
                menuItemId: menuItemId,
                quantity: 1
            },
            success: function (response) {
                if (response.success) {
                    button.addClass('d-none');
                    var cart = button.closest(".menu-item-card");
                    cart.find(".qty-box").removeClass('d-none');
                    cart.find(".qty").text(1)

                    FetchCartCount();
                    toastr.success(response.message || "Item added to the cart successfully.");
                }
            },
            error: function (xhr) {
                if (xhr.status == 401) {
                    toastr.warning("Please log in to continue.");
                } else {
                    toastr.error("Something went wrong.");
                }
            }
        })
    });

    $(document).on('click', '.increase', function (e) {
        e.preventDefault();
        var button = $(this);
        const menuItemId = button.data('menuitemid');
        const qtyBox = button.closest('.qty-box');
        const spanQty = qtyBox.find('.qty');
        const currentQty = parseInt(spanQty.text());
        let newQty = currentQty + 1;
        const decreaseBtn = qtyBox.find('.decrease');

        $.ajax({
            url: '/Customer/ShoppingCart/Update',
            type: 'POST',
            data: {
                menuItemId: menuItemId,
                quantity: newQty
            },
            success: function (response) {
                if (response.success) {
                    spanQty.text(newQty);
                    decreaseBtn.prop("disabled", newQty <= 1);

                    FetchCartCount();
                    toastr.success(response.message || "Cart updated successfully.");
                } else {
                    toastr.error(response.message || "Failed to update cart.");
                }
            },
            error: function (xhr) {
                if (xhr.status == 401) {
                    toastr.warning("Please log in to continue.");
                } else {
                    toastr.error("Something went wrong.");
                }
            }
        })
    });


    $(document).on('click', '.decrease', function (e) {
        e.preventDefault();
        var button = $(this);
        const menuItemId = button.data('menuitemid');
        const qtyBox = button.closest('.qty-box');
        const spanQty = qtyBox.find('.qty');
        const currentQty = parseInt(spanQty.text());
        let newQty = currentQty - 1;
        const decreaseBtn = qtyBox.find('.decrease');

        if (newQty < 1) {
            newQty = 1;
        }

        $.ajax({
            url: '/Customer/ShoppingCart/Update',
            type: 'POST',
            data: {
                menuItemId: menuItemId,
                quantity: newQty
            },
            success: function (response) {
                if (response.success) {
                    spanQty.text(newQty);
                    decreaseBtn.prop("disabled", newQty <= 1);

                    FetchCartCount();
                    toastr.success(response.message || "Cart updated successfully.");
                } else {
                    toastr.error(response.message || "Failed to update cart.");
                }
            },
            error: function (xhr) {
                if (xhr.status == 401) {
                    toastr.warning("Please log in to continue.");
                } else {
                    toastr.error("Something went wrong.");
                }
            }
        })
    });


    $(document).on('click', '#btnDetailsAddToCart', function (e) {
        e.preventDefault();
        var button = $(this);
        var menuItemId = button.data('menuitemid');
        var quantity = parseInt($('#cartQty').text());

        $.ajax({
            url: '/Customer/ShoppingCart/Add',
            type: 'POST',
            data: {
                menuItemId: menuItemId,
                quantity: quantity
            },
            success: function (response) {
                if (response.success) {
                    FetchCartCount();
                    toastr.success(response.message || "Item added to cart successfully.");
                    $('#cartQty').text(1);
                } else {
                    toastr.error(response.message || "Failed to add cart.");
                }
            },
            error: function (xhr) {
                if (xhr.status == 401) {
                    toastr.warning("Please log in to continue.");
                } else {
                    toastr.error("Something went wrong.");
                }
            }
        })
    });
});