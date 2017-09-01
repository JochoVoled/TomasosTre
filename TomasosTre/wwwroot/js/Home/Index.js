$(document).ready(function () {
    $.get('/Api/GetDishNames').then(function (response) {
        $(".js-example-data-array").select2({
            placeholder: "Select a dish to order",
            data: response
            
        });
        $(".js-example-data-array").on('change', function (e) {
            var val = $(".js-example-data-array").val();
            $.post('/Api/Add', { id: val }, function (response)
            {
                $("#cart").html(response);
                createOrderRowEvents();
            });
        });
    });

    if ($(".amount")) {
        createOrderRowEvents();
    }

});

function createOrderRowEvents() {
    $(".amount").on("change",function () {
        //console.log($(this));
        var dishId = $(this).data("dish");
        var amount = $(this).val();
        $.post("/Api/Set", { id: dishId, amount: amount }, function (response)
        {
            // debug line.
            $("#cart").html(response);
            createOrderRowEvents();
        });
    });
    $(".edit").on("click", function() {
        // Open div with checkboxes that lets user customize dish
        $.get("/Render/DishCustomizePartial", { id: $(this).data("dish") }, function(response) {
            $("#dish-customizer-target").html(response);
                registerCustomDishEvents();
            }
        );
    });
    $(".remove").on("click", function () {
        // send data-dish id to Order/Remove
        var dishId = $(this).data("dish");
        $.post("/Api/Remove", { id: dishId }, function (response) {
            $("#cart").html(response);
            createOrderRowEvents();
        });
    });
    registerCheckoutEvents();
}

function registerCustomDishEvents() {
    $("#dish-customize").on("click", function ()
    {
        var data = {
            baseDishId: "",
            isOrderedIngredients: []
        };
        data.baseDishId = $("#title").data("dish");
        // next row taken from https://stackoverflow.com/questions/416752/select-values-of-checkbox-group-with-jquery
        $.each($("input[name='ingredient']:checked"), function ()
        {
            data.isOrderedIngredients.push($(this).data("dish"));
        });
        $.post("/Api/CustomizedDish", data, function (response)
        {
            $('#dish-customizer').modal('hide');
            $("#cart").html(response);
            createOrderRowEvents();
        });
    });
}

function registerCheckoutEvents() {
    $("#checkout").on("click", function ()
    {
        var data = {};
        $.get('Render/CheckoutPartial', data, function (response)
        {
            $("#root").html(response);
            //setupCheckout();
        });
    });
}

function setupCheckout() {
    $("#order").on("click", function() {
        var data = {
            CardNumber: $("#card-number").val(),
            SecurityNumber: $("#security-number").val(),
            ExpiryMonth: $("#expiry-month").val(),
            Address: $("#address").val(),
            Zip: $("#zip").val(),
            City: $("#city").val(),
            Email: $("#email").val(),
            IsRegistrating: $("#isRegistrating").checked
        };

        $.get('Api/PlaceOrder', data, function (response) {
            $("#register").html(response);
            if (data.IsRegistrating) {
                setupCheckout();
            }            
        });
    });
}