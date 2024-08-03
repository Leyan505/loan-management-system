// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

+function ($, window) {
    function calc() {
        var amount = parseFloat($('.amount-input').val());
        var utility = parseFloat($('#utility').val());
        var total = parseFloat(amount * utility) + parseFloat(amount);
        var quote = parseFloat(total) / parseFloat($('#payment_number').val())

        $('.total-box #total_show').html(total.toFixed(2));
        $('.total-box #quote').html(quote.toFixed(2));
        if (isNaN(total)) {
            $('.total-box').addClass('hidden');
        } else {
            $('.total-box').removeClass('hidden');
        }
    }

    $('body').on('keyup', '.amount-input', function (e) {
        return calc();
    });
    $('body').on('change', '#utility', function () {
        return calc();
    });
    $('body').on('change', '#payment_number', function () {
        return calc();
    });

    $('body').on('change', '.supervisor-client #wallet', function () {
        $('#link_client_audit').attr('disabled', false);
        $('#link_client_audit').attr('href', $('#link_client_audit').attr('href') + '/' + $(this).val());
    });

    $('body').on('submit', '.payment-create', function () {
        if (confirm('Esta seguro de realizar el pago (' + $('.payment-create #amount').val() + ')')) {
            return true;
        } else {
            return false;
        }
    });

    $('body').on('click', '.ajax-btn', function () {
        var id_user = $(this).attr('id_user');
        var id_credit = $(this).attr('id_credit');
        $(this).prop("disabled", true);
        $.get("/payment/Edit/" + id_user,
            {
                id_credit: id_credit,
                ajax: true
            }
        )
            .done(function (data) {
                $('#td_' + id_credit).addClass('d-none');
            });
    });

    $('body table').addClass('table-striped');

    $('#modal_pay').on('show.bs.modal', function (e) {
        $('body form').submit(function (event) {
            $(this).find(":submit").prop("disabled", false);
        });
        $('body .modal-pay .msg-success .text-success').val(0);
        $('body .modal-pay .msg-success .text-primary').val(0);
        $('body .modal-pay .main-body').removeClass('d-none');
        $('body .modal-pay .msg-success').addClass('d-none');
        $('body .modal-pay #name').val('');
        $('body .modal-pay #credit_id').val('');
        $('body .modal-pay #amount_value').val('');
        $('body .modal-pay #done').val('');
        $('body .modal-pay #saldo').val('');
        $('body .modal-pay #payment_quote').val('');
        $('body .modal-pay #done_payment').val('');
        $('body .modal-pay #amount').attr('max', '');
        $('body .modal-pay #amount').val('')
        var attr = e.relatedTarget.attributes;
        for (var a in attr) {
            if (a == 3) {
                $.get("/payment/" + attr[a].nodeValue, {
                    format: 'json'
                })
                    .done(function (res) {
                        $('body .modal-pay #name').val(res.name + ' ' + res.last_name);
                        $('body .modal-pay #credit_id').val(res.credit_id);
                        $('body .modal-pay #amount_value').val(res.amount_neto.toFixed(2) + ' en ' + res.payment_number);
                        $('body .modal-pay #done').val(res.positive.toFixed(2));
                        $('body .modal-pay #saldo').val(res.rest.toFixed(2));
                        $('body .modal-pay #payment_quote').val(res.payment_quote.toFixed(2));
                        $('body .modal-pay #done_payment').val(res.payment_done);
                        $('body .modal-pay #amount').attr('max', res.rest);
                        $('body .modal-pay #amount').val(res.payment_quote)
                    });
            }
        }
    });

    $('body form').submit(function (event) {
        $(this).find(":submit").prop("disabled", true);
    });

    $('body .modal-pay').submit(function (event) {

        //console.log(event);
        event.preventDefault();
        var data = {
            credit_id: $('body .modal-pay #credit_id').val(),
            amount: $('body .modal-pay #amount').val(),
            format: 'json',
        };
        var actionurl = event.currentTarget.action;
        //console.log(data);
        //do your own request an handle the results
        
        $.post(actionurl,
            data, function (res) {
                if (res.status === 'success') {
                    var id_credit = $('body .modal-pay #credit_id').val();
                    $('body .agente-route-table #td_' + id_credit).addClass('visually-hidden');
                    $('body .modal-pay .msg-success .text-success').html(res.recent);
                    $('body .modal-pay .msg-success .text-primary').html(res.rest);
                    $('body .modal-pay .main-body').addClass('visually-hidden');
                    $('body .modal-pay .msg-success').removeClass('visually-hidden');
                } else {
                    var conf = confirm('Ya realizo un pago hoy, deseas realizar otro?');

                    if (conf) {
                        var data = {
                            credit_id: $('body .modal-pay #credit_id').val(),
                            amount: $('body .modal-pay #amount').val(),
                            format: 'json',
                            rev: true
                        };
                        var actionurl = event.currentTarget.action;
                        $.post(actionurl,
                            data, function (res) {
                                if (res.status === 'success') {
                                    var id_credit = $('body .modal-pay #credit_id').val();
                                    $('body .agente-route-table #td_' + id_credit).addClass('visually-hidden');
                                    $('body .modal-pay .msg-success .text-success').html(res.recent);
                                    $('body .modal-pay .msg-success .text-primary').html(res.rest);
                                    $('body .modal-pay .main-body').addClass('visually-hidden');
                                    $('body .modal-pay .msg-success').removeClass('visually-hidden');
                                } else {
                                    alert('Algo sucedio');
                                }
                            });
                        
                    }

                }
        })
        $('#modal_pay').on('hide.bs.modal', function (e) {
            $.get("/payment/", {
                format: 'json'
            })
                .done(function (res) {
                    $('body .modal-pay #name').val(res.name + ' ' + res.last_name);
                    $('body .modal-pay #credit_id').val(res.credit_id);
                    $('body .modal-pay #amount_value').val(res.amount_neto + ' en ' + res.payment_number);
                    $('body .modal-pay #done').val(res.positive);
                    $('body .modal-pay #saldo').val(res.rest);
                    $('body .modal-pay #payment_quote').val(res.payment_quote);
                    $('body .modal-pay #done_payment').val(res.payment_done);
                    $('body .modal-pay #amount').attr('max', res.rest);
                    $('body .modal-pay #amount').val(res.payment_quote)
                });
        });
    });

}(jQuery, window);

function initialize() {
    const input = document.querySelector('.g-autoplaces-address');
    const autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.addListener('place_changed', function () {
        const place = autocomplete.getPlace();
        //console.log(place)
        const mapElement = document.querySelector(".over-change-display")
        if (place) {
            //console.log(mapElement)
            mapElement.setAttribute('style', 'display:block !important');
            initMap({ lat: place.geometry['location'].lat(), lng: place.geometry['location'].lng() })
        }
        // $('#latitude').val(place.geometry['location'].lat());
        // $('#longitude').val(place.geometry['location'].lng());
        //
        // // --------- show lat and long ---------------
        // $("#lat_area").removeClass("d-none");
        // $("#long_area").removeClass("d-none");
    });
}

function toggleBounce(event) {
    // initMap({lat:event.latLng.lat(), lng:event.latLng.lng()})
    $('body .new-register #lat').val(event.latLng.lat());
    $('body .new-register #lng').val(event.latLng.lng());

}

function initMap({ lat, lng }) {
    // The location of Uluru
    const uluru = { lat, lng };
    // The map, centered at Uluru
    const map = new google.maps.Map(document.querySelector(".map-google"), {
        zoom: 9,
        center: uluru,
    });
    // The marker, positioned at Uluru
    const marker = new google.maps.Marker({
        position: uluru,
        map: map,
        draggable: true
    });

    $('body .new-register #lat').val(lat);
    $('body .new-register #lng').val(lng);

    marker.addListener("dragend", toggleBounce);
}
