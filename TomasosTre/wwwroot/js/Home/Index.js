$(document).ready(function () {
    $.get('/Home/GetDishNames').then(function (response) {
        $(".js-example-data-array").select2({
            placeholder: "Select a dish to order",
            data: response
            
        });
        $(".js-example-data-array").on('change', function (e) {
            var val = $(".js-example-data-array").val();
            $.post('/Order/Add', { id: val }, function (response) {
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
        // BUGFIX: Find out why this event only fires once (jQuery shuts it down after execution, but why?)
        var dishId = $(this).data("dish");
        var amount = $(this).val();
        $.post("/Order/Set", { id: dishId, amount: amount }, function (response) {
            // debug line.
            console.log("Post returned" + response);
            $("#cart").html(response);
            createOrderRowEvents()
        });
    });
    $(".edit").on("click", function() {
        // Open div with checkboxes that lets user customize dish
        $.get("/Home/DishCustomizePartial", { id: $(this).data("dish") }, function(response) {
                $("#dish-customizer-target").html(response);
            }
        );
    });
    $(".remove").on("click", function () {
        // send data-dish id to Order/Remove
        var dishId = $(this).data("dish");
        $.post("/Order/Remove", { id: dishId }, function (response) {
            $("#cart").html(response);
            createOrderRowEvents()
        });
    });
}