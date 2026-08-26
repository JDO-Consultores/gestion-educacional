$.ajaxSetup({
    beforeSend: function (xhr) {
        var token = localStorage.getItem('jwtToken');
        if (token) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        }
    }
});

function formatChileanNumber(input, allowDecimals) {
    let value = input.replace(/\./g, '').replace(/,/g, '.');

    // Permitir decimales si corresponde
    if (!allowDecimals) {
        value = value.replace(/[^0-9]/g, '');
    } else {
        value = value.replace(/[^0-9.]/g, '').replace(/(\..*?)\..*/g, '$1');
    }

    if (value === '') return '';

    let parts = value.split('.');
    let integerPart = parts[0];
    let decimalPart = parts.length > 1 ? ',' + parts[1].substring(0, 2) : '';

    // Agregar separadores de miles
    integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, '.');

    return integerPart + decimalPart;
}

function adjuntarOrdenEditor(container, options) {
    var uniqueId = 'file_' + kendo.guid();
    var fileInput = $('<input type="file" id="' + uniqueId + '" name="' + options.field + '" accept=".pdf,.jpg,.jpeg,.png,.doc,.docx" class="k-textbox" style="width: 100%;" />')
        .appendTo(container)
        .on('change', function (e) {
            var file = e.target.files[0];
            if (file) {
                if (file.size > 5242880) {
                    showToast("error", "El archivo no debe superar 5MB.");
                    $(this).val('');
                    return;
                }
                options.model.set(options.field, file.name);
                options.model.fileData = file;
            }
        });
}

function logoutAndSync() {
    localStorage.setItem('logout', 'true');
    document.getElementById('logoutForm').submit();
}

function loadView($id, controllerUrl) {
    $id.empty().load(controllerUrl, function (responseTxt, statusTxt, xhr) {
        if (statusTxt == "error")
            $(this).append("Ocurrio un error al cargar la vista: " + xhr.status);
        return statusTxt;
    });
}

/**
 * Elimina backdrops (.modal-backdrop) huérfanos y restaura el scroll del body.
 * Útil para modales cargados dinámicamente vía AJAX, donde el backdrop puede
 * quedar visible (overlay negro) si el nodo del modal se elimina sin cerrarse.
 * Solo limpia si no hay ningún modal visible para no afectar modales apilados.
 */
function limpiarBackdrops() {
    setTimeout(function () {
        if ($('.modal.show').length === 0) {
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open').css({ 'overflow': '', 'padding-right': '' });
        }
    }, 200);
}

function getCurrentDate() {
    var today = new Date();
    var day = String(today.getDate()).padStart(2, '0');
    var month = String(today.getMonth() + 1).padStart(2, '0');
    var year = today.getFullYear();
    return year + '-' + month + '-' + day;
}

function formatNumberWithThousandSeparators(element) {
    let value = element.val().replace(/\D/g, '');
    element.val(value.replace(/\B(?=(\d{3})+(?!\d))/g, "."));
}

function formatNumber(number) {
    return number.toLocaleString('es-CL');
}

/**
 * Formatea un RUT chileno al escribir: 12.345.678-9
 * Uso: formatRut("123456789") → "12.345.678-9"
 */
function formatRut(valor) {
    var v = valor.replace(/\./g, '').replace(/-/g, '').replace(/[^0-9kK]/g, '').toUpperCase();
    if (v.length < 2) return v;
    var dv   = v.slice(-1);
    var body = v.slice(0, -1).replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    return body + '-' + dv;
}

/**
 * Aplica formato RUT en tiempo real a un input jQuery.
 * Uso: aplicarFormatoRut($('#miInput'));
 */
function aplicarFormatoRut($input) {
    $input.on('input', function () {
        var pos = this.selectionStart;
        var raw = $(this).val().replace(/\./g, '').replace(/-/g, '').replace(/[^0-9kK]/gi, '');
        var fmt = formatRut(raw);
        $(this).val(fmt);
        // Mantener cursor al final cuando el usuario escribe
        this.setSelectionRange(fmt.length, fmt.length);
    });
}

function showToast(state, message) {
    var toastElement = $('#myToast');
    var map = {
        success: { cls: 'toast-success', title: 'Éxito',      icon: '✔️' },
        error:   { cls: 'toast-error',   title: 'Error',      icon: '❌' },
        info:    { cls: 'toast-info',    title: 'Información', icon: 'ℹ️' },
        warning: { cls: 'toast-warning', title: 'Atención',   icon: '⚠️' }
    };
    var cfg = map[state] || map.info;

    toastElement.removeClass('toast-success toast-error toast-info toast-warning');
    toastElement.addClass(cfg.cls);
    $('#toastIcon').text(cfg.icon);
    $('#toastTitle').text(cfg.title);
    $('#respuesta').text(message);

    var toast = new bootstrap.Toast(toastElement[0], { delay: 4000 });
    toast.show();
}

function showLoading(visible) {
    if (visible === true) {
        $("#modal-loading").modal('show');
    } else if (visible === false) {
        setTimeout(function () {
            $("#modal-loading").modal('hide');
        }, 500);
    }
}

function urlAjaxParamId(urlController, id) {
    return urlController.replace("param-id", encodeURIComponent(id));
}

function changeSelectCascade(urlController, parametro, $selectCascada) {
    var deferred = $.Deferred();

    $selectCascada.empty();
    clearSelect($selectCascada);

    if (parametro != null && parametro != "") {
        $.ajax({
            type: "GET",
            url: urlController,
            data: { id: parametro },
            success: function (data) {
                if (data.length > 0) {
                    $.each(data, function (i, val) {
                        $selectCascada.append($("<option/>", { value: val.ID, text: val.Text }));
                    });
                }
                clearSelect($selectCascada);
                deferred.resolve();
            },
            error: function (x) {
                console.error(x.status + ": " + x.statusText);
                deferred.reject();
            }
        });
        $selectCascada.prop("disabled", false);
    } else {
        $selectCascada.prop("disabled", true);
        deferred.resolve();
    }

    return deferred.promise();
}
function clearSelect($id) {
    $id.val(null).trigger("change");
}

//function formatRut(rut) {
//    if (!rut) return '';

//    rut = rut.replace(/\D/g, '');
//    if (rut.length > 1) {
//        rut = rut.slice(0, -1) + '-' + rut.slice(-1);
//    }
//    return rut.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
//}

//function formatRut(rut) {
//    if (!rut) return '';

//    const newRut = rut.replace(/\./g, '').replace(/\-/g, '').trim().toUpperCase();
//    const lastDigit = newRut.substr(-1, 1);
//    const rutDigit = newRut.substr(0, newRut.length - 1);

//    // Prepend a '0' if the RUT part is 7 digits (i.e., less than 8 digits overall)
//    const formattedRutDigit = rutDigit.length < 8 ? '0' + rutDigit : rutDigit;

//    // Formatear el RUT con los puntos adecuados
//    let format = '';
//    let count = 0;

//    for (let i = formattedRutDigit.length - 1; i >= 0; i--) {
//        const e = formattedRutDigit.charAt(i);
//        format = e + format;
//        count++;

//        // Agregar un punto cada 3 dígitos, salvo el último grupo
//        if (count % 3 === 0 && i !== 0) {
//            format = '.' + format;
//        }
//    }

//    return format + '-' + lastDigit;
//}

function formatRut(rut) {
    rut = rut.replace(/^0+|[^0-9kK]/g, '');

    if (rut.length > 1) {
        var body = rut.slice(0, -1);
        var dv = rut.slice(-1).toUpperCase();

        body = body.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        return body.replace(/,/g, '.') + '-' + dv;
    }
    return rut;
}

function validarRut(rut) {
    rut = rut.replace(/\./g, '').replace('-', ''); 
    var body = rut.slice(0, -1);
    var dv = rut.slice(-1).toUpperCase();

    var suma = 0;
    var multiplo = 2;
    for (var i = body.length - 1; i >= 0; i--) {
        suma += multiplo * body.charAt(i);
        multiplo = multiplo < 7 ? multiplo + 1 : 2;
    }
    var dvEsperado = 11 - (suma % 11);
    dvEsperado = dvEsperado === 11 ? '0' : dvEsperado === 10 ? 'K' : dvEsperado.toString();

    return dv === dvEsperado;
}

function validateRut(rut) {
    rut = rut.replace(/\./g, '').replace('-', '');  // Eliminar puntos y guiones
    var body = rut.slice(0, -1);
    var dv = rut.slice(-1).toUpperCase();

    // Calcular el dígito verificador
    var suma = 0;
    var multiplo = 2;
    for (var i = body.length - 1; i >= 0; i--) {
        suma += multiplo * body.charAt(i);
        multiplo = multiplo < 7 ? multiplo + 1 : 2;
    }
    var dvEsperado = 11 - (suma % 11);
    dvEsperado = dvEsperado === 11 ? '0' : dvEsperado === 10 ? 'K' : dvEsperado.toString();

    // Retornar true si el dígito es correcto, false si no lo es
    return dv === dvEsperado;
}

function fechaDefuncionEditor(container, options) {
    $('<input name="' + options.field + '" required/>')
        .appendTo(container)
        .kendoDatePicker({
            format: "dd-MM-yyyy"
        });
}

function reusoTemplate(dataItem) {
    if (dataItem.Reuso) {
        return 'Si'
    }
    else {
        return 'No'
    }
}

function fechaReusoEditor(container, options) {
    $('<input name="' + options.field + '" />')
        .appendTo(container)
        .kendoDatePicker({
            format: "dd-MM-yyyy",
            enable: options.model.Reuso
        });
}

function causaDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "Causa",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetCausas,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("Causa", dataItem);
            }
        });
}

function maestroDropDownEditor(container, options) {
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "NombreApellido",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetMaestros,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("Maestro", dataItem);
            }
        });
}
function bancoDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "Banco",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetBancos,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("Banco", dataItem);
            }
        });
}

function lugarDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "Lugar",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetLugarDefuncion,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("Lugar", dataItem);
            }
        });
}

function categoriaDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "Categoria",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetCategorias,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("Categoria", dataItem);
            }
        });
}

function tipoAdministradorDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "TipoAdministrador",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetTipoAdministrador,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("TipoAdministrador", dataItem);
            }
        });
}

function anioDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "Val",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetAnios,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("Val", dataItem);
            }
        });
}

function mantencionDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "FormaPago",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetFormasPago,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("FormaPago", dataItem);
            }
        });
}

function pagoDropDownEditor(container, options) {
    $('<input required name="' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: true,
            dataTextField: "FormaPago",
            dataValueField: "ID",
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        url: GetFormasPago,
                    }
                }
            },
            change: function (e) {
                var dropdown = this;
                var dataItem = dropdown.dataItem();
                options.model.set("FormaPago", dataItem);
            }
        });
}

function updateServicioDropDown(servicios) {
    $("#gridServicios").data("kendoGrid").columns.forEach(function (column) {
        if (column.field === "Servicio") {
            column.editor = function (container, options) {
                $('<input required name="' + options.field + '"/>')
                    .appendTo(container)
                    .kendoDropDownList({
                        autoBind: true,
                        dataTextField: "Servicio",
                        dataValueField: "ID",
                        dataSource: {
                            data: servicios,
                            group: { field: "Servicios.Categoria" }
                         },
                        change: function (e) {
                            var dropdown = this;
                            var dataItem = dropdown.dataItem();
                            options.model.set("Servicio", dataItem);
                            if (!options.model.get("Precio")) {
                                if (dataItem && dataItem.Precio) {
                                    options.model.set("Precio", dataItem.Precio);
                                } else {
                                    options.model.set("Precio", 0);
                                }
                            }
                        }
                    });
            };
        }
    });
}

function updateMaestrosDropDown(maestros) {
    $("#gridServicios").data("kendoGrid").columns.forEach(function (column) {
        if (column.field === "Maestro") {
            column.editor = function (container, options) {
                $('<input name="' + options.field + '"/>')
                    .appendTo(container)
                    .kendoDropDownList({
                        autoBind: true,
                        dataTextField: "NombreApellido",
                        dataValueField: "ID",
                        dataSource: {
                            data: maestros,
                        }
                    });
            };
        }
    });
}
function updateMantencionDropDown(mantencion) {
    $("#gridMantencion").data("kendoGrid").columns.forEach(function (column) {
        if (column.field === "Mantencion") {
            column.editor = function (container, options) {
                $('<input required name="' + options.field + '"/>')
                    .appendTo(container)
                    .kendoDropDownList({
                        autoBind: true,
                        dataTextField: "Mantencion",
                        dataValueField: "ID",
                        dataSource: {
                            data: mantencion
                        },
                        change: function (e) {
                            var dropdown = this;
                            var dataItem = dropdown.dataItem();
                            options.model.set("Mantencion", dataItem);
                            if (!options.model.get("Precio")) {
                                if (dataItem && dataItem.Precio) {
                                    options.model.set("Precio", dataItem.Precio);
                                } else {
                                    options.model.set("Precio", 0);
                                }
                            }
                        }
                    });
            };
        }
    });
}

var dataSourceDifuntos = new kendo.data.DataSource({
    data: [],
    pageSize: 5,
    schema: {
        model: {
            id: "ID",
            fields: {
                ID: { type: "number" },
                NombreApellido: {
                    type: "string", validation: { required: true }
                },
                Rut: {
                    type: "string",
                    validation: {
                        required: true,
                        rutValido: function (input) {
                            if (input.is("[name='Rut']") && !validarRut(input.val())) {
                                input.attr("data-rutValido-msg", "RUT inválido.");
                                return false;
                            }
                            return true;
                        }
                    }
                },
                Edad: {
                    type: "number", validation: { required: true }
                },
                FechaDefuncion: {
                    type: "date", validation: { required: true }
                },
                CausaID: { type: "number" },
                LugarID: { type: "number" },
                Causa: {
                    defaultValue: {
                        ID: null, Causa: "", IsActive: true
                    },
                },
                Lugar: {
                    defaultValue: {
                        ID: null, Lugar: "", IsActive: true
                    },
                },
                Reuso: {
                    type: "boolean"
                },
                FechaReuso: {
                    type: "date"
                },
                NroOrden: {
                    type: "string"
                },
                AdjuntarOrden: {
                    type: "string"
                }
            }
        }
    }
});

var dataSourceFormaPago = new kendo.data.DataSource({
    data: [],
    schema: {
        model: {
            id: "ID",
            fields: {
                ID: { type: "number" },
                FormaPago: {
                    defaultValue: {
                        ID: null, FormaPago: "", IsActive: true
                    },
                },
                FechaPago: {
                    type: "date", validation: { required: true }
                },
                Banco: {
                    defaultValue: {
                        ID: null, Banco: "", IsActive: true
                    },
                },
                NroCheque: {
                    type: "text"
                },
                NroRecaudacion: {
                    type: "number"
                },
                Monto: {
                    type: "number", validation: { required: true }
                }
            }
        }
    }
});

var dataSourceServicios = new kendo.data.DataSource({
    data: [],
    pageSize: 5,
    schema: {
        model: {
            id: "ID",
            fields: {
                ID: { type: "number" },
                Servicio: {
                    defaultValue: {
                        ID: null, Servicio: "", IsActive: true
                    },
                },
                Maestro: {
                    defaultValue: {
                        ID: null, NombreApellido: "", IsActive: true
                    },
                },
                Precio: {
                    type: "number", validation: { required: true }
                }
            }
        }
    }
});

var dataSourceMantencion = new kendo.data.DataSource({
    data: [],
    pageSize: 5,
    schema: {
        model: {
            id: "ID",
            fields: {
                ID: { type: "number" },
                Mantencion: {
                    defaultValue: {
                        ID: null, Mantencion: "", IsActive: true
                    },
                },
                Precio: { type: "number", validation: { required: true } },
                Anio: { type: "string", validation: { required: true } }
            }
        }
    }
});

function calcularSaldo() {
    var mantencionGrid  = $("#gridMantencion").data("kendoGrid");
    var mantencionData;
    var totalMantencion = 0;
    var valorStr = $("#Valor").val();
    var valorSinSeparador = valorStr.replace(/\./g, '');
    var valorProducto = parseFloat(valorSinSeparador) || 0;

    var serviciosData = $("#gridServicios").data("kendoGrid").dataSource.view();
    var totalServicios = 0;

    if (mantencionGrid) {
        mantencionData = mantencionGrid.dataSource.view();
    }

    $.each(serviciosData, function (index, item) {
        totalServicios += item.Precio || 0;
    });
    $.each(mantencionData, function (index, item) {
        totalMantencion += item.Precio || 0;
    });

    var pagosData = $("#gridFormasPago").data("kendoGrid").dataSource.view();
    var totalPagos = 0;

    $.each(pagosData, function (index, item) {
        totalPagos += item.Monto || 0;
    });

    var total = valorProducto + totalServicios + totalMantencion;
    var saldoRestante = total - totalPagos;
    $("#SaldoTotal").val(formatNumber(total));
    $("#SaldoCancelado").val(formatNumber(totalPagos));
    $("#SaldoRestante").val(formatNumber(saldoRestante));
    showToast("info", `Saldo Restante: ${formatNumber(saldoRestante)}`);    
}

function getData(urlController, parametros) {
    if (typeof parametros === "undefined")
        return $.get(urlController);
    else
        return $.get(urlController, { id: parametros });
}

function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

$(document).ready(function () {
    $('.rut-input').on('input', function () {        
        var formattedRut = formatRut($(this).val());
        $(this).val(formattedRut);
    });    
    $('.rut-input').on('blur', function () {
        var rut = $(this).val();
        if (!validateRut(rut)) {
            showToast("info", "Rut Invalido");
            $(this).val('');
        }
    });

    $('input[type="text"], input[type="email"], input[type="textarea"]').on('input', function () {
        this.value = this.value.toUpperCase();
    });

    window.addEventListener('storage', function (event) {
        if (event.key === 'logout' && event.newValue === 'true') {
            window.location.href = '/Account/Login';
        }
    });

    window.addEventListener('load', function () {
        if (localStorage.getItem('logout') === 'true') {
            localStorage.removeItem('logout');
        }
    });

    //$("#CompRut").on("blur", function (e) {
    //    $.ajax({
    //        url: GetCompradorByRut,
    //        type: 'GET',
    //        data: {
    //            rut: e.currentTarget.value
    //        },
    //        success: function (data) {
    //            if (data) {
    //                $("#CompNombre").val(data.Nombre);
    //                $("#CompApellido").val(data.Apellido);
    //                $("#CompRegionId").val(data.Comuna.RegionID).change();
    //                $("#CompDireccion1").val(data.Direccion1);
    //                $("#CompDireccion2").val(data.Direccion2);
    //                $("#CompDirNum").val(data.DirNum);
    //                $("#CompTelefono").val(data.Telefono);
    //                $("#CompEmail").val(data.Email);

    //                changeSelectCascade(GetComunasByRegionIdAsync, data.Comuna.RegionID, $("#CompComunaID")).then(function () {
    //                    $("#CompComunaID").val(data.ComunaID);
    //                });
    //            }
    //            else {
    //                showToast("info", "El rut ingresado no existe.")
    //            }
    //        }

    function handleRutBlur(prefix) {
        $(`#${prefix}Rut`).on("blur", function (e) {
            $.ajax({
                url: GetCompradorByRut,
                type: 'GET',
                data: {
                    rut: e.currentTarget.value
                },
                success: function (response) {
                    if (response.Success) {
                        const data = response.Data;
                        $(`#${prefix}Nombre`).val(data.Nombre);
                        $(`#${prefix}Apellido`).val(data.Apellido);
                        $(`#${prefix}RegionId`).val(data.Comuna.RegionID).change();
                        $(`#${prefix}Direccion1`).val(data.Direccion1);
                        $(`#${prefix}DirNum`).val(data.DirNum);
                        $(`#${prefix}Telefono`).val(data.Telefono);
                        $(`#${prefix}Email`).val(data.Email);

                        changeSelectCascade(GetComunasByRegionIdAsync, data.Comuna.RegionID, $(`#${prefix}ComunaID`)).then(function () {
                            $(`#${prefix}ComunaID`).val(data.ComunaID);
                        });
                    } else {
                        showToast("info", response.Message);
                    }
                },
                error: function (e) {
                    showToast("error", "Ocurrió un error al buscar el RUT.");
                }
            });
        });
    }

    handleRutBlur("Comp");
    handleRutBlur("Ref");
});