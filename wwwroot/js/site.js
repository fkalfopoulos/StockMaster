$(document).ready(function () {
    // Collapsing navbar after clicking a link
    $('.navbar-collapse').click(function () {
        $(".navbar-collapse").collapse('hide');

    });
    $(".navbar-brand").click(function () {
        $(".navbar-collapse").collapse('hide');
    })
});
// end of Collapsing navbar after clicking a link
//////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////
// Symbol search button from _SearchSymbolLayout page Anonymous users

$(document).ready(function () {
    $("#SearchSymbol").focus();
    $("#btnSymbol").on("click", function (event) {
        event.preventDefault();
        $("#txtError").empty();
        var _searchSymbol = $("#SearchSymbol").val();
        var monthsymbol = $("#MonthId").val();
        let data =
        {
            Symbol: _searchSymbol,
            MonthId : monthsymbol
        }
        if (_searchSymbol == "") { $("#txtError").html("Symbol cannot be empty."); return }
        $.ajax({
            url: "/OpenAPI/getPrice/?SearchSymbol",
            type: "GET",
            data : data,
            success: function (response) {               
                $("#priceDiv").html(response);
            }
        });
        console.log(monthsymbol);
        $.ajax({
            url: "/OpenAPI/getChart/?SearchSymbol=" + _searchSymbol,
            type: "GET",
            success: function (res) {
                $("#symbolChartDiv").html(res);
            }
        });
        $.ajax({
            url: "/OpenAPI/marketCap/?SearchSymbol=" + _searchSymbol,
            type: "GET",
            success: function (res) {
                $("#marketCap").html(res);
            }
        });
        $.ajax({
            url: "/OpenAPI/getInfo/?SearchSymbol=" + _searchSymbol,
            type: "GET",
            success: function (res) {
                $("#symbolInfoDiv").html(res);               
            }
        });
        $.ajax({
            url: "/OpenAPI/getAdvanced/?SearchSymbol=" + _searchSymbol,
            type: "GET",
            success: function (res) {
                $("#symbolAdvancedDiv").html(res);
            }
        });
        $.ajax({
            url: "/OpenAPI/getFin/?SearchSymbol=" + _searchSymbol,
            type: "GET",
            success: function (res) {               
                $("#finDiv").html(res);               
            }
        });
        $.ajax({
            url: "/OpenAPI/GetFunds/?SearchSymbol=" + _searchSymbol,
            type: "GET",
            success: function (res) {   
                console.log(res);
                $("#peersDiv").html(res);              
            }
        });
        
     
    });
    $("#SearchSymbol").keypress(function (event) {
        var key = event.which;
        if (key == 13) {
            $("#btnSymbol").click();
            return false;
        }
    });
});

$("#ddlSymbol").change(function (event) {
    event.preventDefault();
    $("#warningMessage").empty();
    var ddlSymbolSelected = $(this).find("option:selected");
    var TickerSymbol = ddlSymbolSelected.text();
    $.ajax({
        url: "/OpenAPI/getChart/?SearchSymbol=" + TickerSymbol,
        type: "GET",
        success: function (res) {
            $("#symbolChartDiv").html(res);
        }
    });
});

// Symbol search button from _SearchSymbolLayout page signed in user
$(document).ready(function () {
    $("#btnSignedInSymbol").on("click", function (event) {
        event.preventDefault();
        $("#txtError").empty();
        var txtBox = document.getElementById('SearchSymbol').value;
        if (typeof txtBox !== 'undefined' && txtBox) {
            $.ajax({
                url: "/OpenAPI/getSymbolPrice/?SearchSymbol=" + txtBox,
                type: "GET",
                success: function (response) {
                    if (response == "") {
                        $("#txtError").html("Symbol is not VALID."); return;
                    }
                    else {
                        window.location.href = '/Trade/Create?symbol=' + txtBox + '&price=' + response;
                    }
                }
            });
        }
        else {
            $("#txtError").html("Symbol cannot be empty."); return;
        }
    });
    $("#SearchSymbol").keypress(function (event) {
        var key = event.which;
        if (key == 13) {
            $("#btnSignedInSymbol").click();
            return false;
        }
    });

});

// on symbol dropdown list change event getting symbol price to fill price input.
// TradeController Create Page
$("#ddlSymbol").change(function () {
    $("#warningMessage").empty();
    var ddlSymbolSelected = $(this).find("option:selected");
    var TickerSymbol = ddlSymbolSelected.text();
    $("#lblTradeON").html(TickerSymbol);

    $.ajax({
        url: "/OpenAPI/getSymbolPrice/?SearchSymbol=" + TickerSymbol,
        type: "GET",
        success: function (response) {
            $("#lblPrice").val(response);
            if (isNaN($("#txtQuantity").val())) {

            }
            else {
                $("#txtQuantity").val('');
                $("#txtTotal").val('');
            }
        },
        fail: function (error) {
            console.log(error);
            $("#warningMessage").html("<h2 class='alert alert-danger text-center'>Error! Try later!!!</h2>")

        }
    });

});

function resetTradeBoxes() {
    $("#txtQuantity").empty();
    $("#lblPrice").empty();
    $("#txtTotal").empty();
}

//Trade Action dropdown list select logic
$("#ddlTradeAction").change(function (event) {
    event.preventDefault();
    resetTradeBoxes();
    $("#warningMessage").empty();
    var ddlTradeActionSelected = $("#ddlTradeAction").find("option:selected");
    var tradeAction = ddlTradeActionSelected.text();
    var symbolId = $("#ddlSymbol").find("option:selected").val();
    console.log(symbolId);
    if (tradeAction.toUpperCase() == "BUY") {
        var total = (Number.parseFloat($("#lblPrice").val()).toFixed(2) * Number.parseFloat($("#txtQuantity").val()).toFixed(2)).toFixed(2);
        var balance = Number.parseFloat($("#lblCashBalanceAmount").text().replace("$", "")).toFixed(2);
        if ((parseFloat(balance) < parseFloat(total))) {
            $("#warningMessage").html("<h2 class='alert alert-danger text-center'>You cannot buy it because of the low balance!</h2>")
            $("#txtQuantity").val('');
            $("#txtTotal").val('');
            return;
        }
        else {
            $("#txtTotal").val(total);
            return;
        }
    }
    else if (tradeAction.toUpperCase() == "SELL") {
        // ajax call to check can user sell or not?
        ajaxWealthQuantity(symbolId);
    }
});


// calculating total amount and comparing with balance.
$("#txtQuantity").keyup(function (event) {
    event.preventDefault();
    resetTradeBoxes();
    $("#warningMessage").empty();
    var total = (Number.parseFloat($("#lblPrice").val()).toFixed(2) * Number.parseFloat($("#txtQuantity").val()).toFixed(2)).toFixed(2);
    var balance = Number.parseFloat($("#lblCashBalanceAmount").text().replace("$", "")).toFixed(2);
    var ddlTradeActionSelected = $("#ddlTradeAction").find("option:selected");
    var tradeAction = ddlTradeActionSelected.text();
    var symbolId = $("#ddlSymbol").find("option:selected").val();
    if ((parseFloat(balance) < parseFloat(total)) && tradeAction.toUpperCase() != "SELL") {
        $("#warningMessage").html("<h2 class='alert alert-danger text-center'>You cannot buy it because of the low balance!</h2>")
        $("#txtQuantity").val('');
        $("#txtTotal").val('');
        return;
    }
    else if (tradeAction.toUpperCase() == "SELL") {
        // ajax call to check can user sell or not?
        ajaxWealthQuantity(symbolId);
    }
    else {
        $("#txtTotal").val(total);
        return;
    }
});

function ajaxWealthQuantity(symbolId) {
    // ajax call to check can user sell or not?
    $.ajax({
        url: "/TradeHelperService/apiWealthFind/?symbolId=" + symbolId,
        type: "GET",
        success: function (response) {
            console.log(response);
            if (typeof response !== "undefined") {
                var ownQty = Number(response);
                var sellQty = Number($("#txtQuantity").val());
                console.log("sellQty: " + sellQty);
                console.log("ownQty: " + ownQty);
                console.log(ownQty > sellQty);
                if (ownQty >= sellQty) {
                    $("#txtTotal").val(total);
                    return;
                }
                else {

                    $("#warningMessage").html("<h2 class='alert alert-danger text-center'>You cannot sell it because of the low shares!</h2>")
                    $("#txtQuantity").val('');
                    $("#txtTotal").val('');
                    return;
                }
            }
            else {

                $("#warningMessage").html("<h2 class='alert alert-danger text-center'>You cannot sell it because of the low shares!</h2>")
                $("#txtQuantity").val('');
                $("#txtTotal").val('');
                return;
            }
        },
    });
}
// Jquery ajax symbol auto complete
$(document).ready(function () {
    $('#SearchSymbol').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/OpenAPI/getSymbolAutocomplete/?txtSymbol=" + $("#SearchSymbol").val(),
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response(data);
                }
            })
        }
    });
});


$(document).ready(function () {
    $("#btnCookieString").on("click", function (el) {
        document.cookie = el.target.dataset.cookieString;
        document.querySelector("#cookieConsent").classList.add("hidden");
    }, false);
});


$(document).ready(function () {
    if (top.location.pathname === "/") {
        $("body").css({'background-image': 'url("/images/bg-pattern.png")'});
    }
});