$(function () {
    modalMessage();
    var validCap = false;
    var numberrand = Math.floor((Math.random() * 100) + 1);
    $('#lblValorAleatorio').html(numberrand);
    $("#slider").slider({
        orientation: "horizontal",
        range: "min",
        min: 0,
        max: 100,
        value: 0,
        slide: function (event, ui) {
            $("#amount").val(ui.value);
            if ($("#amount").val() == $('#lblValorAleatorio').html()) {
                $('#divCapcha').css('background-color', 'green');
                validCap = true;
            }
            else { $('#divCapcha').css('background-color', ''); validCap = false; }
        }
    });
    $("#amount").val($("#slider").slider("value"));

    $("#amount").blur(function () {
        if ($("#amount").val() == $('#lblValorAleatorio').html()) {
            $('#divCapcha').css('background-color', 'green');
            validCap = true;
        }
        else { $('#divCapcha').css('background-color', ''); validCap = false; }
    });

    $('#frmContact').validate({ // initialize plugin
        rules: {
            name: "required",
            message: "required",
            email: {
                required: true,
                email: true
            }
        },
        messages: {
            name: "Especifica tu nombre",
            message: "Escribe un mensaje",
            email: {
                required: "Necesitamos tu email para contactarnos contigo",
                email: "Tu email debe tener el siguiente formato dominio@..."
            }
        }
        ,
        // your rules & options,
        submitHandler: function (form) {
            // your ajax would go here
            if (validCap) {
                sendMail();
            }
            return false;  // blocks regular submit since you have ajax
        },
        showErrors: function (errorMap, errorList) {
            this.defaultShowErrors();
        },
        errorContainer: "#divSummaryErrors",
        errorLabelContainer: "#divSummaryErrors ul",
        wrapper: "li",
        errorClass: "authError",
        highlight: function (element, required) {
            $(element).fadeOut(function () {
                $(element).fadeIn();
                $(element).css('border', '2px solid #FDADAF');
            });
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).css('border', '1px solid #CCC');
        }
    });

});

function sendMail() {
    $.ajax({
        method: "POST",
        url: getHost() + "Home/SendMail",
        data: { name: $('#name').val(), email: $('#email').val(), textMessage: $('#message').val() },
        success: function (msg) {
            $("#dialog-message").html('Tu mensaje ha sido enviado.')
                .dialog('open');
        },
        //Manejo del ERROR
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $("#dialog-message").html('En este momento no podemos procesar tu solicitud, por favor intenta más tarde.')
                .dialog('open');
            console.log(XMLHttpRequest);
        }
    })
}

function modalMessage() {
    $("#dialog-message").dialog({
        modal: true,
        autoOpen: false,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
                location.reload();
            }
        }
    });
}

function getHost() {
    return window.location.protocol + "\\\\" + window.location.host + "\\";
}