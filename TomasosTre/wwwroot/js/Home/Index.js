$(document).ready(function () {
    $(".add").on('click', function (e) {
        var val = $(this).data("dish");
        $.post('/Api/Add', { id: val }, function (response) {
            $("#cart").html(response);
            createOrderRowEvents();
        });
    });
    if ($(".amount")) {
        createOrderRowEvents();
    }

});

function createOrderRowEvents() {
    $(".amount").on("change",function () {
        var dishId = $(this).data("dish");
        var amount = $(this).val();
        $.post("/Api/Set", { id: dishId, amount: amount }, function (response)
        {
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
        });
    });
}