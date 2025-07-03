
function detectarCarga() {
    $("#cargando").hide();

}

function alerta(tipo, mensaje, adicional) {
    if (tipo == 'A') {
        bootbox.alert('<b style="color:#f75757"><span class="fi ti-info-alt"></span> Alerta:</b> ' + mensaje)
    } else if (tipo == 'OK') {
        bootbox.alert('<b style="color:#337ab7"><span class="fi ti-check"></span> Exito:</b> ' + mensaje)
    } else if (tipo == 'OKLINK') {
        bootbox.alert('<b style="color:#337ab7"><span class="fi ti-check-box"></span> Exito:</b> ' + mensaje, function () { $("#cargando").hide(); window.location.href = adicional; })
    } else if (tipo == 'SS') {
        bootbox.alert('<b style="color:#a94442"><span class="fi ti-info-alt"></span> Alerta:</b> La sesión ha expirado. Cierre y vuelva a ingresar al sistema. ', function () { $("#cargando").hide(); window.location.href = $("#timer").data("url"); })
    } else if (tipo == 'OKFOCUS') {
        bootbox.alert('<b style="color:#337ab7"><span class="fi ti-check-box"></span> Exito:</b> ' + mensaje, function () {
            setTimeout(function () {
                $(adicional).focus()
            }, 300);

        })
    } else if (tipo == 'AFOCUS') {
        bootbox.alert('<b style="color:#f75757"><span class="fi ti-info-alt"></span> Alerta:</b> ' + mensaje, function () {
            setTimeout(function () {
                $(adicional).focus()
            }, 300);

        })
    }
    else {
        bootbox.alert('<b style="color:#a94442"><span class="fi ti-thumb-down"></span> Error:</b> ' + mensaje)
    }

}

function soloNumerosValidaVarios(txt, e, tipo) {
    var key = (window.event) ? event.keyCode : e.which;

    var tipoKey = -1;
    if (tipo == 0) // solo numero
        tipoKey = -1
    else if (tipo == 1) // solo numero decimal
        tipoKey = 46
    else if (tipo == 2) // solo numero fecha
        tipoKey = 47
    else if (tipo == 3) // solo numero hora
        tipoKey = 58

    //Was key that was pressed a numeric character (0-9) or backspace (8)?
    if (key > 47 && key < 58 || key == 8 || key == tipoKey || key == 0) {
        var continuar = true;
        if (key == 46) {
            if (txt.value.indexOf(".") != -1) {
                continuar = false;
            }
        }
        if (key == 58) {
            if (txt.value.indexOf(":") != -1) {
                continuar = false;
            }
        }

        if (continuar) {
            return true;
        } else {
            if (window.event) {
                window.event.returnValue = null;
            } //IE
            else {
                e.preventDefault();
            } //Firefox
        }
    } else {
        if (window.event) {
            window.event.returnValue = null;
        } //IE
        else {
            e.preventDefault();
        } //Firefox
    }
}

function soloLetras(e) {
    key = e.keyCode || e.which;
    tecla = String.fromCharCode(key).toLowerCase();
    letras = " áéíóúabcdefghijklmnñopqrstuvwxyz";
    especiales = "8-37-39-46";

    tecla_especial = false
    for (var i in especiales) {
        if (key == especiales[i]) {
            tecla_especial = true;
            break;
        }
    }

    if (letras.indexOf(tecla) == -1 && !tecla_especial) {
        return false;
    }
}

function mayus(e) {
    e.value = e.value.toUpperCase();
}

//timer session
function timer() {
    const tiempoRestanteStr = parseFloat(document.getElementById('timer').getAttribute('data-tiempo-restante'));

    var sec = tiempoRestanteStr * 60,
        countDiv = document.getElementById("timer"),
        secpass,
        countDown = setInterval(function () {
            'use strict';

            secpass();
        }, 1000);

    function secpass() {
        'use strict';

        var min = Math.floor(sec / 60),
            remSec = Math.round(sec % 60);

        if (remSec < 10) {

            remSec = '0' + remSec;

        }
        if (min < 10) {

            min = '0' + min;

        }
        countDiv.innerHTML = min + ":" + remSec;

        if (sec > 0) {

            sec = sec - 1;

        } else {

            clearInterval(countDown);
            countDiv.innerHTML = "00:00";
            var formularioValidado = false;
            alerta('SS');

        }
    }
}

