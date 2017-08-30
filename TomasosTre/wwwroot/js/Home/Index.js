$(document).ready(function () {
    $.get('/Home/GetDishNames').then(function (response) {
        $(".js-example-data-array").select2({
            placeholder: "Select a dish to order",
            data: response
            
        });
        $(".js-example-data-array").on('change', function (e) {
            var val = $(".js-example-data-array").val();
            $.post('/Api/Add', { id: val }, function (response)
            {
                //console.log("Post returned" + response);
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
        console.log($(this));
        var dishId = $(this).data("dish");
        var amount = $(this).val();
        $.post("/Api/Set", { id: dishId, amount: amount }, function (response)
        {
            // debug line.
            console.log("Post returned" + response);
            $("#cart").html(response);
            createOrderRowEvents();
        });
    });
    $(".edit").on("click", function() {
        // Open div with checkboxes that lets user customize dish
        $.get("/Home/DishCustomizePartial", { id: $(this).data("dish") }, function(response) {
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
}

function registerCustomDishEvents() {
    $("#dish-customize").on("click", function ()
    {
        console.log("Howdy there");
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
            console.log(response);
        });
    });
}